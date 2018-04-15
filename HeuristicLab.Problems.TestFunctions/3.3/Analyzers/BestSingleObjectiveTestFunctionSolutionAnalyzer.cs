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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TestFunctions {
  /// <summary>
  /// An operator for analyzing the best solution for a SingleObjectiveTestFunction problem.
  /// </summary>
  [Item("BestSingleObjectiveTestFunctionSolutionAnalyzer", "An operator for analyzing the best solution for a SingleObjectiveTestFunction problem.")]
  [StorableClass]
  public class BestSingleObjectiveTestFunctionSolutionAnalyzer : SingleSuccessorOperator, IBestSingleObjectiveTestFunctionSolutionAnalyzer {
    public virtual bool EnabledByDefault {
      get { return true; }
    }

    public LookupParameter<BoolValue> MaximizationParameter {
      get { return (LookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ScopeTreeLookupParameter<RealVector> RealVectorParameter {
      get { return (ScopeTreeLookupParameter<RealVector>)Parameters["RealVector"]; }
    }
    ILookupParameter IBestSingleObjectiveTestFunctionSolutionAnalyzer.RealVectorParameter {
      get { return RealVectorParameter; }
    }
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    ILookupParameter IBestSingleObjectiveTestFunctionSolutionAnalyzer.QualityParameter {
      get { return QualityParameter; }
    }
    public ILookupParameter<SingleObjectiveTestFunctionSolution> BestSolutionParameter {
      get { return (ILookupParameter<SingleObjectiveTestFunctionSolution>)Parameters["BestSolution"]; }
    }
    public ILookupParameter<RealVector> BestKnownSolutionParameter {
      get { return (ILookupParameter<RealVector>)Parameters["BestKnownSolution"]; }
    }
    public ILookupParameter<DoubleValue> BestKnownQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    public IValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (IValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }
    public IValueLookupParameter<ISingleObjectiveTestFunctionProblemEvaluator> EvaluatorParameter {
      get { return (IValueLookupParameter<ISingleObjectiveTestFunctionProblemEvaluator>)Parameters["Evaluator"]; }
    }
    public ILookupParameter<DoubleMatrix> BoundsParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["Bounds"]; }
    }

    [StorableConstructor]
    protected BestSingleObjectiveTestFunctionSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    protected BestSingleObjectiveTestFunctionSolutionAnalyzer(BestSingleObjectiveTestFunctionSolutionAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public BestSingleObjectiveTestFunctionSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem."));
      Parameters.Add(new ScopeTreeLookupParameter<RealVector>("RealVector", "The SingleObjectiveTestFunction solutions from which the best solution should be visualized."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The qualities of the SingleObjectiveTestFunction solutions which should be visualized."));
      Parameters.Add(new LookupParameter<SingleObjectiveTestFunctionSolution>("BestSolution", "The best SingleObjectiveTestFunction solution."));
      Parameters.Add(new LookupParameter<RealVector>("BestKnownSolution", "The best known solution."));
      Parameters.Add(new LookupParameter<DoubleValue>("BestKnownQuality", "The quality of the best known solution."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the SingleObjectiveTestFunction solution should be stored."));
      Parameters.Add(new ValueLookupParameter<ISingleObjectiveTestFunctionProblemEvaluator>("Evaluator", "The evaluator with which the solution is evaluated."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("Bounds", "The bounds of the function."));

      MaximizationParameter.Hidden = true;
      RealVectorParameter.Hidden = true;
      QualityParameter.Hidden = true;
      BestSolutionParameter.Hidden = true;
      BestKnownSolutionParameter.Hidden = true;
      BestKnownQualityParameter.Hidden = true;
      ResultsParameter.Hidden = true;
      EvaluatorParameter.Hidden = true;
      BoundsParameter.Hidden = true;
    }

    /// <summary>
    /// This method can simply be removed when the plugin version is > 3.3
    /// </summary>
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      // Bounds are introduced in 3.3.0.3894
      if (!Parameters.ContainsKey("Bounds"))
        Parameters.Add(new LookupParameter<DoubleMatrix>("Bounds", "The bounds of the function."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestSingleObjectiveTestFunctionSolutionAnalyzer(this, cloner);
    }

    public override IOperation Apply() {
      ItemArray<RealVector> realVectors = RealVectorParameter.ActualValue;
      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      bool max = MaximizationParameter.ActualValue.Value;
      DoubleValue bestKnownQuality = BestKnownQualityParameter.ActualValue;
      SingleObjectiveTestFunctionSolution solution = BestSolutionParameter.ActualValue;

      int i = -1;
      if (!max) i = qualities.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;
      else i = qualities.Select((x, index) => new { index, x.Value }).OrderByDescending(x => x.Value).First().index;

      if (bestKnownQuality == null ||
          max && qualities[i].Value > bestKnownQuality.Value
          || !max && qualities[i].Value < bestKnownQuality.Value) {
        BestKnownQualityParameter.ActualValue = new DoubleValue(qualities[i].Value);
        BestKnownSolutionParameter.ActualValue = (RealVector)realVectors[i].Clone();
        if (solution != null)
          solution.BestKnownRealVector = BestKnownSolutionParameter.ActualValue;
      }

      if (solution == null) {
        ResultCollection results = ResultsParameter.ActualValue;
        solution = new SingleObjectiveTestFunctionSolution((RealVector)realVectors[i].Clone(),
                                                           (DoubleValue)qualities[i].Clone(),
                                                           EvaluatorParameter.ActualValue);
        solution.Population = realVectors[i].Length == 2
          ? new ItemArray<RealVector>(realVectors.Select(x => x.Clone()).Cast<RealVector>())
          : null;
        solution.BestKnownRealVector = BestKnownSolutionParameter.ActualValue;
        solution.Bounds = BoundsParameter.ActualValue;
        BestSolutionParameter.ActualValue = solution;
        results.Add(new Result("Best Solution", solution));
      } else {
        if (max && qualities[i].Value > solution.BestQuality.Value
          || !max && qualities[i].Value < solution.BestQuality.Value) {
          solution.BestRealVector = (RealVector)realVectors[i].Clone();
          solution.BestQuality = (DoubleValue)qualities[i].Clone();
        }
        solution.Population = realVectors[i].Length == 2
          ? new ItemArray<RealVector>(realVectors.Select(x => x.Clone()).Cast<RealVector>())
          : null;
      }

      return base.Apply();
    }
  }
}
