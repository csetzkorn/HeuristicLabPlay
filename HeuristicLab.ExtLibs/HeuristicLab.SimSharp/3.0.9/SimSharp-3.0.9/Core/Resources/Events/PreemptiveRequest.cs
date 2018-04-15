﻿#region License Information
/* SimSharp - A .NET port of SimPy, discrete event simulation framework
Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.*/
#endregion

using System;

namespace SimSharp {
  public sealed class PreemptiveRequest : PriorityRequest {
    public bool Preempt { get; private set; }

    public PreemptiveRequest(Environment environment, Action<Event> callback, Action<Event> disposeCallback, int priority = 1, bool preempt = false)
      : base(environment, callback, disposeCallback, priority) {
      Preempt = preempt;
    }
  }
}
