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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Optimizer {
  public partial class CreateExperimentDialog : Form {
    private enum DialogMode { Normal = 1, DiscoveringInstances = 2, CreatingExperiment = 3, PreparingExperiment = 4 };

    private IOptimizer optimizer;
    public IOptimizer Optimizer {
      get { return optimizer; }
      set {
        optimizer = value;
        Experiment = null;
        okButton.Enabled = optimizer != null;
        SetTabControlVisibility();
        FillInstanceTreeViewAsync();
        FillParametersListView();
      }
    }

    public Experiment Experiment { get; private set; }

    private bool createBatchRun;
    private int repetitions;
    private readonly Dictionary<IProblemInstanceProvider, HashSet<IDataDescriptor>> instances;
    private readonly Dictionary<IValueParameter, IntArray> intParameters;
    private readonly Dictionary<IValueParameter, DoubleArray> doubleParameters;
    private readonly HashSet<IValueParameter> boolParameters;
    private readonly Dictionary<IValueParameter, HashSet<IItem>> multipleChoiceParameters;
    private readonly IItem optionalNullChoice = new BoolValue(); // any item will do

    private StringBuilder failedInstances;
    private bool suppressTreeViewEventHandling, suppressCheckAllNoneEventHandling;

    public CreateExperimentDialog() : this(null) { }
    public CreateExperimentDialog(IOptimizer optimizer) {
      InitializeComponent();
      instanceDiscoveryProgressLabel.BackColor = instancesTabPage.BackColor;
      createBatchRun = createBatchRunCheckBox.Checked;
      repetitions = (int)repetitionsNumericUpDown.Value;
      // do not set the Optimizer property here, because we want to delay instance discovery to the time when the form loads
      this.optimizer = optimizer;
      Experiment = null;
      okButton.Enabled = optimizer != null;

      instances = new Dictionary<IProblemInstanceProvider, HashSet<IDataDescriptor>>();
      intParameters = new Dictionary<IValueParameter, IntArray>();
      doubleParameters = new Dictionary<IValueParameter, DoubleArray>();
      boolParameters = new HashSet<IValueParameter>();
      multipleChoiceParameters = new Dictionary<IValueParameter, HashSet<IItem>>();
    }

    #region Event handlers
    private void CreateExperimentDialog_Load(object sender, EventArgs e) {
      SetTabControlVisibility();
      FillInstanceTreeViewAsync();
      FillParametersListView();
    }

    private void CreateExperimentDialog_FormClosing(object sender, FormClosingEventArgs e) {
      if (experimentCreationBackgroundWorker.IsBusy) {
        if (DialogResult != System.Windows.Forms.DialogResult.OK) {
          if (experimentCreationBackgroundWorker.IsBusy) experimentCreationBackgroundWorker.CancelAsync();
          if (instanceDiscoveryBackgroundWorker.IsBusy) instanceDiscoveryBackgroundWorker.CancelAsync();
        }
        e.Cancel = true;
      }
    }

    private void okButton_Click(object sender, EventArgs e) {
      SetMode(DialogMode.CreatingExperiment);
      experimentCreationBackgroundWorker.RunWorkerAsync();
    }

    #region Parameters variation
    private void parametersListView_ItemChecked(object sender, ItemCheckedEventArgs e) {
      var parameter = (IValueParameter)e.Item.Tag;
      var isConstrainedValueParameter = typeof(OptionalConstrainedValueParameter<>).Equals(parameter.GetType().GetGenericTypeDefinition())
        || typeof(ConstrainedValueParameter<>).Equals(parameter.GetType().GetGenericTypeDefinition());

      if (!isConstrainedValueParameter && parameter.Value == null) {
        if (e.Item.Checked) e.Item.Checked = false;
        return;
      }

      if (isConstrainedValueParameter) {
        if (e.Item.Checked) multipleChoiceParameters.Add(parameter, new HashSet<IItem>());
        else multipleChoiceParameters.Remove(parameter);
      } else {

        var intValue = parameter.Value as ValueTypeValue<int>;
        if (intValue != null) {
          if (e.Item.Checked) {
            IntArray initialValues;
            if (intValue.Value == int.MinValue)
              initialValues = new IntArray(new int[] { -100, -50, 5 });
            else if (intValue.Value == int.MaxValue)
              initialValues = new IntArray(new int[] { 5, 50, 100 });
            else if (intValue.Value == 0)
              initialValues = new IntArray(new int[] { 0, 1, 2 });
            else if (Math.Abs(intValue.Value) < 10)
              initialValues = new IntArray(new int[] { intValue.Value - 1, intValue.Value, intValue.Value + 1 });
            else initialValues = new IntArray(new int[] { intValue.Value / 2, intValue.Value, intValue.Value * 2 });
            intParameters.Add(parameter, initialValues);
            intParameters[parameter].Reset += new EventHandler(ValuesArray_Reset);
          } else intParameters.Remove(parameter);
        }

        var doubleValue = parameter.Value as ValueTypeValue<double>;
        if (doubleValue != null) {
          if (e.Item.Checked) {
            DoubleArray initialValues;
            if (doubleValue.Value == double.MinValue)
              initialValues = new DoubleArray(new double[] { -1, -0.5, 0 });
            else if (doubleValue.Value == double.MaxValue)
              initialValues = new DoubleArray(new double[] { 0, 0.5, 1 });
            else if (doubleValue.Value == 0.0)
              initialValues = new DoubleArray(new double[] { 0, 0.1, 0.2 });
            else if (Math.Abs(doubleValue.Value) <= 1.0) {
              if (doubleValue.Value > 0.9 || (doubleValue.Value < 0.0 && doubleValue.Value > -0.1))
                initialValues = new DoubleArray(new double[] { doubleValue.Value - 0.2, doubleValue.Value - 0.1, doubleValue.Value });
              else if (doubleValue.Value < -0.9 || (doubleValue.Value > 0 && doubleValue.Value < 0.1))
                initialValues = new DoubleArray(new double[] { doubleValue.Value, doubleValue.Value + 0.1, doubleValue.Value + 0.2 });
              else initialValues = new DoubleArray(new double[] { doubleValue.Value - 0.1, doubleValue.Value, doubleValue.Value + 0.1 });
            } else initialValues = new DoubleArray(new double[] { doubleValue.Value / 2.0, doubleValue.Value, doubleValue.Value * 2.0 });
            doubleParameters.Add(parameter, initialValues);
            doubleParameters[parameter].Reset += new EventHandler(ValuesArray_Reset);
          } else doubleParameters.Remove(parameter);
        }

        var boolValue = parameter.Value as ValueTypeValue<bool>;
        if (boolValue != null) {
          if (e.Item.Checked) boolParameters.Add(parameter);
          else boolParameters.Remove(parameter);
        }

        bool isEnumValue = parameter.Value != null && parameter.Value.GetType().IsGenericType
                           && typeof(EnumValue<>).IsAssignableFrom(parameter.Value.GetType().GetGenericTypeDefinition());
        if (isEnumValue) {
          if (e.Item.Checked) multipleChoiceParameters.Add(parameter, new HashSet<IItem>(new EnumValueEqualityComparer()));
          else multipleChoiceParameters.Remove(parameter);
        }
      }

      UpdateVariationsLabel();
      if (e.Item.Selected) UpdateDetailsView(parameter);
      else e.Item.Selected = true;
    }

    private void parametersListView_SelectedIndexChanged(object sender, EventArgs e) {
      if (parametersListView.SelectedItems.Count == 0) {
        ClearDetailsView();
      } else {
        var parameter = parametersListView.SelectedItems[0].Tag as IValueParameter;
        UpdateDetailsView(parameter);
      }
    }

    private void UpdateDetailsView(IValueParameter parameter) {
      ClearDetailsView();

      var isOptionalConstrainedValueParameter = typeof(OptionalConstrainedValueParameter<>).IsAssignableFrom(parameter.GetType().GetGenericTypeDefinition());
      var isConstrainedValueParameter =
        isOptionalConstrainedValueParameter
        || typeof(ConstrainedValueParameter<>).Equals(parameter.GetType().GetGenericTypeDefinition());
      bool isEnumValue = parameter.Value != null && parameter.Value.GetType().IsGenericType
                         && typeof(EnumValue<>).IsAssignableFrom(parameter.Value.GetType().GetGenericTypeDefinition());

      if (isConstrainedValueParameter || isEnumValue) {
        detailsTypeLabel.Text = "Choices:";
        choicesListView.Tag = parameter;

        if (isConstrainedValueParameter) {
          if (isOptionalConstrainedValueParameter) {
            choicesListView.Items.Add(new ListViewItem("-") {
              Tag = optionalNullChoice,
              Checked = multipleChoiceParameters.ContainsKey(parameter)
                        && multipleChoiceParameters[parameter].Contains(optionalNullChoice)
            });
          }
          dynamic constrainedValuedParameter = parameter;
          dynamic validValues = constrainedValuedParameter.ValidValues;
          foreach (var choice in validValues) {
            choicesListView.Items.Add(new ListViewItem(choice.ToString()) {
              Tag = choice,
              Checked = multipleChoiceParameters.ContainsKey(parameter)
                        && multipleChoiceParameters[parameter].Contains(choice)
            });
          }
        }
        if (isEnumValue) {
          var enumType = parameter.Value.GetType().GetGenericArguments()[0];
          var enumValueType = typeof(EnumValue<>).MakeGenericType(enumType);
          foreach (var enumChoice in Enum.GetValues(enumType)) {
            choicesListView.Items.Add(new ListViewItem(enumChoice.ToString()) {
              Tag = Activator.CreateInstance(enumValueType, enumChoice),
              Checked = multipleChoiceParameters.ContainsKey(parameter)
                        && multipleChoiceParameters[parameter].Any((dynamic item) => item.Value.Equals(enumChoice))
            });
          }
        }
        choicesListView.Enabled = multipleChoiceParameters.ContainsKey(parameter);
        detailsTypeLabel.Visible = true;
        choicesListView.Visible = true;
        return;
      }

      if (parameter.Value is ValueTypeValue<bool>) {
        detailsTypeLabel.Text = "Boolean parameter: True / False";
        detailsTypeLabel.Visible = true;
      }

      var intValue = parameter.Value as ValueTypeValue<int>;
      if (intValue != null) {
        if (intParameters.ContainsKey(parameter))
          stringConvertibleArrayView.Content = intParameters[parameter];
        stringConvertibleArrayView.Visible = true;
        stringConvertibleArrayView.ReadOnly = !intParameters.ContainsKey(parameter);
        generateButton.Tag = parameter;
        generateButton.Enabled = intParameters.ContainsKey(parameter);
        generateButton.Visible = true;
        return;
      }

      var doubleValue = parameter.Value as ValueTypeValue<double>;
      if (doubleValue != null) {
        if (doubleParameters.ContainsKey(parameter))
          stringConvertibleArrayView.Content = doubleParameters[parameter];
        stringConvertibleArrayView.Visible = true;
        stringConvertibleArrayView.ReadOnly = !doubleParameters.ContainsKey(parameter);
        generateButton.Tag = parameter;
        generateButton.Enabled = doubleParameters.ContainsKey(parameter);
        generateButton.Visible = true;
        return;
      }
    }

    #region Detail controls
    private void choiceListView_ItemChecked(object sender, ItemCheckedEventArgs e) {
      var parameter = (IValueParameter)choicesListView.Tag;
      if (multipleChoiceParameters.ContainsKey(parameter)) {
        if (e.Item.Checked) {
          multipleChoiceParameters[parameter].Add((IItem)e.Item.Tag);
        } else multipleChoiceParameters[parameter].Remove((IItem)e.Item.Tag);

        UpdateVariationsLabel();
      }
    }

    private void generateButton_Click(object sender, EventArgs e) {
      var parameter = (IValueParameter)generateButton.Tag;
      bool integerOnly = intParameters.ContainsKey(parameter);
      decimal min = 0, max = 1, step = 1;
      #region Try to calculate some meaningful values
      if (integerOnly) {
        int len = intParameters[parameter].Length;
        if (len > 0) {
          min = intParameters[parameter].Min();
          max = intParameters[parameter].Max();
          step = len >= 2 ? Math.Abs((intParameters[parameter][len - 1] - intParameters[parameter][len - 2])) : 1;
        }
      } else {
        int len = doubleParameters[parameter].Length;
        if (len > 0) {
          min = (decimal)doubleParameters[parameter].Min();
          max = (decimal)doubleParameters[parameter].Max();
          step = len >= 2 ? Math.Abs(((decimal)doubleParameters[parameter][len - 1] - (decimal)doubleParameters[parameter][len - 2])) : 1m;
        }
      }
      #endregion
      using (var dialog = new DefineArithmeticProgressionDialog(integerOnly, min, max, step)) {
        if (dialog.ShowDialog(this) == DialogResult.OK) {
          var values = dialog.Values;
          if (integerOnly) {
            intParameters[parameter].Reset -= new EventHandler(ValuesArray_Reset);
            intParameters[parameter] = new IntArray(values.Select(x => (int)x).ToArray());
            intParameters[parameter].Reset += new EventHandler(ValuesArray_Reset);
            stringConvertibleArrayView.Content = intParameters[parameter];
          } else {
            doubleParameters[parameter].Reset -= new EventHandler(ValuesArray_Reset);
            doubleParameters[parameter] = new DoubleArray(values.Select(x => (double)x).ToArray());
            doubleParameters[parameter].Reset += new EventHandler(ValuesArray_Reset);
            stringConvertibleArrayView.Content = doubleParameters[parameter];
          }
          UpdateVariationsLabel();
        }
      }
    }

    private void ValuesArray_Reset(object sender, EventArgs e) {
      UpdateVariationsLabel();
    }
    #endregion
    #endregion

    #region Instances
    private void instancesTreeView_AfterCheck(object sender, TreeViewEventArgs e) {
      if (!suppressTreeViewEventHandling) {
        if (e.Node.Nodes.Count > 0) { // provider node was (un)checked
          SyncProviderNode(e.Node);
        } else { // descriptor node was (un)checked
          SyncInstanceNode(e.Node);
        }

        suppressCheckAllNoneEventHandling = true;
        try {
          var treeViewNodes = instancesTreeView.Nodes.OfType<TreeNode>().SelectMany(x => x.Nodes.OfType<TreeNode>());
          selectAllCheckBox.Checked = treeViewNodes.Count() == instances.SelectMany(x => x.Value).Count();
          selectNoneCheckBox.Checked = !treeViewNodes.Any(x => x.Checked);
        }
        finally { suppressCheckAllNoneEventHandling = false; }
        UpdateVariationsLabel();
      }
    }

    private void SyncProviderNode(TreeNode node) {
      suppressTreeViewEventHandling = true;
      try {
        foreach (TreeNode n in node.Nodes) {
          if (n.Checked != node.Checked) {
            n.Checked = node.Checked;
            SyncInstanceNode(n, false);
          }
        }
      }
      finally { suppressTreeViewEventHandling = false; }
    }

    private void SyncInstanceNode(TreeNode node, bool providerCheck = true) {
      var provider = (IProblemInstanceProvider)node.Parent.Tag;
      var descriptor = (IDataDescriptor)node.Tag;
      if (node.Checked) {
        if (!instances.ContainsKey(provider))
          instances.Add(provider, new HashSet<IDataDescriptor>());
        instances[provider].Add(descriptor);
      } else {
        if (instances.ContainsKey(provider)) {
          instances[provider].Remove(descriptor);
          if (instances[provider].Count == 0)
            instances.Remove(provider);
        }
      }
      if (providerCheck) {
        bool allChecked = node.Parent.Nodes.OfType<TreeNode>().All(x => x.Checked);
        suppressTreeViewEventHandling = true;
        try {
          node.Parent.Checked = allChecked;
        }
        finally { suppressTreeViewEventHandling = false; }
      }
    }

    private void selectAllCheckBox_CheckedChanged(object sender, EventArgs e) {
      if (!suppressCheckAllNoneEventHandling) {
        if (selectAllCheckBox.Checked) {
          suppressCheckAllNoneEventHandling = true;
          try { selectNoneCheckBox.Checked = false; }
          finally { suppressCheckAllNoneEventHandling = false; }
          try {
            suppressTreeViewEventHandling = true;
            foreach (TreeNode node in instancesTreeView.Nodes) {
              if (!node.Checked) {
                node.Checked = true;
                SyncProviderNode(node);
              }
            }
          }
          finally { suppressTreeViewEventHandling = false; }
        }
        UpdateVariationsLabel();
      }
    }

    private void selectNoneCheckBox_CheckedChanged(object sender, EventArgs e) {
      if (!suppressCheckAllNoneEventHandling) {
        if (selectNoneCheckBox.Checked) {
          suppressCheckAllNoneEventHandling = true;
          try { selectAllCheckBox.Checked = false; }
          finally { suppressCheckAllNoneEventHandling = false; }
          try {
            suppressTreeViewEventHandling = true;
            foreach (TreeNode node in instancesTreeView.Nodes) {
              if (node.Checked) {
                node.Checked = false;
                SyncProviderNode(node);
              }
            }
          }
          finally { suppressTreeViewEventHandling = false; }
        }
        UpdateVariationsLabel();
      }
    }
    #endregion

    private void createBatchRunCheckBox_CheckedChanged(object sender, EventArgs e) {
      repetitionsNumericUpDown.Enabled = createBatchRunCheckBox.Checked;
      createBatchRun = createBatchRunCheckBox.Checked;
    }

    private void repetitionsNumericUpDown_Validated(object sender, EventArgs e) {
      if (repetitionsNumericUpDown.Text == string.Empty)
        repetitionsNumericUpDown.Text = repetitionsNumericUpDown.Value.ToString();
      repetitions = (int)repetitionsNumericUpDown.Value;
    }

    private void experimentsLabel_TextChanged(object sender, EventArgs e) {
      long number;
      if (long.TryParse(variationsLabel.Text, NumberStyles.AllowThousands, CultureInfo.CurrentCulture.NumberFormat, out number)) {
        if (number > 1000) warningProvider.SetError(variationsLabel, "Consider reducing the number of variations!");
        else warningProvider.SetError(variationsLabel, null);
      }
    }
    #endregion

    #region Helpers
    private void SetTabControlVisibility() {
      bool isAlgorithm = optimizer != null && optimizer is IAlgorithm;
      bool instancesAvailable = isAlgorithm
        && ((IAlgorithm)optimizer).Problem != null
        && ProblemInstanceManager.GetProviders(((IAlgorithm)optimizer).Problem).Any();
      if (instancesAvailable && tabControl.TabCount == 1)
        tabControl.TabPages.Add(instancesTabPage);
      else if (!instancesAvailable && tabControl.TabCount == 2)
        tabControl.TabPages.Remove(instancesTabPage);
      tabControl.Visible = isAlgorithm;
      if (isAlgorithm) {
        variationsLabel.Visible = true;
        experimentsToCreateDescriptionLabel.Visible = true;
        Height = 450;
      } else {
        variationsLabel.Visible = false;
        experimentsToCreateDescriptionLabel.Visible = false;
        Height = 130;
      }
    }

    private void FillParametersListView() {
      parametersListView.Items.Clear();
      intParameters.Clear();
      doubleParameters.Clear();
      boolParameters.Clear();
      multipleChoiceParameters.Clear();

      if (Optimizer is IAlgorithm) {
        var parameters = ((IAlgorithm)optimizer).Parameters;
        foreach (var param in parameters) {
          var valueParam = param as IValueParameter;
          if (valueParam != null && (valueParam.Value is ValueTypeValue<bool>
              || valueParam.Value is ValueTypeValue<int>
              || valueParam.Value is ValueTypeValue<double>
              || (valueParam.Value != null && valueParam.Value.GetType().IsGenericType
                    && typeof(EnumValue<>).IsAssignableFrom(valueParam.Value.GetType().GetGenericTypeDefinition())))
            || typeof(OptionalConstrainedValueParameter<>).IsAssignableFrom(param.GetType().GetGenericTypeDefinition())
            || typeof(ConstrainedValueParameter<>).IsAssignableFrom(param.GetType().GetGenericTypeDefinition()))
            parametersListView.Items.Add(new ListViewItem(param.Name) { Tag = param, ToolTipText = param.Description });
        }
      }
    }

    private void FillInstanceTreeViewAsync() {
      instances.Clear();
      instancesTreeView.Nodes.Clear();

      if (Optimizer is IAlgorithm && ((IAlgorithm)Optimizer).Problem != null) {
        SetMode(DialogMode.DiscoveringInstances);
        instanceDiscoveryBackgroundWorker.RunWorkerAsync();
      }
    }

    private void AddOptimizer(IOptimizer optimizer, Experiment experiment) {
      if (createBatchRun) {
        var batchRun = new BatchRun(repetitions.ToString() + "x " + optimizer.Name) {
          Repetitions = repetitions,
          Optimizer = optimizer
        };
        experiment.Optimizers.Add(batchRun);
      } else {
        experiment.Optimizers.Add(optimizer);
      }
    }

    private int GetNumberOfVariations() {
      int instancesCount = 1;
      if (instances.Values.Any())
        instancesCount = Math.Max(instances.Values.SelectMany(x => x).Count(), 1);

      int intParameterVariations = 1;
      foreach (var intParam in intParameters.Values) {
        intParameterVariations *= Math.Max(intParam.Length, 1);
      }
      int doubleParameterVariations = 1;
      foreach (var doubleParam in doubleParameters.Values) {
        doubleParameterVariations *= Math.Max(doubleParam.Length, 1);
      }
      int boolParameterVariations = 1;
      foreach (var boolParam in boolParameters) {
        boolParameterVariations *= 2;
      }
      int choiceParameterVariations = 1;
      foreach (var choiceParam in multipleChoiceParameters.Values) {
        choiceParameterVariations *= Math.Max(choiceParam.Count, 1);
      }

      return (instancesCount * intParameterVariations * doubleParameterVariations * boolParameterVariations * choiceParameterVariations);
    }

    private void SetMode(DialogMode mode) {
      if (InvokeRequired) Invoke((Action<DialogMode>)SetMode, mode);
      else {
        createBatchRunCheckBox.Enabled = mode == DialogMode.Normal;
        repetitionsNumericUpDown.Enabled = mode == DialogMode.Normal;
        parametersSplitContainer.Enabled = mode == DialogMode.Normal || mode == DialogMode.DiscoveringInstances;
        selectAllCheckBox.Enabled = mode == DialogMode.Normal;
        selectNoneCheckBox.Enabled = mode == DialogMode.Normal;
        instancesTreeView.Enabled = mode == DialogMode.Normal;
        instancesTreeView.Visible = mode == DialogMode.Normal || mode == DialogMode.CreatingExperiment || mode == DialogMode.PreparingExperiment;
        okButton.Enabled = mode == DialogMode.Normal;
        okButton.Visible = mode != DialogMode.CreatingExperiment && mode != DialogMode.PreparingExperiment;
        cancelButton.Enabled = mode != DialogMode.PreparingExperiment;
        instanceDiscoveryProgressLabel.Visible = mode == DialogMode.DiscoveringInstances;
        instanceDiscoveryProgressBar.Visible = mode == DialogMode.DiscoveringInstances;
        experimentCreationProgressBar.Visible = mode == DialogMode.CreatingExperiment || mode == DialogMode.PreparingExperiment;
      }
    }

    private void ClearDetailsView() {
      stringConvertibleArrayView.Visible = false;
      stringConvertibleArrayView.Content = null;
      stringConvertibleArrayView.ReadOnly = true;
      generateButton.Visible = false;
      detailsTypeLabel.Visible = false;
      choicesListView.Items.Clear();
      choicesListView.Enabled = false;
      choicesListView.Visible = false;
    }

    private void UpdateVariationsLabel() {
      variationsLabel.Text = GetNumberOfVariations().ToString("#,#", CultureInfo.CurrentCulture);
    }

    #region Retrieve parameter combinations
    private IEnumerable<Dictionary<IValueParameter, int>> GetIntParameterConfigurations() {
      var configuration = new Dictionary<IValueParameter, int>();
      var enumerators = new Dictionary<IValueParameter, IEnumerator<int>>();
      bool finished;
      do {
        foreach (var p in intParameters) {
          if (!enumerators.ContainsKey(p.Key)) {
            enumerators[p.Key] = p.Value.GetEnumerator();
            enumerators[p.Key].MoveNext();
          }
          configuration[p.Key] = enumerators[p.Key].Current;
        }
        yield return configuration;

        finished = true;
        foreach (var p in intParameters) {
          if (!enumerators[p.Key].MoveNext()) {
            enumerators[p.Key] = p.Value.GetEnumerator();
            enumerators[p.Key].MoveNext();
          } else {
            finished = false;
            break;
          }
        }
      } while (!finished);
    }

    private IEnumerable<Dictionary<IValueParameter, double>> GetDoubleParameterConfigurations() {
      var configuration = new Dictionary<IValueParameter, double>();
      var enumerators = new Dictionary<IValueParameter, IEnumerator<double>>();
      bool finished;
      do {
        foreach (var p in doubleParameters) {
          if (!enumerators.ContainsKey(p.Key)) {
            enumerators[p.Key] = p.Value.GetEnumerator();
            enumerators[p.Key].MoveNext();
          }
          configuration[p.Key] = enumerators[p.Key].Current;
        }
        yield return configuration;

        finished = true;
        foreach (var p in doubleParameters) {
          if (!enumerators[p.Key].MoveNext()) {
            enumerators[p.Key] = p.Value.GetEnumerator();
            enumerators[p.Key].MoveNext();
          } else {
            finished = false;
            break;
          }
        }
      } while (!finished);
    }

    private IEnumerable<Dictionary<IValueParameter, bool>> GetBoolParameterConfigurations() {
      var configuration = new Dictionary<IValueParameter, bool>();
      bool finished;
      do {
        foreach (var p in boolParameters) {
          if (!configuration.ContainsKey(p))
            configuration.Add(p, false);
        }

        yield return configuration;
        finished = true;

        foreach (var p in boolParameters) {
          if (configuration[p]) {
            configuration[p] = false;
          } else {
            configuration[p] = true;
            finished = false;
            break;
          }
        }
      } while (!finished);
    }

    private IEnumerable<Dictionary<IValueParameter, IItem>> GetMultipleChoiceConfigurations() {
      var configuration = new Dictionary<IValueParameter, IItem>();
      var enumerators = new Dictionary<IValueParameter, IEnumerator<IItem>>();
      bool finished;
      do {
        foreach (var p in multipleChoiceParameters.Keys.ToArray()) {
          if (!enumerators.ContainsKey(p)) {
            enumerators.Add(p, multipleChoiceParameters[p].GetEnumerator());
            if (!enumerators[p].MoveNext()) {
              multipleChoiceParameters.Remove(p);
              continue;
            }
          }
          configuration[p] = enumerators[p].Current;
        }

        finished = true;
        foreach (var p in multipleChoiceParameters.Keys) {
          if (!enumerators[p].MoveNext()) {
            enumerators[p] = multipleChoiceParameters[p].GetEnumerator();
            enumerators[p].MoveNext();
          } else {
            finished = false;
            break;
          }
        }
        yield return configuration;
      } while (!finished);
    }
    #endregion

    #region EnumValueEqualityComparer
    private class EnumValueEqualityComparer : EqualityComparer<IItem> {
      public override bool Equals(IItem x, IItem y) {
        if (typeof(EnumValue<>).IsAssignableFrom(x.GetType().GetGenericTypeDefinition()) &&
            typeof(EnumValue<>).IsAssignableFrom(y.GetType().GetGenericTypeDefinition())) {
          dynamic enumValueX = x;
          dynamic enumValueY = y;
          return enumValueX.Value.Equals(enumValueY.Value);
        } else return object.ReferenceEquals(x, y);
      }

      public override int GetHashCode(IItem obj) {
        if (typeof(EnumValue<>).IsAssignableFrom(obj.GetType().GetGenericTypeDefinition())) {
          dynamic enumValue = obj;
          return enumValue.Value.GetHashCode();
        } else return obj.GetHashCode();
      }
    }
    #endregion

    #endregion

    #region Background workers
    #region Instance discovery
    private void instanceDiscoveryBackgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
      instanceDiscoveryBackgroundWorker.ReportProgress(0, "Finding instance providers...");
      var instanceProviders = ProblemInstanceManager.GetProviders(((IAlgorithm)Optimizer).Problem).ToArray();
      var nodes = new List<TreeNode>(instanceProviders.Length);
      for (int i = 0; i < instanceProviders.Length; i++) {
        var provider = instanceProviders[i];
        var providerNode = new TreeNode(provider.Name) { Tag = provider };
        var descriptors = ProblemInstanceManager.GetDataDescriptors(provider);
        foreach (var desc in descriptors) {
          #region Check cancellation request
          if (instanceDiscoveryBackgroundWorker.CancellationPending) {
            e.Cancel = true;
            e.Result = nodes.ToArray();
            return;
          }
          #endregion
          var node = new TreeNode(desc.Name) { Tag = desc };
          providerNode.Nodes.Add(node);
        }

        //Do not add instance providers without data descriptors
        if (providerNode.Nodes.Count > 0) {
          nodes.Add(providerNode);
        }

        double progress = nodes.Count > 0 ? i / (double)instanceProviders.Length : 0.0;
        instanceDiscoveryBackgroundWorker.ReportProgress((int)(100 * progress), provider.Name);
      }
      e.Result = nodes.ToArray();
      instanceDiscoveryBackgroundWorker.ReportProgress(100, string.Empty);
    }

    private void instanceDiscoveryBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
      if (instanceDiscoveryProgressBar.Value != e.ProgressPercentage)
        instanceDiscoveryProgressBar.Value = e.ProgressPercentage;
      instanceDiscoveryProgressLabel.Text = (string)e.UserState;
    }

    private void instanceDiscoveryBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      try {
        if (((TreeNode[])e.Result).Length > 0) {
          instancesTreeView.Nodes.AddRange((TreeNode[])e.Result);
          foreach (TreeNode node in instancesTreeView.Nodes)
            node.Collapse();
        }
        selectNoneCheckBox.Checked = true;
      }
      catch { }
      try {
        SetMode(DialogMode.Normal);
        if (e.Error != null) MessageBox.Show(e.Error.Message, "Error occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      catch { }
    }
    #endregion

    #region Experiment creation
    private void experimentCreationBackgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
      failedInstances = new StringBuilder();
      var localExperiment = new Experiment();

      int counter = 0, totalVariations = GetNumberOfVariations();
      int totalInstances = instances.Values.SelectMany(x => x).Count();
      if (totalInstances == 0) {
        try {
          AddParameterVariations(Optimizer, localExperiment, ref counter, totalVariations);
        }
        catch (OperationCanceledException) {
          e.Cancel = true;
          return;
        }
        experimentCreationBackgroundWorker.ReportProgress(100, string.Empty);
      } else {
        foreach (var provider in instances.Keys) {
          foreach (var descriptor in instances[provider]) {
            var algorithm = (IAlgorithm)Optimizer.Clone();
            bool failed = false;
            try {
              ProblemInstanceManager.LoadData(provider, descriptor, (IProblemInstanceConsumer)algorithm.Problem);
            }
            catch (Exception ex) {
              failedInstances.AppendLine(descriptor.Name + ": " + ex.Message);
              failed = true;
            }
            if (!failed) {
              try {
                if (totalInstances > 1 && totalVariations / totalInstances > 1) {
                  var experiment = new Experiment(descriptor.Name);
                  AddParameterVariations(algorithm, experiment, ref counter, totalVariations);
                  localExperiment.Optimizers.Add(experiment);
                } else {
                  AddParameterVariations(algorithm, localExperiment, ref counter, totalVariations);
                }
              }
              catch (OperationCanceledException) {
                e.Cancel = true;
                return;
              }
            } else experimentCreationBackgroundWorker.ReportProgress((int)Math.Round((100.0 * counter) / totalVariations), "Loading failed (" + descriptor.Name + ")");
          }
        }
      }
      // this step can take some time
      SetMode(DialogMode.PreparingExperiment);
      experimentCreationBackgroundWorker.ReportProgress(-1);
      localExperiment.Prepare(true);
      experimentCreationBackgroundWorker.ReportProgress(100);
      Experiment = localExperiment;
    }

    private void AddParameterVariations(IOptimizer optimizer, Experiment localExperiment, ref int counter, int totalVariations) {
      var variations = CalculateParameterVariations(optimizer);
      foreach (var v in variations) {
        if (experimentCreationBackgroundWorker.CancellationPending)
          throw new OperationCanceledException();
        AddOptimizer(v, localExperiment);
        counter++;
        experimentCreationBackgroundWorker.ReportProgress((int)Math.Round((100.0 * counter) / totalVariations), string.Empty);
      }
    }

    private IEnumerable<IOptimizer> CalculateParameterVariations(IOptimizer optimizer) {
      if (!boolParameters.Any() && !intParameters.Any() && !doubleParameters.Any() && !multipleChoiceParameters.Any()) {
        var o = (IOptimizer)optimizer.Clone();
        o.Runs.Clear();
        yield return o;
        yield break;
      }
      bool finished;
      var mcEnumerator = GetMultipleChoiceConfigurations().GetEnumerator();
      var boolEnumerator = GetBoolParameterConfigurations().GetEnumerator();
      var intEnumerator = GetIntParameterConfigurations().GetEnumerator();
      var doubleEnumerator = GetDoubleParameterConfigurations().GetEnumerator();
      mcEnumerator.MoveNext(); boolEnumerator.MoveNext(); intEnumerator.MoveNext(); doubleEnumerator.MoveNext();
      do {
        var variant = (IAlgorithm)optimizer.Clone();
        variant.Runs.Clear();
        variant.Name += " {";
        finished = true;
        if (doubleParameters.Any()) {
          foreach (var d in doubleEnumerator.Current) {
            var value = (ValueTypeValue<double>)((IValueParameter)variant.Parameters[d.Key.Name]).Value;
            value.Value = d.Value;
            variant.Name += d.Key.Name + "=" + d.Value.ToString() + ", ";
          }
          if (finished) {
            if (doubleEnumerator.MoveNext()) {
              finished = false;
            } else {
              doubleEnumerator = GetDoubleParameterConfigurations().GetEnumerator();
              doubleEnumerator.MoveNext();
            }
          }
        }
        if (intParameters.Any()) {
          foreach (var i in intEnumerator.Current) {
            var value = (ValueTypeValue<int>)((IValueParameter)variant.Parameters[i.Key.Name]).Value;
            value.Value = i.Value;
            variant.Name += i.Key.Name + "=" + i.Value.ToString() + ", ";
          }
          if (finished) {
            if (intEnumerator.MoveNext()) {
              finished = false;
            } else {
              intEnumerator = GetIntParameterConfigurations().GetEnumerator();
              intEnumerator.MoveNext();
            }
          }
        }
        if (boolParameters.Any()) {
          foreach (var b in boolEnumerator.Current) {
            var value = (ValueTypeValue<bool>)((IValueParameter)variant.Parameters[b.Key.Name]).Value;
            value.Value = b.Value;
            variant.Name += b.Key.Name + "=" + b.Value.ToString() + ", ";
          }
          if (finished) {
            if (boolEnumerator.MoveNext()) {
              finished = false;
            } else {
              boolEnumerator = GetBoolParameterConfigurations().GetEnumerator();
              boolEnumerator.MoveNext();
            }
          }
        }
        if (multipleChoiceParameters.Any()) {
          foreach (var m in mcEnumerator.Current) {
            if (m.Key.Value != null && m.Key.Value.GetType().IsGenericType
                && typeof(EnumValue<>).IsAssignableFrom(m.Key.Value.GetType().GetGenericTypeDefinition())) {
              var valueParam = (IValueParameter)variant.Parameters[m.Key.Name];
              dynamic targetEnumValue = valueParam.Value;
              dynamic sourceEnumValue = m.Value;
              targetEnumValue.Value = sourceEnumValue.Value;
              variant.Name += m.Key.Name + "=" + m.Value + ", ";
            } else {
              dynamic variantParam = variant.Parameters[m.Key.Name];
              if (m.Value == optionalNullChoice) {
                variantParam.Value = null;
                variant.Name += m.Key.Name + "=null, ";
                continue;
              }
              var variantEnumerator = ((IEnumerable<object>)variantParam.ValidValues).GetEnumerator();
              var originalEnumerator = ((IEnumerable<object>)((dynamic)m.Key).ValidValues).GetEnumerator();
              while (variantEnumerator.MoveNext() && originalEnumerator.MoveNext()) {
                if (m.Value == (IItem)originalEnumerator.Current) {
                  variantParam.Value = (dynamic)variantEnumerator.Current;
                  if (m.Value is INamedItem)
                    variant.Name += m.Key.Name + "=" + ((INamedItem)m.Value).Name + ", ";
                  else variant.Name += m.Key.Name + "=" + m.Value.ToString() + ", ";
                  break;
                }
              }
            }
          }
          if (finished) {
            if (mcEnumerator.MoveNext()) {
              finished = false;
            } else {
              mcEnumerator = GetMultipleChoiceConfigurations().GetEnumerator();
              mcEnumerator.MoveNext();
            }
          }
        }
        variant.Name = variant.Name.Substring(0, variant.Name.Length - 2) + "}";
        yield return variant;
      } while (!finished);
    }

    private void experimentCreationBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
      if (e.ProgressPercentage >= 0 && e.ProgressPercentage <= 100) {
        experimentCreationProgressBar.Style = ProgressBarStyle.Continuous;
        experimentCreationProgressBar.Value = e.ProgressPercentage;
      } else {
        experimentCreationProgressBar.Style = ProgressBarStyle.Marquee;
      }
    }

    private void experimentCreationBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      try {
        SetMode(DialogMode.Normal);
        if (e.Error != null) MessageBox.Show(e.Error.Message, "Error occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
        if (failedInstances.Length > 0) MessageBox.Show("Some instances could not be loaded: " + Environment.NewLine + failedInstances.ToString(), "Some instances failed to load", MessageBoxButtons.OK, MessageBoxIcon.Error);
        if (!e.Cancelled && e.Error == null) {
          DialogResult = System.Windows.Forms.DialogResult.OK;
          Close();
        }
      }
      catch { }
    }
    #endregion
    #endregion
  }
}
