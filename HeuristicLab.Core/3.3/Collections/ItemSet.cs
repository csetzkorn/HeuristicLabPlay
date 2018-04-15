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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  [StorableClass]
  [Item("ItemSet", "Represents a set of items.")]
  public class ItemSet<T> : ObservableSet<T>, IItemSet<T> where T : class, IItem {
    public virtual string ItemName {
      get { return ItemAttribute.GetName(this.GetType()); }
    }
    public virtual string ItemDescription {
      get { return ItemAttribute.GetDescription(this.GetType()); }
    }
    public Version ItemVersion {
      get { return ItemAttribute.GetVersion(this.GetType()); }
    }
    public static Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Class; }
    }
    public virtual Image ItemImage {
      get { return ItemAttribute.GetImage(this.GetType()); }
    }

    [StorableConstructor]
    protected ItemSet(bool deserializing) : base(deserializing) { }
    protected ItemSet(ItemSet<T> original, Cloner cloner) {
      cloner.RegisterClonedObject(original, this);
      set = new HashSet<T>(original.Select(cloner.Clone), original.set.Comparer);
    }
    public ItemSet() : base() { }
    public ItemSet(IEnumerable<T> collection) : base(collection) { }
    public ItemSet(IEqualityComparer<T> comparer) : base(comparer) { }
    public ItemSet(IEnumerable<T> collection, IEqualityComparer<T> comparer) : base(collection, comparer) { }

    public object Clone() {
      return Clone(new Cloner());
    }
    public virtual IDeepCloneable Clone(Cloner cloner) {
      return new ItemSet<T>(this, cloner);
    }

    public new ReadOnlyItemSet<T> AsReadOnly() {
      return new ReadOnlyItemSet<T>(this);
    }

    public override string ToString() {
      return ItemName;
    }

    public event EventHandler ItemImageChanged;
    protected virtual void OnItemImageChanged() {
      EventHandler handler = ItemImageChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler ToStringChanged;
    protected virtual void OnToStringChanged() {
      EventHandler handler = ToStringChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
  }
}
