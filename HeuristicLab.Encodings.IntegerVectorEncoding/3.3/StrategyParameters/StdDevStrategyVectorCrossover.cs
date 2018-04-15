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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.IntegerVectorEncoding {
  [Item("StdDevStrategyVectorCrossover", "Crosses the strategy vector by using intermediate recombination (average crossover).")]
  [StorableClass]
  public class StdDevStrategyVectorCrossover : SingleSuccessorOperator, IStochasticOperator, IIntegerVectorStdDevStrategyParameterCrossover {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ILookupParameter<ItemArray<DoubleArray>> ParentsParameter {
      get { return (ILookupParameter<ItemArray<DoubleArray>>)Parameters["ParentStrategyParameter"]; }
    }
    public ILookupParameter<DoubleArray> StrategyParameterParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters["StrategyParameter"]; }
    }

    [StorableConstructor]
    protected StdDevStrategyVectorCrossover(bool deserializing) : base(deserializing) { }
    protected StdDevStrategyVectorCrossover(StdDevStrategyVectorCrossover original, Cloner cloner) : base(original, cloner) { }
    public StdDevStrategyVectorCrossover()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleArray>("ParentStrategyParameter", "The strategy parameters to cross."));
      Parameters.Add(new LookupParameter<DoubleArray>("StrategyParameter", "The crossed strategy parameter."));
      ParentsParameter.ActualName = "StrategyParameter";
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new StdDevStrategyVectorCrossover(this, cloner);
    }

    public override IOperation Apply() {
      StrategyParameterParameter.ActualValue = Average(RandomParameter.ActualValue, ParentsParameter.ActualValue);
      return base.Apply();
    }

    private DoubleArray Average(IRandom random, ItemArray<DoubleArray> parents) {
      int length = parents[0].Length;
      var result = new DoubleArray(length);
      for (int i = 0; i < length; i++) {
        for (int p = 0; p < parents.Length; p++) {
          result[i] += parents[p][i];
        }
        result[i] /= parents.Length;
      }
      return result;
    }
  }
}
