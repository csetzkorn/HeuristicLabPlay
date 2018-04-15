﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using HeuristicLab.Common;
using HeuristicLab.Core;
using TS = System.Threading.Tasks;


namespace HeuristicLab.Clients.Hive.SlaveCore {
  /// <summary>
  /// The core component of the Hive Slave. 
  /// Handles commands sent from the Hive Server and does all webservice calls for jobs. 
  /// </summary>
  public class Core : MarshalByRefObject {
    private static HeartbeatManager heartbeatManager;
    public static HeartbeatManager HeartbeatManager {
      get { return heartbeatManager; }
    }

    public EventLog ServiceEventLog { get; set; }

    private Semaphore waitShutdownSem = new Semaphore(0, 1);
    private bool abortRequested;
    private ServiceHost slaveComm;
    private WcfService wcfService;
    private TaskManager taskManager;
    private ConfigManager configManager;
    private PluginManager pluginManager;

    public Core() {
      var log = new ThreadSafeLog(SlaveCore.Properties.Settings.Default.MaxLogCount);
      this.pluginManager = new PluginManager(WcfService.Instance, log);
      this.taskManager = new TaskManager(pluginManager, log);
      log.MessageAdded += new EventHandler<EventArgs<string>>(log_MessageAdded);

      RegisterTaskManagerEvents();

      this.configManager = new ConfigManager(taskManager);
      ConfigManager.Instance = this.configManager;
    }

    /// <summary>
    /// Main method for the client
    /// </summary>
    public void Start() {
      abortRequested = false;
      EventLogManager.ServiceEventLog = ServiceEventLog;

      try {
        //start the client communication service (pipe between slave and slave gui)
        slaveComm = new ServiceHost(typeof(SlaveCommunicationService));

        try {
          slaveComm.Open();
        }
        catch (AddressAlreadyInUseException ex) {
          if (ServiceEventLog != null) {
            EventLogManager.LogException(ex);
          }
        }

        // delete all left over task directories
        pluginManager.CleanPluginTemp();
        SlaveClientCom.Instance.LogMessage("Hive Slave started");

        wcfService = WcfService.Instance;
        RegisterServiceEvents();

        StartHeartbeats(); // Start heartbeats thread        
        DispatchMessageQueue(); // dispatch messages until abortRequested
      }
      catch (Exception ex) {
        if (ServiceEventLog != null) {
          EventLogManager.LogException(ex);
        } else {
          //try to log with SlaveClientCom.Instance. if this works the user sees at least a message, 
          //else an exception will be thrown anyways.
          SlaveClientCom.Instance.LogMessage(string.Format("Uncaught exception: {0} {1} Core is going to shutdown.", ex.ToString(), Environment.NewLine));
        }
        ShutdownCore();
      }
      finally {
        DeregisterServiceEvents();
        waitShutdownSem.Release();
      }
    }

    private void StartHeartbeats() {
      //Initialize the heartbeat     
      if (heartbeatManager == null) {
        heartbeatManager = new HeartbeatManager();
        heartbeatManager.StartHeartbeat();
      }
    }

    private void DispatchMessageQueue() {
      MessageQueue queue = MessageQueue.GetInstance();
      while (!abortRequested) {
        MessageContainer container = queue.GetMessage();
        DetermineAction(container);
        if (!abortRequested) {
          SlaveClientCom.Instance.StatusChanged(configManager.GetStatusForClientConsole());
        }
      }
    }

    private void RegisterServiceEvents() {
      WcfService.Instance.Connected += new EventHandler(WcfService_Connected);
      WcfService.Instance.ExceptionOccured += new EventHandler<EventArgs<Exception>>(WcfService_ExceptionOccured);
    }

    private void DeregisterServiceEvents() {
      WcfService.Instance.Connected -= WcfService_Connected;
      WcfService.Instance.ExceptionOccured -= WcfService_ExceptionOccured;
    }

    private void WcfService_ExceptionOccured(object sender, EventArgs<Exception> e) {
      SlaveClientCom.Instance.LogMessage(string.Format("Connection to server interruped with exception: {0}", e.Value.Message));
    }

    private void WcfService_Connected(object sender, EventArgs e) {
      SlaveClientCom.Instance.LogMessage("Connected successfully to Hive server");
    }

