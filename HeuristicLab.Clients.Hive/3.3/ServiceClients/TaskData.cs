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
using HeuristicLab.Common;

namespace HeuristicLab.Clients.Hive {
  public partial class TaskData : IDeepCloneable, IContent {
    public TaskData() { }

    protected TaskData(TaskData original, Cloner cloner) {
      cloner.RegisterClonedObject(original, this);
      if (original.Data != null) {
        this.Data = new byte[original.Data.Length];
        Array.Copy(original.Data, this.Data, original.Data.Length);
      }
      this.LastUpdate = original.LastUpdate;
    }

    public IDeepCloneable Clone(Cloner cloner) {
      return new TaskData(this, cloner);
    }

    public object Clone() {
      return Clone(new Cloner());
    }
  }
}