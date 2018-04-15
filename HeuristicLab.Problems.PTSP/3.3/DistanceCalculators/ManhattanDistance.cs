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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Problems.PTSP {
  [Item("Manhattan Distance", "Calculates the Manhattan distance.")]
  [StorableClass]
  public sealed class ManhattanDistance : DistanceCalculator {

    [StorableConstructor]
    private ManhattanDistance(bool deserializing) : base(deserializing) { }
    private ManhattanDistance(ManhattanDistance original, Cloner cloner) : base(original, cloner) { }
    public ManhattanDistance() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ManhattanDistance(this, cloner);
    }

    public override double Calculate(int from, int to, DoubleMatrix coordinates) {
      return DistanceHelper.GetDistance(DistanceMeasure.Manhattan, coordinates[from, 0], coordinates[from, 1], coordinates[to, 0], coordinates[to, 1]);
    }
  }
}
