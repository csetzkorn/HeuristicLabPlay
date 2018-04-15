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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.Potvin {
  [Item("PotvinPDExchangeMultiMoveGenerator", "Generates exchange moves from a given PDP encoding.")]
  [StorableClass]
  public sealed class PotvinPDExchangeMultiMoveGenerator : PotvinPDExchangeMoveGenerator, IMultiMoveGenerator, IMultiVRPMoveGenerator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    public IValueLookupParameter<IntValue> SampleSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["SampleSize"]; }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PotvinPDExchangeMultiMoveGenerator(this, cloner);
    }

    [StorableConstructor]
    private PotvinPDExchangeMultiMoveGenerator(bool deserializing) : base(deserializing) { }

    public PotvinPDExchangeMultiMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
      Parameters.Add(new ValueLookupParameter<IntValue>("SampleSize", "The number of moves to generate."));
    }

    private PotvinPDExchangeMultiMoveGenerator(PotvinPDExchangeMultiMoveGenerator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected override PotvinPDExchangeMove[] GenerateMoves(PotvinEncoding individual, IVRPProblemInstance problemInstance) {
      List<PotvinPDExchangeMove> result = new List<PotvinPDExchangeMove>();

      for (int i = 0; i < SampleSizeParameter.ActualValue.Value; i++) {
        var move = PotvinPDExchangeSingleMoveGenerator.Apply(individual, ProblemInstance, RandomParameter.ActualValue);
        if (move != null)
          result.Add(move);
      }

      return result.ToArray();
    }
  }
}
