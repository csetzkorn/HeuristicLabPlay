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

namespace HeuristicLab.Parameters.Views {
  partial class ValueParameterView<T> {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.valueGroupBox = new System.Windows.Forms.GroupBox();
      this.showInRunCheckBox = new System.Windows.Forms.CheckBox();
      this.valueViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.clearValueButton = new System.Windows.Forms.Button();
      this.setValueButton = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.valueGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // dataTypeLabel
      // 
      this.dataTypeLabel.Location = new System.Drawing.Point(3, 29);
      this.dataTypeLabel.TabIndex = 3;
      // 
      // dataTypeTextBox
      // 
      this.dataTypeTextBox.Location = new System.Drawing.Point(69, 26);
      this.dataTypeTextBox.Size = new System.Drawing.Size(317, 20);
      this.dataTypeTextBox.TabIndex = 4;
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Location = new System.Drawing.Point(69, 0);
      this.nameTextBox.Size = new System.Drawing.Size(292, 20);
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(367, 3);
      // 
      // valueGroupBox
      // 
      this.valueGroupBox.AllowDrop = true;
      this.valueGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.valueGroupBox.Controls.Add(this.showInRunCheckBox);
      this.valueGroupBox.Controls.Add(this.valueViewHost);
      this.valueGroupBox.Controls.Add(this.clearValueButton);
      this.valueGroupBox.Controls.Add(this.setValueButton);
      this.valueGroupBox.Location = new System.Drawing.Point(0, 52);
      this.valueGroupBox.Name = "valueGroupBox";
      this.valueGroupBox.Size = new System.Drawing.Size(386, 263);
      this.valueGroupBox.TabIndex = 5;
      this.valueGroupBox.TabStop = false;
      this.valueGroupBox.Text = "Value";
      this.valueGroupBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.valueGroupBox_DragDrop);
      this.valueGroupBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.valueGroupBox_DragEnterOver);
      this.valueGroupBox.DragOver += new System.Windows.Forms.DragEventHandler(this.valueGroupBox_DragEnterOver);
      // 
      // showInRunCheckBox
      // 
      this.showInRunCheckBox.AutoSize = true;
      this.showInRunCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.showInRunCheckBox.Checked = true;
      this.showInRunCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.showInRunCheckBox.Location = new System.Drawing.Point(66, 24);
      this.showInRunCheckBox.Name = "showInRunCheckBox";
      this.showInRunCheckBox.Size = new System.Drawing.Size(90, 17);
      this.showInRunCheckBox.TabIndex = 2;
      this.showInRunCheckBox.Text = "&Show in Run:";
      this.toolTip.SetToolTip(this.showInRunCheckBox, "Check to show the value of this parameter in each run.");
      this.showInRunCheckBox.UseVisualStyleBackColor = true;
      this.showInRunCheckBox.CheckedChanged += new System.EventHandler(this.showInRunCheckBox_CheckedChanged);
      // 
      // valueViewHost
      // 
      this.valueViewHost.AllowDrop = true;
      this.valueViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.valueViewHost.Caption = "View";
      this.valueViewHost.Content = null;
      this.valueViewHost.Enabled = false;
      this.valueViewHost.Location = new System.Drawing.Point(6, 49);
      this.valueViewHost.Name = "valueViewHost";
      this.valueViewHost.ReadOnly = false;
      this.valueViewHost.Size = new System.Drawing.Size(374, 208);
      this.valueViewHost.TabIndex = 3;
      this.valueViewHost.ViewsLabelVisible = true;
      this.valueViewHost.ViewType = null;
      // 
      // clearValueButton
      // 
      this.clearValueButton.Enabled = false;
      this.clearValueButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Remove;
      this.clearValueButton.Location = new System.Drawing.Point(36, 19);
      this.clearValueButton.Name = "clearValueButton";
      this.clearValueButton.Size = new System.Drawing.Size(24, 24);
      this.clearValueButton.TabIndex = 1;
      this.toolTip.SetToolTip(this.clearValueButton, "Clear Value");
      this.clearValueButton.UseVisualStyleBackColor = true;
      this.clearValueButton.Click += new System.EventHandler(this.clearValueButton_Click);
      // 
      // setValueButton
      // 
      this.setValueButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Edit;
      this.setValueButton.Location = new System.Drawing.Point(6, 19);
      this.setValueButton.Name = "setValueButton";
      this.setValueButton.Size = new System.Drawing.Size(24, 24);
      this.setValueButton.TabIndex = 0;
      this.toolTip.SetToolTip(this.setValueButton, "Set Value");
      this.setValueButton.UseVisualStyleBackColor = true;
      this.setValueButton.Click += new System.EventHandler(this.setValueButton_Click);
      // 
      // ValueParameterView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.valueGroupBox);
      this.Name = "ValueParameterView";
      this.Size = new System.Drawing.Size(386, 315);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.dataTypeTextBox, 0);
      this.Controls.SetChildIndex(this.dataTypeLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.valueGroupBox, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.valueGroupBox.ResumeLayout(false);
      this.valueGroupBox.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.GroupBox valueGroupBox;
    protected HeuristicLab.MainForm.WindowsForms.ViewHost valueViewHost;
    protected System.Windows.Forms.Button setValueButton;
    protected System.Windows.Forms.Button clearValueButton;
    protected System.Windows.Forms.CheckBox showInRunCheckBox;
  }
}
