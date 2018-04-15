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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [Item("Symbolic Regression Problem (single-objective)", "Represents a single objective symbolic regression problem.")]
  [StorableClass]
  [Creatable(CreatableAttribute.Categories.GeneticProgrammingProblems, Priority = 100)]
  public class SymbolicRegressionSingleObjectiveProblem : SymbolicDataAnalysisSingleObjectiveProblem<IRegressionProblemData, ISymbolicRegressionSingleObjectiveEvaluator, ISymbolicDataAnalysisSolutionCreator>, IRegressionProblem {
    private const double PunishmentFactor = 10;
    private const int InitialMaximumTreeDepth = 8;
    private const int InitialMaximumTreeLength = 25;
    private const string EstimationLimitsParameterName = "EstimationLimits";
    private const string EstimationLimitsParameterDescription = "The limits for the estimated value that can be returned by the symbolic regression model.";

    #region parameter properties
    public IFixedValueParameter<DoubleLimit> EstimationLimitsParameter {
      get { return (IFixedValueParameter<DoubleLimit>)Parameters[EstimationLimitsParameterName]; }
    }
    #endregion
    #region properties
    public DoubleLimit EstimationLimits {
      get { return EstimationLimitsParameter.Value; }
    }
    #endregion
    [StorableConstructor]
    protected SymbolicRegressionSingleObjectiveProblem(bool deserializing) : base(deserializing) { }
    protected SymbolicRegressionSingleObjectiveProblem(SymbolicRegressionSingleObjectiveProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }
    public override IDeepCloneable Clone(Cloner cloner) { return new SymbolicRegressionSingleObjectiveProblem(this, cloner); }

    public SymbolicRegressionSingleObjectiveProblem()
      : base(new RegressionProblemData(), new SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator(), new SymbolicDataAnalysisExpressionTreeCreator()) {
      Parameters.Add(new FixedValueParameter<DoubleLimit>(EstimationLimitsParameterName, EstimationLimitsParameterDescription));

      EstimationLimitsParameter.Hidden = true;


      ApplyLinearScalingParameter.Value.Value = true;
      Maximization.Value = true;
      MaximumSymbolicExpressionTreeDepth.Value = InitialMaximumTreeDepth;
      MaximumSymbolicExpressionTreeLength.Value = InitialMaximumTreeLength;

      RegisterEventHandlers();
      ConfigureGrammarSymbols();
      InitializeOperators();
      UpdateEstimationLimits();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
      // compatibility
      bool changed = false;
      if (!Operators.OfType<SymbolicRegressionSingleObjectiveTrainingParetoBestSolutionAnalyzer>().Any()) {
        Operators.Add(new SymbolicRegressionSingleObjectiveTrainingParetoBestSolutionAnalyzer());
        changed = true;
      }
      if (!Operators.OfType<SymbolicRegressionSingleObjectiveValidationParetoBestSolutionAnalyzer>().Any()) {
        Operators.Add(new SymbolicRegressionSingleObjectiveValidationParetoBestSolutionAnalyzer());
        changed = true;
      }
      if (!Operators.OfType<SymbolicRegressionSolutionsAnalyzer>().Any()) {
        Operators.Add(new SymbolicRegressionSolutionsAnalyzer());
        changed = true;
      }
      if (changed) {
        ParameterizeOperators();
      }
    }

    private void RegisterEventHandlers() {
      SymbolicExpressionTreeGrammarParameter.ValueChanged += (o, e) => ConfigureGrammarSymbols();
    }

    private void ConfigureGrammarSymbols() {
      var grammar = SymbolicExpressionTreeGrammar as TypeCoherentExpressionGrammar;
      if (grammar != null) grammar.ConfigureAsDefaultRegressionGrammar();
    }

    private void InitializeOperators() {
      Operators.Add(new SymbolicRegressionSingleObjectiveTrainingBestSolutionAnalyzer());
      Operators.Add(new SymbolicRegressionSingleObjectiveValidationBestSolutionAnalyzer());
      Operators.Add(new SymbolicRegressionSingleObjectiveOverfittingAnalyzer());
      Operators.Add(new SymbolicRegressionSingleObjectiveTrainingParetoBestSolutionAnalyzer());
      Operators.Add(new SymbolicRegressionSingleObjectiveValidationParetoBestSolutionAnalyzer());
      Operators.Add(new SymbolicRegressionSolutionsAnalyzer());
      Operators.Add(new SymbolicExpressionTreePhenotypicSimilarityCalculator());
      Operators.Add(new SymbolicRegressionPhenotypicDiversityAnalyzer(Operators.OfType<SymbolicExpressionTreePhenotypicSimilarityCalculator>()) { DiversityResultName = "Phenotypic Diversity" });
      ParameterizeOperators();
    }

    private void UpdateEstimationLimits() {
      if (ProblemData.TrainingIndices.Any()) {
        var targetValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, ProblemData.TrainingIndices).ToList();
        var mean = targetValues.Average();
        var range = targetValues.Max() - targetValues.Min();
        EstimationLimits.Upper = mean + PunishmentFactor * range;
        EstimationLimits.Lower = mean - PunishmentFactor * range;
      } else {
        EstimationLimits.Upper = double.MaxValue;
        EstimationLimits.Lower = double.MinValue;
      }
    }

    protected override void OnProblemDataChanged() {
      base.OnProblemDataChanged();
      UpdateEstimationLimits();
    }

    protected override void ParameterizeOperators() {
      base.ParameterizeOperators();
      if (Parameters.ContainsKey(EstimationLimitsParameterName)) {
        var operators = Parameters.OfType<IValueParameter>().Select(p => p.Value).OfType<IOperator>().Union(Operators);
        foreach (var op in operators.OfType<ISymbolicDataAnalysisBoundedOperator>()) {
          op.EstimationLimitsParameter.ActualName = EstimationLimitsParameter.Name;
        }
      }

      foreach (var op in Operators.OfType<ISolutionSimilarityCalculator>()) {
        op.SolutionVariableName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
        op.QualityVariableName = Evaluator.QualityParameter.ActualName;

        if (op is SymbolicExpressionTreePhenotypicSimilarityCalculator) {
          var phenotypicSimilarityCalculator = (SymbolicExpressionTreePhenotypicSimilarityCalculator)op;
          phenotypicSimilarityCalculator.ProblemData = ProblemData;
          phenotypicSimilarityCalculator.Interpreter = SymbolicExpressionTreeInterpreter;
        }
      }
    }
  }
}
