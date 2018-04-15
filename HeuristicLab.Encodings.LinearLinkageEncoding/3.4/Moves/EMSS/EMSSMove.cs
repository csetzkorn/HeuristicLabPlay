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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("EMSSMove", "Base class for shift, split, merge, and extract moves.")]
  [StorableClass]
  public abstract class EMSSMove : Item {
    [Storable]
    private int item;
    public int Item { get { return item; } }

    [StorableConstructor]
    protected EMSSMove(bool deserializing) : base(deserializing) { }
    protected EMSSMove(EMSSMove original, Cloner cloner)
      : base(original, cloner) {
      item = original.item;
    }
    protected EMSSMove(int item) : base() {
      this.item = item;
    }
    
    public abstract void Apply(LinearLinkage lle);
    public abstract void Undo(LinearLinkage lle);

    public abstract void ApplyToLLEb(int[] lleb);
  }
}
