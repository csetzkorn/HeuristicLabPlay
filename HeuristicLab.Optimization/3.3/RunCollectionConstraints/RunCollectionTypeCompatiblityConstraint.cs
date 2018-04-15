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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [StorableClass]
  [Item("RunCollectionTypeCompatibilityConstraint", "A constraint which checks the members of the contained runs for type compabitiliby to the constraint data.")]
  public class RunCollectionTypeCompatibilityConstraint : TypeCompatibilityConstraint, IRunCollectionColumnConstraint {
    [StorableConstructor]
    protected RunCollectionTypeCompatibilityConstraint(bool deserializing) {
    }
    protected RunCollectionTypeCompatibilityConstraint(RunCollectionTypeCompatibilityConstraint original, Cloner cloner)
      : base(original, cloner) {
      ConstraintData = original.ConstraintData;
      ConstraintOperation = original.ConstraintOperation;
      constraintColumn = original.constraintColumn;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RunCollectionTypeCompatibilityConstraint(this, cloner);
    }

    public RunCollectionTypeCompatibilityConstraint()
      : base() {
    }
    public RunCollectionTypeCompatibilityConstraint(RunCollection constrainedValue, ConstraintOperation constraintOperation, Type constraintData)
      : base(constrainedValue, constraintOperation, constraintData) {
    }
    public RunCollectionTypeCompatibilityConstraint(RunCollection constrainedValue, ConstraintOperation constraintOperation, Type constraintData, bool active)
      : base(constrainedValue, constraintOperation, constraintData, active) {
    }

    public new RunCollection ConstrainedValue {
      get { return (RunCollection)base.ConstrainedValue; }
      set { base.ConstrainedValue = value; }
    }
    [Storable]
    private string constraintColumn;
    public string ConstraintColumn {
      get { return constraintColumn; }
      set {
        if (!((IStringConvertibleMatrix)ConstrainedValue).ColumnNames.Contains(value))
          throw new ArgumentException("Could not set ConstraintData to not existing column index.");
        if (constraintColumn != value) {
          constraintColumn = value;
          this.OnConstraintColumnChanged();
          this.OnToStringChanged();
        }
      }
    }

    public event EventHandler ConstraintColumnChanged;
    protected virtual void OnConstraintColumnChanged() {
      EventHandler handler = ConstraintColumnChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }

    protected override void OnConstrainedValueChanged() {
      base.OnConstrainedValueChanged();
      IStringConvertibleMatrix matrix = (IStringConvertibleMatrix)ConstrainedValue;
      if (constraintColumn == null && ConstrainedValue != null && matrix.Columns != 0)
        constraintColumn = matrix.ColumnNames.ElementAt(0);
    }

    protected override bool Check(object constrainedMember) {
      if (!Active)
        return true;

      foreach (IRun run in ConstrainedValue.Where(r => r.Visible)) {
        IItem item = ConstrainedValue.GetValue(run, constraintColumn);
        if (item == null || !base.Check(item))
          run.Visible = false;
      }
      return true;
    }

    protected override bool Check(object constrainedMember, out string errorMessage) {
      errorMessage = string.Empty;
      if (!Active)
        return true;

      foreach (IRun run in ConstrainedValue.Where(r => r.Visible)) {
        IItem item = ConstrainedValue.GetValue(run, constraintColumn);
        if (!base.Check(item))
          run.Visible = false;
      }
      return true;
    }

    public override string ToString() {
      string s = string.Empty;
      IStringConvertibleMatrix matrix = ConstrainedValue;
      if (matrix != null && matrix.ColumnNames.Count() != 0)
        s += constraintColumn + " ";
      else
        return "TypeCompabitilityConstraint";

      if (ConstraintOperation != null)
        s += ConstraintOperation.ToString() + " ";

      if (ConstraintData != null)
        s += ConstraintData;
      else
        s += "null";

      return s;
    }
  }
}