    /// <summary>
    /// Reads and analyzes the Messages from the MessageQueue and starts corresponding actions
    /// </summary>
    /// <param name="container">The container, containing the message</param>
    private void DetermineAction(MessageContainer container) {
      SlaveClientCom.Instance.LogMessage(string.Format("Message: {0} for task: {1} ", container.Message.ToString(), container.TaskId));

      switch (container.Message) {
        case MessageContainer.MessageType.CalculateTask:
          CalculateTaskAsync(container.TaskId);
          break;
        case MessageContainer.MessageType.AbortTask:
          AbortTaskAsync(container.TaskId);
          break;
        case MessageContainer.MessageType.StopTask:
          StopTaskAsync(container.TaskId);
          break;
        case MessageContainer.MessageType.PauseTask:
          PauseTaskAsync(container.TaskId);
          break;
        case MessageContainer.MessageType.StopAll:
          DoStopAll();
          break;
        case MessageContainer.MessageType.PauseAll:
          DoPauseAll();
          break;
        case MessageContainer.MessageType.AbortAll:
          DoAbortAll();
          break;
        case MessageContainer.MessageType.ShutdownSlave:
          ShutdownCore();
          break;
        case MessageContainer.MessageType.Restart:
          DoStartSlave();
          break;
        case MessageContainer.MessageType.Sleep:
          Sleep();
          break;
        case MessageContainer.MessageType.SayHello:
          wcfService.Connect(configManager.GetClientInfo());
          break;
        case MessageContainer.MessageType.NewHBInterval:
          int interval = wcfService.GetNewHeartbeatInterval(ConfigManager.Instance.GetClientInfo().Id);
          if (interval != -1) {
            HeartbeatManager.Interval = TimeSpan.FromSeconds(interval);
          }
          break;
        case MessageContainer.MessageType.ShutdownComputer:
          ShutdownComputer();
          break;
      }
    }

    private void CalculateTaskAsync(Guid jobId) {
      TS.Task.Factory.StartNew(HandleCalculateTask, jobId)
      .ContinueWith((t) => {
        SlaveStatusInfo.IncrementTasksFailed();
        SlaveClientCom.Instance.LogMessage(t.Exception.ToString());
      }, TaskContinuationOptions.OnlyOnFaulted);
    }

    private void StopTaskAsync(Guid jobId) {
      TS.Task.Factory.StartNew(HandleStopTask, jobId)
       .ContinueWith((t) => {
         SlaveStatusInfo.IncrementTasksFailed();
         SlaveClientCom.Instance.LogMessage(t.Exception.ToString());
       }, TaskContinuationOptions.OnlyOnFaulted);
    }

    private void PauseTaskAsync(Guid jobId) {
      TS.Task.Factory.StartNew(HandlePauseTask, jobId)
       .ContinueWith((t) => {
         SlaveStatusInfo.IncrementTasksFailed();
         SlaveClientCom.Instance.LogMessage(t.Exception.ToString());
       }, TaskContinuationOptions.OnlyOnFaulted);
    }

    private void AbortTaskAsync(Guid jobId) {
      TS.Task.Factory.StartNew(HandleAbortTask, jobId)
       .ContinueWith((t) => {
         SlaveStatusInfo.IncrementTasksFailed();
         SlaveClientCom.Instance.LogMessage(t.Exception.ToString());
       }, TaskContinuationOptions.OnlyOnFaulted);
    }

    private void HandleCalculateTask(object taskIdObj) {
      Guid taskId = (Guid)taskIdObj;
      Task task = null;
      int usedCores = 0;
      try {
        task = wcfService.GetTask(taskId);
        if (task == null) throw new TaskNotFoundException(taskId);
        if (ConfigManager.Instance.GetFreeCores() < task.CoresNeeded) throw new OutOfCoresException();
        if (ConfigManager.Instance.GetFreeMemory() < task.MemoryNeeded) throw new OutOfMemoryException();
        SlaveStatusInfo.IncrementUsedCores(task.CoresNeeded); usedCores = task.CoresNeeded;
        TaskData taskData = wcfService.GetTaskData(taskId);
        if (taskData == null) throw new TaskDataNotFoundException(taskId);
        task = wcfService.UpdateJobState(taskId, TaskState.Calculating, null);
        if (task == null) throw new TaskNotFoundException(taskId);
        taskManager.StartTaskAsync(task, taskData);
      }
      catch (TaskNotFoundException) {
        SlaveStatusInfo.DecrementUsedCores(usedCores);
        throw;
      }
      catch (TaskDataNotFoundException) {
        SlaveStatusInfo.DecrementUsedCores(usedCores);
        throw;
      }
      catch (TaskAlreadyRunningException) {
        SlaveStatusInfo.DecrementUsedCores(usedCores);
        throw;
      }
      catch (OutOfCoresException) {
        wcfService.UpdateJobState(taskId, TaskState.Waiting, "No more cores available");
        throw;
      }
      catch (OutOfMemoryException) {
        wcfService.UpdateJobState(taskId, TaskState.Waiting, "No more memory available");
        throw;
      }
      catch (Exception e) {
        SlaveStatusInfo.DecrementUsedCores(usedCores);
        wcfService.UpdateJobState(taskId, TaskState.Failed, e.ToString());
        throw;
      }
    }

