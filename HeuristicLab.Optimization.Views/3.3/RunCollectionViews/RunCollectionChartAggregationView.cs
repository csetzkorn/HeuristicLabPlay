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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Collections;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Optimization.Views {
  [View("Chart Aggregation")]
  [Content(typeof(RunCollection), false)]
  public partial class RunCollectionChartAggregationView : ItemView {
    private const string AllDataRows = "All DataRows";

    public new RunCollection Content {
      get { return (RunCollection)base.Content; }
      set { base.Content = value; }
    }

    private int rowNumber;
    private bool suppressUpdates;
    private readonly Dictionary<IRun, IEnumerable<DataRow>> runMapping;
    private readonly DataTable combinedDataTable;
    public DataTable CombinedDataTable {
      get { return combinedDataTable; }
    }

    public RunCollectionChartAggregationView() {
      InitializeComponent();
      runMapping = new Dictionary<IRun, IEnumerable<DataRow>>();
      combinedDataTable = new DataTable("Combined DataTable", "A data table containing data rows from multiple runs.");
      viewHost.Content = combinedDataTable;
      suppressUpdates = false;
    }

    #region Content events
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ItemsAdded += Content_ItemsAdded;
      Content.ItemsRemoved += Content_ItemsRemoved;
      Content.CollectionReset += Content_CollectionReset;
      Content.UpdateOfRunsInProgressChanged += Content_UpdateOfRunsInProgressChanged;
      Content.OptimizerNameChanged += Content_AlgorithmNameChanged;
    }
    protected override void DeregisterContentEvents() {
      Content.ItemsAdded -= Content_ItemsAdded;
      Content.ItemsRemoved -= Content_ItemsRemoved;
      Content.CollectionReset -= Content_CollectionReset;
      Content.UpdateOfRunsInProgressChanged -= Content_UpdateOfRunsInProgressChanged;
      Content.OptimizerNameChanged -= Content_AlgorithmNameChanged;
      base.DeregisterContentEvents();
    }

    private void Content_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      if (suppressUpdates) return;
      if (InvokeRequired) {
        Invoke(new CollectionItemsChangedEventHandler<IRun>(Content_ItemsAdded), sender, e);
        return;
      }
      UpdateDataTableComboBox(); // will trigger AddRuns
    }
    private void Content_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      if (suppressUpdates) return;
      if (InvokeRequired) {
        Invoke(new CollectionItemsChangedEventHandler<IRun>(Content_ItemsRemoved), sender, e);
        return;
      }
      RemoveRuns(e.Items);
      UpdateDataTableComboBox();
      UpdateDataRowComboBox();
      RebuildCombinedDataTable();
    }
    private void Content_CollectionReset(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      if (suppressUpdates) return;
      if (InvokeRequired) {
        Invoke(new CollectionItemsChangedEventHandler<IRun>(Content_CollectionReset), sender, e);
        return;
      }
      RemoveRuns(e.OldItems);
      UpdateDataTableComboBox();
      UpdateDataRowComboBox();
      RebuildCombinedDataTable();
    }
    private void Content_AlgorithmNameChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_AlgorithmNameChanged), sender, e);
      else UpdateCaption();
    }
    private void Content_UpdateOfRunsInProgressChanged(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(Content_UpdateOfRunsInProgressChanged), sender, e);
        return;
      }
      suppressUpdates = Content.UpdateOfRunsInProgress;
      if (!suppressUpdates) {
        foreach (var run in runMapping)
          DeregisterRunEvents(run.Key);
        runMapping.Clear();
        combinedDataTable.Rows.Clear();
        UpdateDataTableComboBox();
      }
    }

    private void RegisterRunEvents(IRun run) {
      run.PropertyChanged += run_PropertyChanged;
    }
    private void DeregisterRunEvents(IRun run) {
      run.PropertyChanged -= run_PropertyChanged;
    }
    private void run_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      if (suppressUpdates) return;
      if (InvokeRequired) {
        Invoke((Action<object, PropertyChangedEventArgs>)run_PropertyChanged, sender, e);
      } else {
        var run = (IRun)sender;
        if (e.PropertyName == "Color" || e.PropertyName == "Visible")
          UpdateRuns(new[] { run });
      }
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      dataTableComboBox.Items.Clear();
      dataRowComboBox.Items.Clear();
      combinedDataTable.Rows.Clear();
      runMapping.Clear();

      UpdateCaption();
      if (Content != null) {
        UpdateDataTableComboBox();
      }
    }

    private void RebuildCombinedDataTable() {
      RemoveRuns(Content);
      rowNumber = 0;
      AddRuns(Content);
    }

    private void AddRuns(IEnumerable<IRun> runs) {
      foreach (var run in runs) {
        runMapping[run] = ExtractDataRowsFromRun(run).ToList();
        RegisterRunEvents(run);
      }
      var dataRows = runs.Where(r => r.Visible && runMapping.ContainsKey(r)).SelectMany(r => runMapping[r]);
      combinedDataTable.Rows.AddRange(dataRows);
    }

    private void RemoveRuns(IEnumerable<IRun> runs) {
      var dataRows = runs.Where(r => runMapping.ContainsKey(r)).SelectMany(r => runMapping[r]).ToList();
      foreach (var run in runs) {
        if (!runMapping.ContainsKey(run)) continue;
        runMapping.Remove(run);
        DeregisterRunEvents(run);
      }
      combinedDataTable.Rows.RemoveRange(dataRows);
    }

    private void UpdateRuns(IEnumerable<IRun> runs) {
      foreach (var run in runs) {
        //update color
        if (!runMapping.ContainsKey(run)) {
          runMapping[run] = ExtractDataRowsFromRun(run).ToList();
          RegisterRunEvents(run);
        } else {
          foreach (var dataRow in runMapping[run]) {
            dataRow.VisualProperties.Color = run.Color;
          }
        }
      }
      //update visibility - remove and add all rows to keep the same order as before
      combinedDataTable.Rows.Clear();
      combinedDataTable.Rows.AddRange(runMapping.Where(mapping => mapping.Key.Visible).SelectMany(mapping => mapping.Value));
    }

    private IEnumerable<DataRow> ExtractDataRowsFromRun(IRun run) {
      var resultName = (string)dataTableComboBox.SelectedItem;
      if (string.IsNullOrEmpty(resultName)) yield break;

      var rowName = (string)dataRowComboBox.SelectedItem;
      if (!run.Results.ContainsKey(resultName)) yield break;

      var dataTable = (DataTable)run.Results[resultName];
      foreach (var dataRow in dataTable.Rows) {
        if (dataRow.Name != rowName && rowName != AllDataRows) continue;
        rowNumber++;
        var clonedRow = (DataRow)dataRow.Clone();
        //row names must be unique -> add incremented number to the row name
        clonedRow.Name = run.Name + "." + dataRow.Name + rowNumber;
        clonedRow.VisualProperties.DisplayName = run.Name + "." + dataRow.Name;
        clonedRow.VisualProperties.Color = run.Color;
        yield return clonedRow;
      }
    }

    private void UpdateDataTableComboBox() {
      string selectedItem = (string)dataTableComboBox.SelectedItem;

      dataTableComboBox.Items.Clear();
      var dataTables = (from run in Content
                        from result in run.Results
                        where result.Value is DataTable
                        select result.Key).Distinct().ToArray();

      dataTableComboBox.Items.AddRange(dataTables);
      if (selectedItem != null && dataTableComboBox.Items.Contains(selectedItem)) {
        dataTableComboBox.SelectedItem = selectedItem;
      } else if (dataTableComboBox.Items.Count > 0) {
        dataTableComboBox.SelectedItem = dataTableComboBox.Items[0];
      }
    }

    private void UpdateCaption() {
      Caption = Content != null ? Content.OptimizerName + " Chart Aggregation" : ViewAttribute.GetViewName(GetType());
    }

    private void UpdateDataRowComboBox() {
      string selectedItem = (string)dataRowComboBox.SelectedItem;

      dataRowComboBox.Items.Clear();
      var resultName = (string)dataTableComboBox.SelectedItem;
      if (resultName == null) return;

      var dataTables = from run in Content
                       where run.Results.ContainsKey(resultName)
                       select run.Results[resultName] as DataTable;
      var rowNames = (from dataTable in dataTables
                      from row in dataTable.Rows
                      select row.Name).Distinct().ToArray();

      dataRowComboBox.Items.AddRange(rowNames);
      dataRowComboBox.Items.Add(AllDataRows);
      if (selectedItem != null && dataRowComboBox.Items.Contains(selectedItem)) {
        dataRowComboBox.SelectedItem = selectedItem;
      } else if (dataRowComboBox.Items.Count > 0) {
        dataRowComboBox.SelectedItem = dataRowComboBox.Items[0];
      }
    }

    private void dataTableComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      UpdateDataRowComboBox();
    }
    private void dataRowComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (suppressUpdates) return;
      RebuildCombinedDataTable();
    }
  }
}
