#region License Information
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

namespace HeuristicLab.Operators {
  /// <summary>
  /// An operator which acts as a placeholder for another operator retrieved from the scope or a parent execution context.
  /// </summary>
  [Item("Placeholder", "An operator which acts as a placeholder for another operator retrieved from the scope or a parent execution context.")]
  [StorableClass]
  public sealed class Placeholder : SingleSuccessorOperator {
    public LookupParameter<IOperator> OperatorParameter {
      get { return (LookupParameter<IOperator>)Parameters["Operator"]; }
    }

    [StorableConstructor]
    private Placeholder(bool deserializing) : base(deserializing) { }
    private Placeholder(Placeholder original, Cloner cloner)
      : base(original, cloner) {
    }
    public Placeholder()
      : base() {
      Parameters.Add(new LookupParameter<IOperator>("Operator", "The operator which is retrieved from the scope or a parent execution context and applied on the current scope."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Placeholder(this, cloner);
    }

    public override IOperation Apply() {
      OperationCollection next = new OperationCollection(base.Apply());
      IOperator op = OperatorParameter.ActualValue;
      if (op != null)
        next.Insert(0, ExecutionContext.CreateOperation(op));
      return next;
    }
  }
}
