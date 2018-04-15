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
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common;
using WeifenLuo.WinFormsUI.Docking;

namespace HeuristicLab.MainForm.WindowsForms {
  /// <summary>
  /// Displays the used view.
  /// </summary>
  internal partial class DockForm : DockContent {
    public DockForm(IView view, bool allowContextMenu) {
      InitializeComponent();
      this.view = view;
      this.allowContextMenu = allowContextMenu;

      if (view != null) {
        if (view is UserControl) {
          switch (((UserControl)view).Dock) {
            case DockStyle.Left:
              this.ShowHint = DockState.DockLeft;
              break;
            case DockStyle.Right:
              this.ShowHint = DockState.DockRight;
              break;
            case DockStyle.Top:
              this.ShowHint = DockState.DockTop;
              break;
            case DockStyle.Bottom:
              this.ShowHint = DockState.DockBottom;
              break;
          }
          Sidebar sidebar = view as Sidebar;
          if (sidebar != null) {
            if (sidebar.Collapsed)
              this.ShowHint = DockState.DockLeftAutoHide;
            else
              this.ShowHint = DockState.DockLeft;
            this.DockAreas = DockAreas.DockLeft | DockAreas.DockRight;
          }

          Type viewType = view.GetType();
          Control control = (Control)view;
          control.Dock = DockStyle.Fill;
          this.view.CaptionChanged += new EventHandler(View_CaptionChanged);
          UpdateText();
          viewPanel.Controls.Add(control);
        }
      } else {
        Label errorLabel = new Label();
        errorLabel.Name = "errorLabel";
        errorLabel.Text = "No view available";
        errorLabel.AutoSize = false;
        errorLabel.Dock = DockStyle.Fill;
        viewPanel.Controls.Add(errorLabel);
      }
    }

    protected override void Dispose(bool disposing) {
      if (View != null)
        View.CaptionChanged -= new System.EventHandler(View_CaptionChanged);
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    private IView view;
    public IView View {
      get { return this.view; }
    }
    private readonly bool allowContextMenu;

    private void UpdateText() {
      if (InvokeRequired)
        Invoke(new MethodInvoker(UpdateText));
      else
        this.Text = this.View.Caption;
    }

    protected override void OnDockStateChanged(EventArgs e) {
      base.OnDockStateChanged(e);
      Sidebar sidebar = view as Sidebar;
      if (sidebar != null) {
        if (this.DockState == DockState.DockLeftAutoHide || this.DockState == DockState.DockRightAutoHide)
          sidebar.Collapsed = true;
        else
          sidebar.Collapsed = false;
      }
    }

    #region View Events
    private void View_CaptionChanged(object sender, EventArgs e) {
      UpdateText();
    }
    #endregion

    #region Context Menu Events
    private void contextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e) {
      if (!allowContextMenu) {
        e.Cancel = true;
        return;
      }

      var contentView = View as IContentView;
      var content = contentView != null ? contentView.Content : null;

      cloneToolStripMenuItem.Enabled = contentView != null && !contentView.Locked && content is IDeepCloneable;
    }

    private void closeToolStripMenuItem_Click(object sender, EventArgs e) {
      Close();
    }
    private void closeAllToolStripMenuItem_Click(object sender, EventArgs e) {
      foreach (var dockForm in CurrentDockForms) {
        dockForm.Close();
      }
    }
    private void closeAllButThisToolStripMenuItem_Click(object sender, EventArgs e) {
      foreach (var dockForm in CurrentDockForms.Except(this.ToEnumerable())) {
        dockForm.Close();
      }
    }
    private IEnumerable<DockForm> CurrentDockForms {
      get {
        var dockForms = Pane.Contents.OfType<DockForm>().Where(c => c.Pane == Pane); // Pane.Contents contains DockForms that are not placed on that pane
        return dockForms.ToList(); // .ToList() necessary because closing a DockForm removes it from the Content collection
      }
    }

    private void cloneToolStripMenuItem_Click(object sender, EventArgs e) {
      var contentView = View as IContentView;
      if (contentView == null) return;

      var cloneable = contentView.Content as IDeepCloneable;
      if (cloneable == null) return;

      var clone = (IContent)cloneable.Clone();

      var viewHost = contentView as ViewHost;
      var newView = viewHost != null ? viewHost.ViewType : contentView.GetType();
      MainFormManager.MainForm.ShowContent(clone, newView);
    }
    #endregion
  }
}
