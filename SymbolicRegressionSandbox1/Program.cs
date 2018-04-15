using System;
using System.Linq;
using System.Threading;
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HeuristicLab.Problems.Instances.DataAnalysis;
using HeuristicLab.SequentialEngine;

namespace SymbolicRegressionSandbox1 {

  class Program {
    private readonly static ManualResetEvent mutex = new ManualResetEvent(false);
    private static int generationTracker = 0;

    static void Main(string[] args) {

      var path = @"D:\Data\Sinus\SimpleSinusHeuristicLab.csv";

      var parser = new TableFileParser();
      parser.Parse(path, columnNamesInFirstLine: true);

      var dataset = new Dataset(parser.VariableNames, parser.Values);

      var problemData = new RegressionProblemData(dataset, dataset.DoubleVariables.Take(dataset.Columns - 1), dataset.DoubleVariables.Last());

      var grammar = new TypeCoherentExpressionGrammar();
     var add = new Addition();
//      var sub = new Subtraction();
//      var mul = new Multiplication();
//      var div = new Division();
      grammar.ConfigureAsDefaultRegressionGrammar();

      var problem = new SymbolicRegressionSingleObjectiveProblem {
        ProblemData = problemData,
        SymbolicExpressionTreeInterpreter = new SymbolicDataAnalysisExpressionTreeLinearInterpreter(),
        SymbolicExpressionTreeGrammar = grammar,
        EvaluatorParameter = { Value = new SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator() },
        SolutionCreatorParameter = { Value = new SymbolicDataAnalysisExpressionTreeCreator() }
      };

      var ga = new GeneticAlgorithm {
        PopulationSize = { Value = 1000 },
        MaximumGenerations = { Value = 100 },
        Problem = problem,
        MutationProbability = new PercentValue(25),
        Engine = new SequentialEngine()
      };
      var manipulator = new MultiSymbolicExpressionTreeManipulator();
      ga.MutatorParameter.ValidValues.Add(manipulator);


      ga.ExecutionStateChanged += OnExecutionStateChanged;
      ga.ExecutionTimeChanged += OnExecutionTimeChanged;

      ga.Prepare();
      ga.Start();
      mutex.WaitOne();

    }

    private static void OnExecutionStateChanged(object sender, EventArgs e) {
      if (((IExecutable)sender).ExecutionState == ExecutionState.Stopped)
        mutex.Set();
    }

    private static void OnExecutionTimeChanged(object sender, EventArgs e) {
      var algorithm = (IAlgorithm)sender;
      
      if (!algorithm.Results.ContainsKey("Generations") || !algorithm.Results.ContainsKey("BestQuality"))
        return;

      var generations = (IntValue)algorithm.Results["Generations"].Value;
      var quality = (DoubleValue)algorithm.Results["BestQuality"].Value;

      if (generationTracker != generations.Value) {
        generationTracker = generations.Value;
        Console.WriteLine("Generation {0}: fitness = {1:0.000}", generations.Value, quality.Value);
      }
    }
  }
}
