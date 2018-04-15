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
using HeuristicLab.Core;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {
  [Item("DTLZ4", "Testfunction as defined as DTLZ4 in http://repository.ias.ac.in/81671/ [30.11.15]")]
  [StorableClass]
  public class DTLZ4 : DTLZ {
    protected override double GetBestKnownHypervolume(int objectives) {
      if (objectives == 2) return 121.0 - 1.0 / 4.0 * Math.PI;
      return -1;
    }

    [StorableConstructor]
    protected DTLZ4(bool deserializing) : base(deserializing) { }
    protected DTLZ4(DTLZ4 original, Cloner cloner) : base(original, cloner) { }
    public DTLZ4() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new DTLZ4(this, cloner);
    }

    public override double[] Evaluate(RealVector r, int objectives) {
      if (r.Length < objectives) {
        throw new ArgumentException("The dimensionality of the problem(ProblemSize) must be larger than or equal to the number of objectives");
      }
      double[] res = new double[objectives];

      //calculate g(Xm)
      double g = 0;
      for (int i = objectives; i < r.Length; i++) {
        double d = r[i] - 0.5;
        g += d * d;
      }

      //calculating f0...fM-1
      for (int i = 0; i < objectives; i++) {
        double f = i == 0 ? 1 : (Math.Sin(Math.Pow(r[objectives - i - 1], 100) * Math.PI / 2)) * (1 + g);
        for (int j = 0; j < objectives - i - 1; j++) {
          f *= Math.Cos(Math.Pow(r[j], 100) * Math.PI / 2);
        }
        res[i] = f;
      }
      return res;
    }
  }
}
