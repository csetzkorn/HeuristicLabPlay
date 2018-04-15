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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {
  [Item("IHR6", "Testfunction as defined as IHR6 in \"Igel, C., Hansen, N., & Roth, S. (2007). Covariance matrix adaptation for multi-objective optimization. Evolutionary computation, 15(1), 1-28.\" [24.06.16]")]
  [StorableClass]
  public class IHR6 : IHR {
    protected override IEnumerable<double[]> GetOptimalParetoFront(int objectives) {
      List<double[]> res = new List<double[]>();
      for (int i = 0; i <= 500; i++) {
        RealVector r = new RealVector(objectives);
        r[0] = 1 / 500.0 * i;
        res.Add(this.Evaluate(r, objectives));
      }
      return res;
    }

    protected override double GetBestKnownHypervolume(int objectives) {
      return Hypervolume.Calculate(GetOptimalParetoFront(objectives), GetReferencePoint(objectives), GetMaximization(objectives));
    }

    [StorableConstructor]
    protected IHR6(bool deserializing) : base(deserializing) { }
    protected IHR6(IHR6 original, Cloner cloner) : base(original, cloner) { }
    public IHR6() : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new IHR6(this, cloner);
    }

    protected override double F1(RealVector y) {
      return 1 - Math.Exp(-4 * Math.Abs(y[0])) * Math.Pow(Math.Sin(6 * Math.PI * y[0]), 6);
    }

    protected override double F2(RealVector y) {
      var g = G(y);
      var d = H(y[0], y) / g;
      return g * HF(1 - d * d, y);
    }

    protected override double G(RealVector y) {
      double sum = 0.0;
      for (int i = 1; i < y.Length; i++) {
        sum += HG(y[i]);
      }
      return 1 + 9 * Math.Pow(sum / (y.Length - 1), 0.25);
    }
  }
}
