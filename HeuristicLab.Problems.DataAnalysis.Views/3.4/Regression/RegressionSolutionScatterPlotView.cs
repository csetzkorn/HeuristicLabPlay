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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Visualization.ChartControlsExtensions;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Scatter Plot")]
  [Content(typeof(IRegressionSolution))]
  public partial class RegressionSolutionScatterPlotView : DataAnalysisSolutionEvaluationView {
    private const string ALL_SERIES = "All samples";
    private const string TRAINING_SERIES = "Training samples";
    private const string TEST_SERIES = "Test samples";

    private const int OPACITY_LEVEL = 150;

    public new IRegressionSolution Content {
      get { return (IRegressionSolution)base.Content; }
      set { base.Content = value; }
    }

    public RegressionSolutionScatterPlotView()
      : base() {
      InitializeComponent();

      this.chart.Series.Add(ALL_SERIES);
      this.chart.Series[ALL_SERIES].LegendText = ALL_SERIES;
      this.chart.Series[ALL_SERIES].ChartType = SeriesChartType.FastPoint;

      this.chart.Series.Add(TRAINING_SERIES);
      this.chart.Series[TRAINING_SERIES].LegendText = TRAINING_SERIES;
      this.chart.Series[TRAINING_SERIES].ChartType = SeriesChartType.FastPoint;
      this.chart.Series[TRAINING_SERIES].Points.Add(1.0);

      this.chart.Series.Add(TEST_SERIES);
      this.chart.Series[TEST_SERIES].LegendText = TEST_SERIES;
      this.chart.Series[TEST_SERIES].ChartType = SeriesChartType.FastPoint;

      this.chart.TextAntiAliasingQuality = TextAntiAliasingQuality.High;
      this.chart.AxisViewChanged += new EventHandler<System.Windows.Forms.DataVisualization.Charting.ViewEventArgs>(chart_AxisViewChanged);

      //make series colors semi transparent
      this.chart.ApplyPaletteColors();
      this.chart.Series[ALL_SERIES].Color = Color.FromArgb(OPACITY_LEVEL, this.chart.Series[ALL_SERIES].Color);
      this.chart.Series[TRAINING_SERIES].Color = Color.FromArgb(OPACITY_LEVEL, this.chart.Series[TRAINING_SERIES].Color);
      this.chart.Series[TEST_SERIES].Color = Color.FromArgb(OPACITY_LEVEL, this.chart.Series[TEST_SERIES].Color);

      //change all markers to circles
      this.chart.Series[ALL_SERIES].MarkerStyle = MarkerStyle.Circle;
      this.chart.Series[TRAINING_SERIES].MarkerStyle = MarkerStyle.Circle;
      this.chart.Series[TEST_SERIES].MarkerStyle = MarkerStyle.Circle;

      //configure axis 
      this.chart.CustomizeAllChartAreas();
      this.chart.ChartAreas[0].AxisY.Title = "Estimated Values";
      this.chart.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
      this.chart.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
      this.chart.ChartAreas[0].CursorX.Interval = 1;
      this.chart.ChartAreas[0].CursorY.Interval = 1;

      this.chart.ChartAreas[0].AxisX.Title = "Target Values";
      this.chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
      this.chart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
      this.chart.ChartAreas[0].AxisX.IsStartedFromZero = true;
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ModelChanged += new EventHandler(Content_ModelChanged);
      Content.ProblemDataChanged += new EventHandler(Content_ProblemDataChanged);
    }
    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.ModelChanged -= new EventHandler(Content_ModelChanged);
      Content.ProblemDataChanged -= new EventHandler(Content_ProblemDataChanged);
    }


    private void Content_ProblemDataChanged(object sender, EventArgs e) {
      UpdateChart();
    }
    private void Content_ModelChanged(object sender, EventArgs e) {
      UpdateSeries();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      UpdateChart();
    }

    private void UpdateChart() {
      if (InvokeRequired) Invoke((Action)UpdateChart);
      else {
        if (Content != null) {
          this.UpdateSeries();
          if (!this.chart.Series.Any(s => s.Points.Count > 0))
            this.ClearChart();
        }
      }
    }

    private void UpdateCursorInterval() {
      var estimatedValues = this.chart.Series[ALL_SERIES].Points.Select(x => x.XValue).DefaultIfEmpty(1.0);
      var targetValues = this.chart.Series[ALL_SERIES].Points.Select(x => x.YValues[0]).DefaultIfEmpty(1.0);
      double estimatedValuesRange = estimatedValues.Max() - estimatedValues.Min();
      double targetValuesRange = targetValues.Max() - targetValues.Min();
      double interestingValuesRange = Math.Min(Math.Max(targetValuesRange, 1.0), Math.Max(estimatedValuesRange, 1.0));
      double digits = (int)Math.Log10(interestingValuesRange) - 3;
      double zoomInterval = Math.Max(Math.Pow(10, digits), 10E-5);
      this.chart.ChartAreas[0].CursorX.Interval = zoomInterval;
      this.chart.ChartAreas[0].CursorY.Interval = zoomInterval;

      this.chart.ChartAreas[0].AxisX.ScaleView.SmallScrollSize = zoomInterval;
      this.chart.ChartAreas[0].AxisY.ScaleView.SmallScrollSize = zoomInterval;

      this.chart.ChartAreas[0].AxisX.ScaleView.SmallScrollMinSizeType = DateTimeIntervalType.Number;
      this.chart.ChartAreas[0].AxisX.ScaleView.SmallScrollMinSize = zoomInterval;
      this.chart.ChartAreas[0].AxisY.ScaleView.SmallScrollMinSizeType = DateTimeIntervalType.Number;
      this.chart.ChartAreas[0].AxisY.ScaleView.SmallScrollMinSize = zoomInterval;

      if (digits < 0) {
        this.chart.ChartAreas[0].AxisX.LabelStyle.Format = "F" + (int)Math.Abs(digits);
        this.chart.ChartAreas[0].AxisY.LabelStyle.Format = "F" + (int)Math.Abs(digits);
      } else {
        this.chart.ChartAreas[0].AxisX.LabelStyle.Format = "F0";
        this.chart.ChartAreas[0].AxisY.LabelStyle.Format = "F0";
      }
    }


    private void UpdateSeries() {
      if (InvokeRequired) Invoke((Action)UpdateSeries);
      else {
        string targetVariableName = Content.ProblemData.TargetVariable;
        var dataset = Content.ProblemData.Dataset;
        if (this.chart.Series[ALL_SERIES].Points.Count > 0)
          this.chart.Series[ALL_SERIES].Points.DataBindXY(dataset.GetDoubleValues(targetVariableName).ToArray(), "",
            Content.EstimatedValues.ToArray(), "");
        if (this.chart.Series[TRAINING_SERIES].Points.Count > 0)
          this.chart.Series[TRAINING_SERIES].Points.DataBindXY(dataset.GetDoubleValues(targetVariableName, Content.ProblemData.TrainingIndices).ToArray(), "",
            Content.EstimatedTrainingValues.ToArray(), "");
        if (this.chart.Series[TEST_SERIES].Points.Count > 0)
          this.chart.Series[TEST_SERIES].Points.DataBindXY(dataset.GetDoubleValues(targetVariableName, Content.ProblemData.TestIndices).ToArray(), "",
            Content.EstimatedTestValues.ToArray(), "");
        double max = Content.EstimatedTrainingValues.Concat(Content.EstimatedTestValues.Concat(Content.EstimatedValues.Concat(dataset.GetDoubleValues(targetVariableName)))).Max();
        double min = Content.EstimatedTrainingValues.Concat(Content.EstimatedTestValues.Concat(Content.EstimatedValues.Concat(dataset.GetDoubleValues(targetVariableName)))).Min();

        double axisMin, axisMax, axisInterval;
        ChartUtil.CalculateOptimalAxisInterval(min, max, out axisMin, out axisMax, out axisInterval);
        this.chart.ChartAreas[0].AxisY.Title = "Estimated " + targetVariableName;
        this.chart.ChartAreas[0].AxisY.Maximum = axisMax;
        this.chart.ChartAreas[0].AxisY.Minimum = axisMin;
        this.chart.ChartAreas[0].AxisY.Interval = axisInterval;
        this.chart.ChartAreas[0].AxisX.Title = targetVariableName;
        this.chart.ChartAreas[0].AxisX.Maximum = axisMax;
        this.chart.ChartAreas[0].AxisX.Minimum = axisMin;
        this.chart.ChartAreas[0].AxisX.Interval = axisInterval;

        UpdateCursorInterval();
      }
    }

    private void ClearChart() {
      this.chart.Series[ALL_SERIES].Points.Clear();
      this.chart.Series[TRAINING_SERIES].Points.Clear();
      this.chart.Series[TEST_SERIES].Points.Clear();
    }

    private void ToggleSeriesData(Series series) {
      if (series.Points.Count > 0) {  //checks if series is shown
        if (this.chart.Series.Any(s => s != series && s.Points.Count > 0)) {
          series.Points.Clear();
        }
      } else if (Content != null) {
        string targetVariableName = Content.ProblemData.TargetVariable;

        double[] predictedValues = null;
        double[] targetValues = null;
        switch (series.Name) {
          case ALL_SERIES:
            predictedValues = Content.EstimatedValues.ToArray();
            targetValues = Content.ProblemData.Dataset.GetDoubleValues(targetVariableName).ToArray();
            break;
          case TRAINING_SERIES:
            predictedValues = Content.EstimatedTrainingValues.ToArray();
            targetValues = Content.ProblemData.Dataset.GetDoubleValues(targetVariableName, Content.ProblemData.TrainingIndices).ToArray();
            break;
          case TEST_SERIES:
            predictedValues = Content.EstimatedTestValues.ToArray();
            targetValues = Content.ProblemData.Dataset.GetDoubleValues(targetVariableName, Content.ProblemData.TestIndices).ToArray();
            break;
        }
        if (predictedValues.Length == targetValues.Length)
          series.Points.DataBindXY(targetValues, "", predictedValues, "");
        this.chart.Legends[series.Legend].ForeColor = Color.Black;
        UpdateCursorInterval();
      }
    }

    private void chart_MouseDown(object sender, MouseEventArgs e) {
      HitTestResult result = chart.HitTest(e.X, e.Y);
      if (result.ChartElementType == ChartElementType.LegendItem) {
        this.ToggleSeriesData(result.Series);
      }
    }

    private void chart_MouseMove(object sender, MouseEventArgs e) {
      HitTestResult result = chart.HitTest(e.X, e.Y);
      if (result.ChartElementType == ChartElementType.LegendItem)
        this.Cursor = Cursors.Hand;
      else
        this.Cursor = Cursors.Default;
    }

    private void chart_AxisViewChanged(object sender, System.Windows.Forms.DataVisualization.Charting.ViewEventArgs e) {
      this.chart.ChartAreas[0].AxisX.ScaleView.Size = e.NewSize;
      this.chart.ChartAreas[0].AxisY.ScaleView.Size = e.NewSize;
    }

    private void chart_CustomizeLegend(object sender, CustomizeLegendEventArgs e) {
      e.LegendItems[0].Cells[1].ForeColor = this.chart.Series[ALL_SERIES].Points.Count == 0 ? Color.Gray : Color.Black;
      e.LegendItems[1].Cells[1].ForeColor = this.chart.Series[TRAINING_SERIES].Points.Count == 0 ? Color.Gray : Color.Black;
      e.LegendItems[2].Cells[1].ForeColor = this.chart.Series[TEST_SERIES].Points.Count == 0 ? Color.Gray : Color.Black;
    }

    private void chart_PostPaint(object sender, ChartPaintEventArgs e) {
      var chartArea = e.ChartElement as ChartArea;
      if (chartArea != null) {
        ChartGraphics chartGraphics = e.ChartGraphics;
        using (Pen p = new Pen(Color.DarkGray)) {
          double xmin = chartArea.AxisX.ScaleView.ViewMinimum;
          double xmax = chartArea.AxisX.ScaleView.ViewMaximum;
          double ymin = chartArea.AxisY.ScaleView.ViewMinimum;
          double ymax = chartArea.AxisY.ScaleView.ViewMaximum;

          if (xmin > ymax || ymin > xmax) return;

          PointF start = PointF.Empty;
          start.X = (float)chartGraphics.GetPositionFromAxis(chartArea.Name, chartArea.AxisX.AxisName, Math.Max(xmin, ymin));
          start.Y = (float)chartGraphics.GetPositionFromAxis(chartArea.Name, chartArea.AxisY.AxisName, Math.Max(xmin, ymin));
          PointF end = PointF.Empty;
          end.X = (float)chartGraphics.GetPositionFromAxis(chartArea.Name, chartArea.AxisX.AxisName, Math.Min(xmax, ymax));
          end.Y = (float)chartGraphics.GetPositionFromAxis(chartArea.Name, chartArea.AxisY.AxisName, Math.Min(xmax, ymax));

          chartGraphics.Graphics.DrawLine(p, chartGraphics.GetAbsolutePoint(start), chartGraphics.GetAbsolutePoint(end));
        }
      }
    }
  }
}
