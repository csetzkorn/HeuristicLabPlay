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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Optimization.Views {
  [View("RunCollection View")]
  [Content(typeof(RunCollection), true)]
  [Content(typeof(IItemCollection<IRun>), false)]
  public sealed partial class RunCollectionView : ItemView {
    private Dictionary<IRun, List<ListViewItem>> itemListViewItemMapping;
    private bool validDragOperation;
    private bool suppressUpdates;

    public new IItemCollection<IRun> Content {
      get { return (IItemCollection<IRun>)base.Content; }
      set { base.Content = value; }
    }

    public RunCollection RunCollection {
      get { return Content as RunCollection; }
    }

    private int EmptyImageIndex { get { return 0; } }
    private int RunImageIndex { get { return 1; } }

    public RunCollectionView() {
      InitializeComponent();
      UpdateGroupBoxText();

      itemsListView.SmallImageList.Images.Add(HeuristicLab.Common.Resources.VSImageLibrary.Nothing);
      itemsListView.SmallImageList.Images.Add(HeuristicLab.Common.Resources.VSImageLibrary.Class);

      itemListViewItemMapping = new Dictionary<IRun, List<ListViewItem>>();
      runCollectionModifiersListView.Evaluator = EvaluateModifications;
    }

    protected override void DeregisterContentEvents() {
      Content.ItemsAdded -= new CollectionItemsChangedEventHandler<IRun>(Content_ItemsAdded);
      Content.ItemsRemoved -= new CollectionItemsChangedEventHandler<IRun>(Content_ItemsRemoved);
      Content.CollectionReset -= new CollectionItemsChangedEventHandler<IRun>(Content_CollectionReset);
      if (RunCollection != null)
        RunCollection.UpdateOfRunsInProgressChanged -= new EventHandler(RunCollection_UpdateOfRunsInProgressChanged);
      foreach (IRun run in itemListViewItemMapping.Keys) {
        DeregisterItemEvents(run);
      }
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ItemsAdded += new CollectionItemsChangedEventHandler<IRun>(Content_ItemsAdded);
      Content.ItemsRemoved += new CollectionItemsChangedEventHandler<IRun>(Content_ItemsRemoved);
      Content.CollectionReset += new CollectionItemsChangedEventHandler<IRun>(Content_CollectionReset);
      if (RunCollection != null)
        RunCollection.UpdateOfRunsInProgressChanged += new EventHandler(RunCollection_UpdateOfRunsInProgressChanged);
    }
    private void DeregisterItemEvents(IRun item) {
      item.ToStringChanged -= new EventHandler(Item_ToStringChanged);
      item.PropertyChanged -= Item_PropertyChanged;
    }
    private void RegisterItemEvents(IRun item) {
      item.ToStringChanged += new EventHandler(Item_ToStringChanged);
      item.PropertyChanged += Item_PropertyChanged;
    }

    protected override void OnInitialized(EventArgs e) {
      base.OnInitialized(e);
      var viewTypes = MainFormManager.GetViewTypes(typeof(RunCollection), true);
      foreach (Type viewType in viewTypes.OrderBy(x => ViewAttribute.GetViewName(x))) {
        if ((viewType != typeof(ItemCollectionView<IRun>)) && (viewType != typeof(ViewHost))) {
          ToolStripMenuItem menuItem = new ToolStripMenuItem();
          menuItem.Text = ViewAttribute.GetViewName(viewType);
          menuItem.Tag = viewType;
          menuItem.Click += new EventHandler(menuItem_Click);
          analyzeRunsToolStripDropDownButton.DropDownItems.Add(menuItem);
        }
      }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();

      string selectedName = null;
      if ((itemsListView.SelectedItems.Count == 1) && (itemsListView.SelectedItems[0].Tag != null))
        selectedName = ((IRun)itemsListView.SelectedItems[0].Tag).Name;

      itemsListView.Items.Clear();
      itemListViewItemMapping.Clear();
      viewHost.Content = null;

      UpdateGroupBoxText();

      if (Content != null) {
        if (RunCollection != null) {
          if (!tabControl.TabPages.Contains(constraintPage))
            tabControl.TabPages.Add(constraintPage);
          runCollectionConstraintCollectionView.Content = RunCollection.Constraints;
          runCollectionConstraintCollectionView.ReadOnly = itemsListView.Items.Count == 0;
          if (!tabControl.TabPages.Contains(modifiersPage))
            tabControl.TabPages.Add(modifiersPage);
          runCollectionModifiersListView.Content = RunCollection.Modifiers;
        }

        ListViewItem[] items = new ListViewItem[Content.Count];
        int count = 0;
        foreach (IRun item in Content) {
          ListViewItem listViewItem = CreateListViewItem(item);

          if ((selectedName != null) && item.Name.Equals(selectedName))
            listViewItem.Selected = true;
          items[count] = listViewItem;
          count++;
        }
        itemsListView.Items.AddRange(items);
        AdjustListViewColumnSizes();
      } else {
        runCollectionConstraintCollectionView.Content = null;
        if (tabControl.TabPages.Contains(constraintPage))
          tabControl.TabPages.Remove(constraintPage);
        if (tabControl.TabPages.Contains(modifiersPage))
          tabControl.TabPages.Remove(modifiersPage);
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      if (Content == null) {
        analyzeRunsToolStripDropDownButton.Enabled = false;
        runCollectionConstraintCollectionView.ReadOnly = true;
        itemsListView.Enabled = false;
        detailsGroupBox.Enabled = false;
        viewHost.Enabled = false;
        removeButton.Enabled = false;
        clearButton.Enabled = false;
      } else {
        analyzeRunsToolStripDropDownButton.Enabled = itemsListView.Items.Count > 0;
        runCollectionConstraintCollectionView.ReadOnly = itemsListView.Items.Count == 0;
        itemsListView.Enabled = true;
        detailsGroupBox.Enabled = itemsListView.SelectedItems.Count == 1;
        removeButton.Enabled = itemsListView.SelectedItems.Count > 0 && !Content.IsReadOnly && !ReadOnly;
        clearButton.Enabled = itemsListView.Items.Count > 0 && !Content.IsReadOnly && !ReadOnly;
        viewHost.Enabled = true;
      }
    }

    private static readonly string tooltipText = ItemAttribute.GetName(typeof(Run)) + ": " +
                                    ItemAttribute.GetDescription(typeof(Run));
    private ListViewItem CreateListViewItem(IRun run) {
      ListViewItem listViewItem = new ListViewItem();
      if (run == null) {
        listViewItem.Text = "null";
        listViewItem.ImageIndex = EmptyImageIndex;
        return listViewItem;
      }

      listViewItem.Text = run.Name;
      listViewItem.ToolTipText = tooltipText;
      listViewItem.ImageIndex = RunImageIndex;
      listViewItem.Tag = run;

      if (run.Visible) {
        listViewItem.Font = new Font(listViewItem.Font, FontStyle.Regular);
        listViewItem.ForeColor = run.Color;
      } else {
        listViewItem.Font = new Font(listViewItem.Font, FontStyle.Italic);
        listViewItem.ForeColor = Color.LightGray;
      }

      if (!itemListViewItemMapping.ContainsKey(run)) {
        itemListViewItemMapping.Add(run, new List<ListViewItem>());
        RegisterItemEvents(run);
      }
      itemListViewItemMapping[run].Add(listViewItem);

      return listViewItem;
    }

    private void RemoveListViewItem(ListViewItem listViewItem) {
      if (listViewItem == null) throw new ArgumentNullException();
      IRun run = listViewItem.Tag as IRun;
      if (run != null) {
        itemListViewItemMapping[run].Remove(listViewItem);
        if (itemListViewItemMapping[run].Count == 0) {
          DeregisterItemEvents(run);
          itemListViewItemMapping.Remove(run);
        }
      }
      listViewItem.Remove();
    }

    private void UpdateListViewItemText(ListViewItem listViewItem) {
      if (listViewItem == null) throw new ArgumentNullException();
      IRun item = listViewItem.Tag as IRun;
      listViewItem.Text = item == null ? "null" : item.ToString();
      listViewItem.ToolTipText = item == null ? string.Empty : item.ItemName + ": " + item.ItemDescription;
    }
    private IEnumerable<ListViewItem> GetListViewItemsForItem(IRun item) {
      if (item == null) {
        List<ListViewItem> listViewItems = new List<ListViewItem>();
        foreach (ListViewItem listViewItem in itemsListView.Items) {
          if (listViewItem.Tag == null) listViewItems.Add(listViewItem);
        }
        return listViewItems;
      } else {
        List<ListViewItem> listViewItems = null;
        itemListViewItemMapping.TryGetValue(item, out listViewItems);
        return listViewItems == null ? Enumerable.Empty<ListViewItem>() : listViewItems;
      }
    }

    private void UpdateGroupBoxText() {
      if (Content == null || Content.Count == 0) itemsGroupBox.Text = "Runs";
      else itemsGroupBox.Text = @"Runs (" + Content.Count + @")";
    }

    #region ListView Events
    private void itemsListView_SelectedIndexChanged(object sender, EventArgs e) {
      removeButton.Enabled = itemsListView.SelectedItems.Count > 0 && (Content != null) && !Content.IsReadOnly && !ReadOnly;
      // for performance reason (multiple selection fires this handler for every selected item)
      if (itemsListView.SelectedIndices.Count <= 1)
        AdjustListViewColumnSizes();
      if (showDetailsCheckBox.Checked) {
        if (itemsListView.SelectedItems.Count == 1) {
          IRun item = (IRun)itemsListView.SelectedItems[0].Tag;
          detailsGroupBox.Enabled = true;
          viewHost.Content = item;
        } else {
          viewHost.Content = null;
          detailsGroupBox.Enabled = false;
        }
      }
    }
    private void itemsListView_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Delete) {
        if ((itemsListView.SelectedItems.Count > 0) && !Content.IsReadOnly && !ReadOnly) {
          if (RunCollection != null) {
            RunCollection.RemoveRange(itemsListView.SelectedItems.Cast<ListViewItem>().Select(i => (IRun)i.Tag));
          } else {
            foreach (ListViewItem item in itemsListView.SelectedItems)
              Content.Remove((IRun)item.Tag);
          }
        }
      }
    }
    private void itemsListView_DoubleClick(object sender, EventArgs e) {
      if (itemsListView.SelectedItems.Count == 1) {
        IRun item = itemsListView.SelectedItems[0].Tag as IRun;
        if (item != null) {
          IContentView view = MainFormManager.MainForm.ShowContent(item);
          if (view != null) {
            view.ReadOnly = ReadOnly;
            view.Locked = Locked;
          }
        }
      }
    }
    private void itemsListView_ItemDrag(object sender, ItemDragEventArgs e) {
      if (!Locked) {
        List<IRun> items = new List<IRun>();
        foreach (ListViewItem listViewItem in itemsListView.SelectedItems) {
          IRun item = listViewItem.Tag as IRun;
          if (item != null) items.Add(item);
        }

        if (items.Count > 0) {
          DataObject data = new DataObject();
          if (items.Count == 1) data.SetData(HeuristicLab.Common.Constants.DragDropDataFormat, items[0]);
          else data.SetData(HeuristicLab.Common.Constants.DragDropDataFormat, items);
          if (Content.IsReadOnly || ReadOnly) {
            DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Link);
          } else {
            DragDropEffects result = DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Link | DragDropEffects.Move);
            if (result.HasFlag(DragDropEffects.Move)) {
              foreach (IRun item in items) Content.Remove(item);
            }
          }
        }
      }
    }
    private void itemsListView_DragEnter(object sender, DragEventArgs e) {
      validDragOperation = false;
      if (!Content.IsReadOnly && !ReadOnly && (e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) is IRun)) {
        validDragOperation = true;
      } else if (!Content.IsReadOnly && !ReadOnly && (e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) is IEnumerable)) {
        validDragOperation = true;
        IEnumerable items = (IEnumerable)e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
        foreach (object item in items)
          validDragOperation = validDragOperation && (item is IRun);
      }
    }
    private void itemsListView_DragOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (validDragOperation) {
        if ((e.KeyState & 32) == 32) e.Effect = DragDropEffects.Link;  // ALT key
        else if ((e.KeyState & 4) == 4) e.Effect = DragDropEffects.Move;  // SHIFT key
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Copy)) e.Effect = DragDropEffects.Copy;
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Move)) e.Effect = DragDropEffects.Move;
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Link)) e.Effect = DragDropEffects.Link;
      }
    }
    private void itemsListView_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        if (e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) is IRun) {
          IRun item = (IRun)e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
          Content.Add(e.Effect.HasFlag(DragDropEffects.Copy) ? (IRun)item.Clone() : item);
        } else if (e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) is IEnumerable) {
          IEnumerable<IRun> items = ((IEnumerable)e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat)).Cast<IRun>();
          if (e.Effect.HasFlag(DragDropEffects.Copy)) {
            Cloner cloner = new Cloner();
            items = items.Select(x => cloner.Clone(x));
          }
          if (RunCollection != null) {
            RunCollection.AddRange(items);
          } else { // the content is an IItemCollection<IRun>
            foreach (IRun item in items)
              Content.Add(item);
          }
        }
      }
    }
    #endregion

    #region Button Events
    private void menuItem_Click(object sender, EventArgs e) {
      ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
      Type viewType = (Type)menuItem.Tag;
      IContentView view = MainFormManager.MainForm.ShowContent(Content, viewType);
      if (view != null) {
        view.Locked = Locked;
        view.ReadOnly = ReadOnly;
      }
    }
    private void removeButton_Click(object sender, EventArgs e) {
      if (itemsListView.SelectedItems.Count > 0) {
        if (RunCollection != null) {
          RunCollection.RemoveRange(itemsListView.SelectedItems.Cast<ListViewItem>().Select(i => (IRun)i.Tag));
        } else {
          foreach (ListViewItem item in itemsListView.SelectedItems)
            Content.Remove((IRun)item.Tag);
        }
        itemsListView.SelectedItems.Clear();
      }
    }
    private void clearButton_Click(object sender, EventArgs e) {
      Content.Clear();
    }
    #endregion

    #region Control Events
    private void showDetailsCheckBox_CheckedChanged(object sender, EventArgs e) {
      if (showDetailsCheckBox.Checked) {
        splitContainer.Panel2Collapsed = false;
        detailsGroupBox.Enabled = itemsListView.SelectedItems.Count == 1;
        viewHost.Content = itemsListView.SelectedItems.Count == 1 ? (IRun)itemsListView.SelectedItems[0].Tag : null;
      } else {
        splitContainer.Panel2Collapsed = true;
        viewHost.Content = null;
      }
    }
    private void EvaluateModifications() {
      if (RunCollection == null)
        return;
      ReadOnly = true;

      try {
        RunCollection.UpdateOfRunsInProgress = true;
        RunCollection.Modify();
      }
      finally {
        ReadOnly = false;
        RunCollection.UpdateOfRunsInProgress = false;
      }
    }
    #endregion

    #region Content Events
    private void Content_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      if (suppressUpdates) return;
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IRun>(Content_ItemsAdded), sender, e);
      else {
        var items = e.Items.Select(CreateListViewItem).ToArray();
        itemsListView.Items.AddRange(items);
        AdjustListViewColumnSizes();
        analyzeRunsToolStripDropDownButton.Enabled = itemsListView.Items.Count > 0;
        clearButton.Enabled = itemsListView.Items.Count > 0 && !Content.IsReadOnly && !ReadOnly;
        runCollectionConstraintCollectionView.ReadOnly = itemsListView.Items.Count == 0;
        UpdateGroupBoxText();
      }
    }
    private void Content_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      if (suppressUpdates) return;
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IRun>(Content_ItemsRemoved), sender, e);
      else {
        foreach (IRun item in e.Items) {
          //remove only the first matching ListViewItem, because the IRun could be contained multiple times in the ItemCollection
          ListViewItem listViewItem = GetListViewItemsForItem(item).FirstOrDefault();
          if (listViewItem != null) RemoveListViewItem(listViewItem);
        }
        analyzeRunsToolStripDropDownButton.Enabled = itemsListView.Items.Count > 0;
        clearButton.Enabled = itemsListView.Items.Count > 0 && !Content.IsReadOnly && !ReadOnly;
        runCollectionConstraintCollectionView.ReadOnly = itemsListView.Items.Count == 0;
        UpdateGroupBoxText();
      }
    }
    private void Content_CollectionReset(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      if (suppressUpdates) return;
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IRun>(Content_CollectionReset), sender, e);
      else {
        foreach (IRun item in e.OldItems) {
          //remove only the first matching ListViewItem, because the IRun could be contained multiple times in the ItemCollection
          ListViewItem listViewItem = GetListViewItemsForItem(item).FirstOrDefault();
          if (listViewItem != null) RemoveListViewItem(listViewItem);
        }
        var items = e.Items.Select(CreateListViewItem).ToArray();
        itemsListView.Items.AddRange(items);

        AdjustListViewColumnSizes();
        analyzeRunsToolStripDropDownButton.Enabled = itemsListView.Items.Count > 0;
        clearButton.Enabled = itemsListView.Items.Count > 0 && !Content.IsReadOnly && !ReadOnly;
        runCollectionConstraintCollectionView.ReadOnly = itemsListView.Items.Count == 0;
        UpdateGroupBoxText();
      }
    }
    private void RunCollection_UpdateOfRunsInProgressChanged(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((Action<object, EventArgs>)RunCollection_UpdateOfRunsInProgressChanged, sender, e);
      else {
        suppressUpdates = RunCollection.UpdateOfRunsInProgress;
        if (!suppressUpdates) {
          foreach (IRun run in Content) {
            DeregisterItemEvents(run);
          }
          itemsListView.Items.Clear();
          itemListViewItemMapping.Clear();
          var items = Content.Select(CreateListViewItem).ToArray();
          itemsListView.Items.AddRange(items);

          AdjustListViewColumnSizes();
          analyzeRunsToolStripDropDownButton.Enabled = itemsListView.Items.Count > 0;
          clearButton.Enabled = itemsListView.Items.Count > 0 && !Content.IsReadOnly && !ReadOnly;
          runCollectionConstraintCollectionView.ReadOnly = itemsListView.Items.Count == 0;
          UpdateGroupBoxText();
        }
      }
    }
    #endregion

    #region Item Events
    private void Item_ToStringChanged(object sender, EventArgs e) {
      if (suppressUpdates) return;
      if (InvokeRequired)
        Invoke(new EventHandler(Item_ToStringChanged), sender, e);
      else {
        IRun item = (IRun)sender;
        foreach (ListViewItem listViewItem in GetListViewItemsForItem(item))
          UpdateListViewItemText(listViewItem);
        AdjustListViewColumnSizes();
      }
    }
    private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      if (suppressUpdates) return;
      if (InvokeRequired)
        Invoke((Action<object, PropertyChangedEventArgs>)Item_PropertyChanged, sender, e);
      else {
        IRun run = (IRun)sender;
        if (e.PropertyName == "Color" || e.PropertyName == "Visible")
          UpdateRun(run);
      }
    }

    private void UpdateRun(IRun run) {
      foreach (ListViewItem listViewItem in GetListViewItemsForItem(run)) {
        if (run.Visible) {
          listViewItem.Font = new Font(listViewItem.Font, FontStyle.Regular);
          listViewItem.ForeColor = run.Color;
        } else {
          listViewItem.Font = new Font(listViewItem.Font, FontStyle.Italic);
          listViewItem.ForeColor = Color.LightGray;
        }
      }
    }
    #endregion

    #region Helpers
    private void AdjustListViewColumnSizes() {
      if (itemsListView.Items.Count > 0) {
        for (int i = 0; i < itemsListView.Columns.Count; i++) {
          itemsListView.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }
      }
    }
    #endregion
  }
}