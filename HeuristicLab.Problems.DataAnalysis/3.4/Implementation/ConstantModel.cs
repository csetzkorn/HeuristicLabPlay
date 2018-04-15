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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableClass]
  [Item("Constant Model", "A model that always returns the same constant value regardless of the presented input data.")]
  public class ConstantModel : RegressionModel, IClassificationModel, ITimeSeriesPrognosisModel, IStringConvertibleValue {
    public override IEnumerable<string> VariablesUsedForPrediction { get { return Enumerable.Empty<string>(); } }


    [Storable]
    private readonly double constant;
    public double Constant {
      get { return constant; }
      // setter not implemented because manipulation of the constant is not allowed
    }

    [StorableConstructor]
    protected ConstantModel(bool deserializing) : base(deserializing) { }
    protected ConstantModel(ConstantModel original, Cloner cloner)
      : base(original, cloner) {
      this.constant = original.constant;
    }

    public override IDeepCloneable Clone(Cloner cloner) { return new ConstantModel(this, cloner); }

    public ConstantModel(double constant, string targetVariable)
      : base(targetVariable) {
      this.name = ItemName;
      this.description = ItemDescription;
      this.constant = constant;
      this.ReadOnly = true; // changing a constant regression model is not supported
    }

    public override IEnumerable<double> GetEstimatedValues(IDataset dataset, IEnumerable<int> rows) {
      return rows.Select(row => Constant);
    }
    public IEnumerable<double> GetEstimatedClassValues(IDataset dataset, IEnumerable<int> rows) {
      return GetEstimatedValues(dataset, rows);
    }
    public IEnumerable<IEnumerable<double>> GetPrognosedValues(IDataset dataset, IEnumerable<int> rows, IEnumerable<int> horizons) {
      return rows.Select(_ => horizons.Select(__ => Constant));
    }

    public override IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      return new ConstantRegressionSolution(this, new RegressionProblemData(problemData));
    }
    public IClassificationSolution CreateClassificationSolution(IClassificationProblemData problemData) {
      return new ConstantClassificationSolution(this, new ClassificationProblemData(problemData));
    }
    public ITimeSeriesPrognosisSolution CreateTimeSeriesPrognosisSolution(ITimeSeriesPrognosisProblemData problemData) {
      return new TimeSeriesPrognosisSolution(this, new TimeSeriesPrognosisProblemData(problemData));
    }

    public override string ToString() {
      return string.Format("Constant: {0}", GetValue());
    }

    #region IStringConvertibleValue
    public bool ReadOnly { get; private set; }
    public bool Validate(string value, out string errorMessage) {
      throw new NotSupportedException(); // changing a constant regression model is not supported
    }

    public string GetValue() {
      return string.Format("{0:E4}", constant);
    }

    public bool SetValue(string value) {
      throw new NotSupportedException(); // changing a constant regression model is not supported
    }

#pragma warning disable 0067
    public event EventHandler ValueChanged;
#pragma warning restore 0067
    #endregion

  }
}
