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
using System.Linq;
using HeuristicLab.Common;

namespace HeuristicLab.Analysis.Statistics {
  public static class EnumerableStatisticExtensions {
    public static Tuple<double, double> ConfidenceIntervals(this IEnumerable<double> values, double alpha) {
      return ConfidenceIntervals(values.ToArray(), alpha);
    }

    public static Tuple<double, double> ConfidenceIntervals(this double[] values, double alpha) {
      double lower, upper;
      int n = values.Length;
      if (n <= 1) return new Tuple<double, double>(double.NaN, double.NaN);

      double s = values.StandardDeviation();
      double x = values.Average();
      double t = alglib.invstudenttdistribution(n - 1, (1.0 - alpha) / 2.0);

      lower = x + t * (s / Math.Sqrt(n));
      upper = x - t * (s / Math.Sqrt(n));

      return new Tuple<double, double>(lower, upper);
    }
  }
}
