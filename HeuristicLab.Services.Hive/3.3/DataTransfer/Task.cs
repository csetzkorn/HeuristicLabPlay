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
using System.Runtime.Serialization;

namespace HeuristicLab.Services.Hive.DataTransfer {
  [DataContract]
  [Serializable]
  public class Task : LightweightTask {
    [DataMember]
    public int Priority { get; set; }
    [DataMember]
    public int CoresNeeded { get; set; }
    [DataMember]
    public int MemoryNeeded { get; set; }
    [DataMember]
    public List<Guid> PluginsNeededIds { get; set; }
    [DataMember]
    public DateTime? LastHeartbeat { get; set; }
    [DataMember]
    public bool IsParentTask { get; set; }
    [DataMember]
    public bool FinishWhenChildJobsFinished { get; set; }
    [DataMember]
    public Guid JobId { get; set; }
    // BackwardsCompatibility3.3
    #region Backwards compatible code, remove with 3.4
    [DataMember]
    public bool IsPrivileged { get; set; }
    #endregion

    public Task() {
      PluginsNeededIds = new List<Guid>();
      IsPrivileged = true;
    }
  }
}
