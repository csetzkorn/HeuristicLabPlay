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
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class KeijzerFunctionTwelve : ArtificialRegressionDataDescriptor {

    public override string Name { get { return "Keijzer 12 f(x, y) = x^4 - x³ + y² / 2 - y"; } }
    public override string Description {
      get {
        return "Paper: Improving Symbolic Regression with Interval Arithmetic and Linear Scaling" + Environment.NewLine
               + "Authors: Maarten Keijzer" + Environment.NewLine
               + "Function: f(x, y) = x^4 - x³ + y² / 2 - y" + Environment.NewLine
               + "range(train): 20 Training cases x,y = rnd(-3, 3)" + Environment.NewLine
               + "range(test): x,y = [-3:0.1:3]" + Environment.NewLine
               + "Function Set: x + y, x * y, 1/x, -x, sqrt(x)" + Environment.NewLine
               + "Comments: Reduced test set compared to original publication!";
      }
    }
    protected override string TargetVariable { get { return "F"; } }
    protected override string[] VariableNames { get { return new string[] { "X", "Y", "F" }; } }
    protected override string[] AllowedInputVariables { get { return new string[] { "X", "Y" }; } }
    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return 20; } }
    protected override int TestPartitionStart { get { return 20; } }
    protected override int TestPartitionEnd { get { return 20 + (61 * 61); } }
    public int Seed { get; private set; }

    public KeijzerFunctionTwelve() : this((int)System.DateTime.Now.Ticks) {
    }
    public KeijzerFunctionTwelve(int seed) : base() {
      Seed = seed;
    }
    protected override List<List<double>> GenerateValues() {
      List<List<double>> data = new List<List<double>>();
      List<double> oneVariableTestData = SequenceGenerator.GenerateSteps(-3, 3, 0.1m).Select(v => (double)v).ToList();
      List<List<double>> testData = new List<List<double>>() { oneVariableTestData, oneVariableTestData };

      var combinations = ValueGenerator.GenerateAllCombinationsOfValuesInLists(testData).ToList();
      var rand = new MersenneTwister((uint)Seed);

      for (int i = 0; i < AllowedInputVariables.Count(); i++) {
        data.Add(ValueGenerator.GenerateUniformDistributedValues(rand.Next(), 20, -3, 3).ToList());
        data[i].AddRange(combinations[i]);
      }

      double x, y;
      List<double> results = new List<double>();
      for (int i = 0; i < data[0].Count; i++) {
        x = data[0][i];
        y = data[1][i];
        results.Add(Math.Pow(x, 4) - Math.Pow(x, 3) + Math.Pow(y, 2) / 2 - y);
      }
      data.Add(results);

      return data;
    }
  }
}
