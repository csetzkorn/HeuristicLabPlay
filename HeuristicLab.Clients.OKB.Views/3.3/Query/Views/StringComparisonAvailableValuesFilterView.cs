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

using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Clients.OKB.Query {
  [View("StringComparisonAvailableValuesFilter View")]
  [Content(typeof(StringComparisonAvailableValuesFilter), true)]
  public partial class StringComparisonAvailableValuesFilterView : FilterView {
    public new StringComparisonAvailableValuesFilter Content {
      get { return (StringComparisonAvailableValuesFilter)base.Content; }
      set { base.Content = value; }
    }

    public StringComparisonAvailableValuesFilterView() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      comparisonComboBox.SelectedIndex = -1;
      if (Content != null) {
        if (Content.Comparison == StringComparison.Equal)
          comparisonComboBox.SelectedItem = "=";
        else if (Content.Comparison == StringComparison.NotEqual)
          comparisonComboBox.SelectedItem = "<>";
        else if (Content.Comparison == StringComparison.Contains)
          comparisonComboBox.SelectedItem = "contains";
        else if (Content.Comparison == StringComparison.NotContains)
          comparisonComboBox.SelectedItem = "not contains";
        else if (Content.Comparison == StringComparison.Like)
          comparisonComboBox.SelectedItem = "like";
        else if (Content.Comparison == StringComparison.NotLike)
          comparisonComboBox.SelectedItem = "not like";
      }
      valueComboBox.DataSource = Content == null ? null : Content.AvailableValues;
      valueComboBox.Text = Content == null ? string.Empty : Content.Value;
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      comparisonComboBox.Enabled = Content != null && !ReadOnly;
      valueComboBox.Enabled = Content != null && !ReadOnly;
    }

    private void comparisonComboBox_SelectedIndexChanged(object sender, System.EventArgs e) {
      if (Content != null) {
        if (comparisonComboBox.SelectedItem.ToString() == "=")
          Content.Comparison = StringComparison.Equal;
        else if (comparisonComboBox.SelectedItem.ToString() == "<>")
          Content.Comparison = StringComparison.NotEqual;
        else if (comparisonComboBox.SelectedItem.ToString() == "contains")
          Content.Comparison = StringComparison.Contains;
        else if (comparisonComboBox.SelectedItem.ToString() == "not contains")
          Content.Comparison = StringComparison.NotContains;
        else if (comparisonComboBox.SelectedItem.ToString() == "like")
          Content.Comparison = StringComparison.Like;
        else if (comparisonComboBox.SelectedItem.ToString() == "not like")
          Content.Comparison = StringComparison.NotLike;
      }
    }

    private void valueTextBox_KeyDown(object sender, KeyEventArgs e) {
      if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return))
        label.Focus();  // set focus on label to validate data
      if (e.KeyCode == Keys.Escape) {
        valueComboBox.Text = Content.Value;
        label.Focus();  // set focus on label to validate data
      }
    }
    private void valueComboBox_Validated(object sender, System.EventArgs e) {
      if (Content != null) {
        Content.Value = valueComboBox.Text;
      }
    }
  }
}
