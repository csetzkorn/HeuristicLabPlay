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

using HeuristicLab.Core;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  public interface INcaInitializer : IOperator {
    /// <summary>
    /// Calculates an initial projection for the NCA to start from.
    /// </summary>
    /// <param name="data">The problem data that contains the AllowedInputVariables and TrainingIndices.</param>
    /// <param name="dimensions">The amount of columns in the matrix</param>
    /// <returns>The matrix that projects the input variables into a lower dimensional space.</returns>
    double[,] Initialize(IClassificationProblemData data, int dimensions);
  }
}