    private void HandleStopTask(object taskIdObj) {
      Guid taskId = (Guid)taskIdObj;
      try {
        Task task = wcfService.GetTask(taskId);
        if (task == null) throw new TaskNotFoundException(taskId);
        taskManager.StopTaskAsync(taskId);
      }
      catch (TaskNotFoundException) {
        throw;
      }
      catch (TaskNotRunningException) {
        throw;
      }
      catch (AppDomainNotCreatedException) {
        throw;
      }
    }

    private void HandlePauseTask(object taskIdObj) {
      Guid taskId = (Guid)taskIdObj;
      try {
        Task task = wcfService.GetTask(taskId);
        if (task == null) throw new TaskNotFoundException(taskId);
        taskManager.PauseTaskAsync(taskId);
      }
      catch (TaskNotFoundException) {
        throw;
      }
      catch (TaskNotRunningException) {
        throw;
      }
      catch (AppDomainNotCreatedException) {
        throw;
      }
    }

    private void HandleAbortTask(object taskIdObj) {
      Guid taskId = (Guid)taskIdObj;
      try {
        taskManager.AbortTask(taskId);
      }
      catch (TaskNotFoundException) {
        throw;
      }
    }

    #region TaskManager Events
    private void RegisterTaskManagerEvents() {
      this.taskManager.TaskStarted += new EventHandler<EventArgs<SlaveTask>>(taskManager_TaskStarted);
      this.taskManager.TaskPaused += new EventHandler<EventArgs<SlaveTask, TaskData>>(taskManager_TaskPaused);
      this.taskManager.TaskStopped += new EventHandler<EventArgs<SlaveTask, TaskData>>(taskManager_TaskStopped);
      this.taskManager.TaskFailed += new EventHandler<EventArgs<Tuple<SlaveTask, TaskData, Exception>>>(taskManager_TaskFailed);
      this.taskManager.TaskAborted += new EventHandler<EventArgs<SlaveTask>>(taskManager_TaskAborted);
    }

    private void taskManager_TaskStarted(object sender, EventArgs<SlaveTask> e) {
      // successfully started, everything is good
    }

    private void taskManager_TaskPaused(object sender, EventArgs<SlaveTask, TaskData> e) {
      try {
        SlaveStatusInfo.DecrementUsedCores(e.Value.CoresNeeded);
        heartbeatManager.AwakeHeartBeatThread();
        Task task = wcfService.GetTask(e.Value.TaskId);
        if (task == null) throw new TaskNotFoundException(e.Value.TaskId);
        task.ExecutionTime = e.Value.ExecutionTime;
        TaskData taskData = e.Value2;
        wcfService.UpdateTaskData(task, taskData, configManager.GetClientInfo().Id, TaskState.Paused);
      }
      catch (TaskNotFoundException ex) {
        SlaveClientCom.Instance.LogMessage(ex.ToString());
      }
      catch (Exception ex) {
        SlaveClientCom.Instance.LogMessage(ex.ToString());
      }
    }

    private void taskManager_TaskStopped(object sender, EventArgs<SlaveTask, TaskData> e) {
      try {
        SlaveStatusInfo.DecrementUsedCores(e.Value.CoresNeeded);
        heartbeatManager.AwakeHeartBeatThread();
        Task task = wcfService.GetTask(e.Value.TaskId);
        if (task == null) throw new TaskNotFoundException(e.Value.TaskId);
        task.ExecutionTime = e.Value.ExecutionTime;
        TaskData taskData = e.Value2;
        wcfService.UpdateTaskData(task, taskData, configManager.GetClientInfo().Id, TaskState.Finished);
      }
      catch (TaskNotFoundException ex) {
        SlaveClientCom.Instance.LogMessage(ex.ToString());
      }
      catch (Exception ex) {
        SlaveClientCom.Instance.LogMessage(ex.ToString());
      }
    }

