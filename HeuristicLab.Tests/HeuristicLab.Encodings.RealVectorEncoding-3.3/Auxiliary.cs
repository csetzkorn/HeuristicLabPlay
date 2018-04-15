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

namespace HeuristicLab.Encodings.RealVectorEncoding.Tests {
  public static class Auxiliary {
    public static bool RealVectorIsAlmostEqualByPosition(RealVector p1, RealVector p2) {
      bool equal = (p1.Length == p2.Length);
      if (equal) {
        for (int i = 0; i < p1.Length; i++) {
          if (!p1[i].IsAlmost(p2[i])) {
            equal = false;
            break;
          }
        }
      }
      return equal;
    }
  }
}
