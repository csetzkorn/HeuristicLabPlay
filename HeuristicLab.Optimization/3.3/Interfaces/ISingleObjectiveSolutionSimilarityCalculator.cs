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

namespace HeuristicLab.Optimization {
  // use HeuristicLab.Optimization.ISolutionSimilarityCalculator instead
  // BackwardsCompatibility3.3
  #region Backwards compatible code, remove with 3.4
  /// <summary>
  /// An interface which represents an operator for similarity calculation between single objective solutions.
  /// </summary>
  [Obsolete("use HeuristicLab.Optimization.ISolutionSimilarityCalculator instead")]
  public interface ISingleObjectiveSolutionSimilarityCalculator : ISolutionSimilarityCalculator, ISingleObjectiveOperator {

  }
  #endregion
}
