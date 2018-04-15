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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Encodings.Alba;

namespace HeuristicLab.Problems.VehicleRouting {
  [Item("AlbaLambdaInterchangeMoveEvaluator", "Evaluates a lamnbda interchange move for a VRP representation.  It is implemented as described in Alba, E. and Dorronsoro, B. (2004). Solving the Vehicle Routing Problem by Using Cellular Genetic Algorithms.")]
  [StorableClass]
  public sealed class AlbaLambdaInterchangeMoveEvaluator : AlbaMoveEvaluator, IAlbaLambdaInterchangeMoveOperator {
    public ILookupParameter<AlbaLambdaInterchangeMove> LambdaInterchangeMoveParameter {
      get { return (ILookupParameter<AlbaLambdaInterchangeMove>)Parameters["AlbaLambdaInterchangeMove"]; }
    }

    public override ILookupParameter VRPMoveParameter {
      get { return LambdaInterchangeMoveParameter; }
    }

    [StorableConstructor]
    private AlbaLambdaInterchangeMoveEvaluator(bool deserializing) : base(deserializing) { }

    public AlbaLambdaInterchangeMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<AlbaLambdaInterchangeMove>("AlbaLambdaInterchangeMove", "The move to evaluate."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlbaLambdaInterchangeMoveEvaluator(this, cloner);
    }

    private AlbaLambdaInterchangeMoveEvaluator(AlbaLambdaInterchangeMoveEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected override void EvaluateMove() {
      AlbaEncoding newSolution = LambdaInterchangeMoveParameter.ActualValue.Individual.Clone() as AlbaEncoding;
      AlbaLambdaInterchangeMoveMaker.Apply(newSolution, LambdaInterchangeMoveParameter.ActualValue);

      UpdateEvaluation(newSolution);
    }
  }
}