    private void taskManager_TaskFailed(object sender, EventArgs<Tuple<SlaveTask, TaskData, Exception>> e) {
      try {
        SlaveStatusInfo.DecrementUsedCores(e.Value.Item1.CoresNeeded);
        heartbeatManager.AwakeHeartBeatThread();
        SlaveTask slaveTask = e.Value.Item1;
        TaskData taskData = e.Value.Item2;
        Exception exception = e.Value.Item3;

        Task task = wcfService.GetTask(slaveTask.TaskId);
        if (task == null) throw new TaskNotFoundException(slaveTask.TaskId);
        task.ExecutionTime = slaveTask.ExecutionTime;
        if (taskData != null) {
          wcfService.UpdateTaskData(task, taskData, configManager.GetClientInfo().Id, TaskState.Failed, exception.ToString());
        } else {
          wcfService.UpdateJobState(task.Id, TaskState.Failed, exception.ToString());
        }
        SlaveClientCom.Instance.LogMessage(exception.Message);
      }
      catch (TaskNotFoundException ex) {
        SlaveStatusInfo.IncrementTasksFailed();
        SlaveClientCom.Instance.LogMessage(ex.ToString());
      }
      catch (Exception ex) {
        SlaveStatusInfo.IncrementTasksFailed();
        SlaveClientCom.Instance.LogMessage(ex.ToString());
      }
    }

    private void taskManager_TaskAborted(object sender, EventArgs<SlaveTask> e) {
      SlaveStatusInfo.DecrementUsedCores(e.Value.CoresNeeded);
    }
    #endregion

    #region Log Events
    private void log_MessageAdded(object sender, EventArgs<string> e) {
      try {
        SlaveClientCom.Instance.LogMessage(e.Value.Split('\t')[1]);
      }
      catch { }
    }
    #endregion

    /// <summary>
    /// aborts all running jobs, no results are sent back
    /// </summary>
    private void DoAbortAll() {
      SlaveClientCom.Instance.LogMessage("Aborting all jobs.");
      foreach (Guid taskId in taskManager.TaskIds) {
        AbortTaskAsync(taskId);
      }
    }

    /// <summary>
    /// wait for jobs to finish, then pause client
    /// </summary>
    private void DoPauseAll() {
      SlaveClientCom.Instance.LogMessage("Pausing all jobs.");
      foreach (Guid taskId in taskManager.TaskIds) {
        PauseTaskAsync(taskId);
      }
    }

    /// <summary>
    /// pause slave immediately
    /// </summary>
    private void DoStopAll() {
      SlaveClientCom.Instance.LogMessage("Stopping all jobs.");
      foreach (Guid taskId in taskManager.TaskIds) {
        StopTaskAsync(taskId);
      }
    }

    #region Slave Lifecycle Methods
    /// <summary>
    /// completly shudown slave
    /// </summary>
    public void Shutdown() {
      MessageContainer mc = new MessageContainer(MessageContainer.MessageType.ShutdownSlave);
      MessageQueue.GetInstance().AddMessage(mc);
      waitShutdownSem.WaitOne();
    }

    private void ShutdownComputer() {
      var t = TS.Task.Factory.StartNew(new Action(Shutdown));
      t.ContinueWith(c => {
        try {
          //we assume that *.exe means an executable in the current directory, otherwise it is a command
          if (SlaveCore.Properties.Settings.Default.ShutdownCommand.EndsWith(".exe")) {
            string dirName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Process.Start(Path.Combine(dirName, SlaveCore.Properties.Settings.Default.ShutdownCommand));
          } else {
            Process.Start(SlaveCore.Properties.Settings.Default.ShutdownCommand);
          }
        }
        catch (Exception ex) {
          if (ServiceEventLog != null) {
            EventLogManager.LogException(ex);
          } else
            throw ex;
        }
      });
    }

    /// <summary>
    /// complete shutdown, should be called before the the application is exited
    /// </summary>
    private void ShutdownCore() {
      SlaveClientCom.Instance.LogMessage("Shutdown signal received");
      SlaveClientCom.Instance.LogMessage("Stopping heartbeat");
      heartbeatManager.StopHeartBeat();
      SlaveClientCom.Instance.LogMessage("Stopping checkpointing");
      taskManager.StopCheckpointing();

      abortRequested = true;

      DoAbortAll();

      SlaveClientCom.Instance.LogMessage("Logging out");
      WcfService.Instance.Disconnect();
      SlaveClientCom.Instance.ClientCom.Shutdown();
      SlaveClientCom.Close();

      if (slaveComm.State != CommunicationState.Closed)
        slaveComm.Close();
    }

    /// <summary>
    /// reinitializes everything and continues operation, 
    /// can be called after Sleep()
    /// </summary>  
    private void DoStartSlave() {
      SlaveClientCom.Instance.LogMessage("Restart received");
      configManager.Asleep = false;
    }

    /// <summary>
    /// stop slave, except for client gui communication, 
    /// primarily used by gui if core is running as windows service
    /// </summary>    
    private void Sleep() {
      SlaveClientCom.Instance.LogMessage("Sleep received - not accepting any new jobs");
      configManager.Asleep = true;
      DoPauseAll();
    }
    #endregion
  }
}
