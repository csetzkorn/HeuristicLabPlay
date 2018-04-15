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

namespace HeuristicLab.Optimization.Views {
  partial class RunCollectionComparisonConstraintView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      this.txtConstraintData = new System.Windows.Forms.TextBox();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // txtConstraintData
      // 
      this.txtConstraintData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.errorProvider.SetIconAlignment(this.txtConstraintData, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.txtConstraintData, 2);
      this.txtConstraintData.Location = new System.Drawing.Point(127, 56);
      this.txtConstraintData.Name = "txtConstraintData";
      this.txtConstraintData.Size = new System.Drawing.Size(246, 20);
      this.txtConstraintData.TabIndex = 9;
      this.txtConstraintData.Validated += new System.EventHandler(this.txtConstraintData_Validated);
      this.txtConstraintData.Validating += new System.ComponentModel.CancelEventHandler(this.txtConstraintData_Validating);
      // 
      // errorProvider
      // 
      this.errorProvider.ContainerControl = this;
      // 
      // RunCollectionComparisonConstraintView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.txtConstraintData);
      this.errorProvider.SetIconAlignment(this, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.Name = "RunCollectionComparisonConstraintView";
      this.Controls.SetChildIndex(this.txtConstraintData, 0);
      this.Controls.SetChildIndex(this.cmbConstraintOperation, 0);
      this.Controls.SetChildIndex(this.cmbConstraintColumn, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox txtConstraintData;
    private System.Windows.Forms.ErrorProvider errorProvider;
  }
}
