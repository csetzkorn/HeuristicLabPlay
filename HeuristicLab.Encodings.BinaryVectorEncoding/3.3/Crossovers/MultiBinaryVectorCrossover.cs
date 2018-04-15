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
using System.Linq;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  [Item("MultiBinaryVectorCrossover", "Randomly selects and applies one of its crossovers every time it is called.")]
  [StorableClass]
  public class MultiBinaryVectorCrossover : StochasticMultiBranch<IBinaryVectorCrossover>, IBinaryVectorCrossover, IStochasticOperator {
    public override bool CanChangeName {
      get { return false; }
    }
    protected override bool CreateChildOperation {
      get { return true; }
    }

    public ILookupParameter<ItemArray<BinaryVector>> ParentsParameter {
      get { return (ILookupParameter<ItemArray<BinaryVector>>)Parameters["Parents"]; }
    }

    public ILookupParameter<BinaryVector> ChildParameter {
      get { return (ILookupParameter<BinaryVector>)Parameters["Child"]; }
    }

    [StorableConstructor]
    protected MultiBinaryVectorCrossover(bool deserializing) : base(deserializing) { }
    protected MultiBinaryVectorCrossover(MultiBinaryVectorCrossover original, Cloner cloner) : base(original, cloner) { }
    public MultiBinaryVectorCrossover()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<BinaryVector>("Parents", "The parent binary vector which should be crossed."));
      ParentsParameter.ActualName = "BinaryVector";
      Parameters.Add(new LookupParameter<BinaryVector>("Child", "The child binary vector resulting from the crossover."));
      ChildParameter.ActualName = "BinaryVector";

      foreach (Type type in ApplicationManager.Manager.GetTypes(typeof(IBinaryVectorCrossover))) {
        if (!typeof(MultiOperator<IBinaryVectorCrossover>).IsAssignableFrom(type))
          Operators.Add((IBinaryVectorCrossover)Activator.CreateInstance(type), true);
      }

      SelectedOperatorParameter.ActualName = "SelectedCrossoverOperator";
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiBinaryVectorCrossover(this, cloner);
    }

    protected override void Operators_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<IBinaryVectorCrossover>> e) {
      base.Operators_ItemsReplaced(sender, e);
      ParameterizeCrossovers();
    }

    protected override void Operators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IBinaryVectorCrossover>> e) {
      base.Operators_ItemsAdded(sender, e);
      ParameterizeCrossovers();
    }

    private void ParameterizeCrossovers() {
      foreach (IBinaryVectorCrossover crossover in Operators.OfType<IBinaryVectorCrossover>()) {
        crossover.ChildParameter.ActualName = ChildParameter.Name;
        crossover.ParentsParameter.ActualName = ParentsParameter.Name;
      }
      foreach (IStochasticOperator crossover in Operators.OfType<IStochasticOperator>()) {
        crossover.RandomParameter.ActualName = RandomParameter.Name;
      }
    }

    public override IOperation InstrumentedApply() {
      if (Operators.Count == 0) throw new InvalidOperationException(Name + ": Please add at least one binary vector crossover to choose from.");
      return base.InstrumentedApply();
    }
  }
}
