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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Represents a Gaussian process model.
  /// </summary>
  [StorableClass]
  [Item("GaussianProcessModel", "Represents a Gaussian process posterior.")]
  public sealed class GaussianProcessModel : RegressionModel, IGaussianProcessModel {
    public override IEnumerable<string> VariablesUsedForPrediction {
      get { return allowedInputVariables; }
    }

    [Storable]
    private double negativeLogLikelihood;
    public double NegativeLogLikelihood {
      get { return negativeLogLikelihood; }
    }

    [Storable]
    private double loocvNegLogPseudoLikelihood;
    public double LooCvNegativeLogPseudoLikelihood {
      get { return loocvNegLogPseudoLikelihood; }
    }

    [Storable]
    private double[] hyperparameterGradients;
    public double[] HyperparameterGradients {
      get {
        var copy = new double[hyperparameterGradients.Length];
        Array.Copy(hyperparameterGradients, copy, copy.Length);
        return copy;
      }
    }

    [Storable]
    private ICovarianceFunction covarianceFunction;
    public ICovarianceFunction CovarianceFunction {
      get { return covarianceFunction; }
    }
    [Storable]
    private IMeanFunction meanFunction;
    public IMeanFunction MeanFunction {
      get { return meanFunction; }
    }

    [Storable]
    private string[] allowedInputVariables;
    public string[] AllowedInputVariables {
      get { return allowedInputVariables; }
    }

    [Storable]
    private double[] alpha;
    [Storable]
    private double sqrSigmaNoise;
    public double SigmaNoise {
      get { return Math.Sqrt(sqrSigmaNoise); }
    }

    [Storable]
    private double[] meanParameter;
    [Storable]
    private double[] covarianceParameter;

    private double[,] l; // used to be storable in previous versions (is calculated lazily now)
    private double[,] x; // scaled training dataset, used to be storable in previous versions (is calculated lazily now)

    // BackwardsCompatibility3.4
    #region Backwards compatible code, remove with 3.5
    [Storable(Name = "l")] // restore if available but don't store anymore
    private double[,] l_storable {
      set { this.l = value; }
      get {
        if (trainingDataset == null) return l; // this model has been created with an old version 
        else return null; // if the training dataset is available l should not be serialized
      }
    }
    [Storable(Name = "x")] // restore if available but don't store anymore
    private double[,] x_storable {
      set { this.x = value; }
      get {
        if (trainingDataset == null) return x; // this model has been created with an old version 
        else return null; // if the training dataset is available x should not be serialized
      }
    }
    #endregion


    [Storable]
    private IDataset trainingDataset; // it is better to store the original training dataset completely because this is more efficient in persistence
    [Storable]
    private int[] trainingRows;

    [Storable]
    private Scaling inputScaling;


    [StorableConstructor]
    private GaussianProcessModel(bool deserializing) : base(deserializing) { }
    private GaussianProcessModel(GaussianProcessModel original, Cloner cloner)
      : base(original, cloner) {
      this.meanFunction = cloner.Clone(original.meanFunction);
      this.covarianceFunction = cloner.Clone(original.covarianceFunction);
      if (original.inputScaling != null)
        this.inputScaling = cloner.Clone(original.inputScaling);
      this.trainingDataset = cloner.Clone(original.trainingDataset);
      this.negativeLogLikelihood = original.negativeLogLikelihood;
      this.loocvNegLogPseudoLikelihood = original.loocvNegLogPseudoLikelihood;
      this.sqrSigmaNoise = original.sqrSigmaNoise;
      if (original.meanParameter != null) {
        this.meanParameter = (double[])original.meanParameter.Clone();
      }
      if (original.covarianceParameter != null) {
        this.covarianceParameter = (double[])original.covarianceParameter.Clone();
      }

      // shallow copies of arrays because they cannot be modified
      this.trainingRows = original.trainingRows;
      this.allowedInputVariables = original.allowedInputVariables;
      this.alpha = original.alpha;
      this.l = original.l;
      this.x = original.x;
    }
    public GaussianProcessModel(IDataset ds, string targetVariable, IEnumerable<string> allowedInputVariables, IEnumerable<int> rows,
      IEnumerable<double> hyp, IMeanFunction meanFunction, ICovarianceFunction covarianceFunction,
      bool scaleInputs = true)
      : base(targetVariable) {
      this.name = ItemName;
      this.description = ItemDescription;
      this.meanFunction = (IMeanFunction)meanFunction.Clone();
      this.covarianceFunction = (ICovarianceFunction)covarianceFunction.Clone();
      this.allowedInputVariables = allowedInputVariables.ToArray();


      int nVariables = this.allowedInputVariables.Length;
      meanParameter = hyp
        .Take(this.meanFunction.GetNumberOfParameters(nVariables))
        .ToArray();

      covarianceParameter = hyp.Skip(this.meanFunction.GetNumberOfParameters(nVariables))
                                             .Take(this.covarianceFunction.GetNumberOfParameters(nVariables))
                                             .ToArray();
      sqrSigmaNoise = Math.Exp(2.0 * hyp.Last());
      try {
        CalculateModel(ds, rows, scaleInputs);
      } catch (alglib.alglibexception ae) {
        // wrap exception so that calling code doesn't have to know about alglib implementation
        throw new ArgumentException("There was a problem in the calculation of the Gaussian process model", ae);
      }
    }

    private void CalculateModel(IDataset ds, IEnumerable<int> rows, bool scaleInputs = true) {
      this.trainingDataset = (IDataset)ds.Clone();
      this.trainingRows = rows.ToArray();
      this.inputScaling = scaleInputs ? new Scaling(ds, allowedInputVariables, rows) : null;

      x = GetData(ds, this.allowedInputVariables, this.trainingRows, this.inputScaling);

      IEnumerable<double> y;
      y = ds.GetDoubleValues(TargetVariable, rows);

      int n = x.GetLength(0);

      var columns = Enumerable.Range(0, x.GetLength(1)).ToArray();
      // calculate cholesky decomposed (lower triangular) covariance matrix
      var cov = covarianceFunction.GetParameterizedCovarianceFunction(covarianceParameter, columns);
      this.l = CalculateL(x, cov, sqrSigmaNoise);

      // calculate mean
      var mean = meanFunction.GetParameterizedMeanFunction(meanParameter, columns);
      double[] m = Enumerable.Range(0, x.GetLength(0))
        .Select(r => mean.Mean(x, r))
        .ToArray();

      // calculate sum of diagonal elements for likelihood
      double diagSum = Enumerable.Range(0, n).Select(i => Math.Log(l[i, i])).Sum();

      // solve for alpha
      double[] ym = y.Zip(m, (a, b) => a - b).ToArray();

      int info;
      alglib.densesolverreport denseSolveRep;

      alglib.spdmatrixcholeskysolve(l, n, false, ym, out info, out denseSolveRep, out alpha);
      for (int i = 0; i < alpha.Length; i++)
        alpha[i] = alpha[i] / sqrSigmaNoise;
      negativeLogLikelihood = 0.5 * Util.ScalarProd(ym, alpha) + diagSum + (n / 2.0) * Math.Log(2.0 * Math.PI * sqrSigmaNoise);

      // derivatives
      int nAllowedVariables = x.GetLength(1);

      alglib.matinvreport matInvRep;
      double[,] lCopy = new double[l.GetLength(0), l.GetLength(1)];
      Array.Copy(l, lCopy, lCopy.Length);

      alglib.spdmatrixcholeskyinverse(ref lCopy, n, false, out info, out matInvRep);
      if (info != 1) throw new ArgumentException("Can't invert matrix to calculate gradients.");

      // LOOCV log pseudo-likelihood (or log predictive probability) (GPML page 116 and 117)
      var sumLoo = 0.0;
      var ki = new double[n];
      for (int i = 0; i < n; i++) {
        for (int j = 0; j < n; j++) ki[j] = cov.Covariance(x, i, j);
        ki[i] += sqrSigmaNoise;

        var yi = Util.ScalarProd(ki, alpha);
        var yi_loo = yi - alpha[i] / (lCopy[i, i] / sqrSigmaNoise);
        var s2_loo = 1.0 / (lCopy[i, i] / sqrSigmaNoise);
        var err = ym[i] - yi_loo;
        var nll_loo = 0.5 * Math.Log(2 * Math.PI * s2_loo) + 0.5 * err * err / s2_loo;
        sumLoo += nll_loo;
      }
      loocvNegLogPseudoLikelihood = sumLoo;

      for (int i = 0; i < n; i++) {
        for (int j = 0; j <= i; j++)
          lCopy[i, j] = lCopy[i, j] / sqrSigmaNoise - alpha[i] * alpha[j];
      }

      double noiseGradient = sqrSigmaNoise * Enumerable.Range(0, n).Select(i => lCopy[i, i]).Sum();

      double[] meanGradients = new double[meanFunction.GetNumberOfParameters(nAllowedVariables)];
      for (int k = 0; k < meanGradients.Length; k++) {
        var meanGrad = new double[alpha.Length];
        for (int g = 0; g < meanGrad.Length; g++)
          meanGrad[g] = mean.Gradient(x, g, k);
        meanGradients[k] = -Util.ScalarProd(meanGrad, alpha);
      }

      double[] covGradients = new double[covarianceFunction.GetNumberOfParameters(nAllowedVariables)];
      if (covGradients.Length > 0) {
        for (int i = 0; i < n; i++) {
          for (int j = 0; j < i; j++) {
            var g = cov.CovarianceGradient(x, i, j);
            for (int k = 0; k < covGradients.Length; k++) {
              covGradients[k] += lCopy[i, j] * g[k];
            }
          }

          var gDiag = cov.CovarianceGradient(x, i, i);
          for (int k = 0; k < covGradients.Length; k++) {
            // diag
            covGradients[k] += 0.5 * lCopy[i, i] * gDiag[k];
          }
        }
      }

      hyperparameterGradients =
        meanGradients
        .Concat(covGradients)
        .Concat(new double[] { noiseGradient }).ToArray();

    }

    private static double[,] GetData(IDataset ds, IEnumerable<string> allowedInputs, IEnumerable<int> rows, Scaling scaling) {
      if (scaling != null) {
        // BackwardsCompatibility3.3
        #region Backwards compatible code, remove with 3.4
        // TODO: completely remove Scaling class
        List<string> variablesList = allowedInputs.ToList();
        List<int> rowsList = rows.ToList();

        double[,] matrix = new double[rowsList.Count, variablesList.Count];

        int col = 0;
        foreach (string column in variablesList) {
          var values = scaling.GetScaledValues(ds, column, rowsList);
          int row = 0;
          foreach (var value in values) {
            matrix[row, col] = value;
            row++;
          }
          col++;
        }
        return matrix;
        #endregion
      } else {
        return ds.ToArray(allowedInputs, rows);
      }
    }

    private static double[,] CalculateL(double[,] x, ParameterizedCovarianceFunction cov, double sqrSigmaNoise) {
      int n = x.GetLength(0);
      var l = new double[n, n];

      // calculate covariances
      for (int i = 0; i < n; i++) {
        for (int j = i; j < n; j++) {
          l[j, i] = cov.Covariance(x, i, j) / sqrSigmaNoise;
          if (j == i) l[j, i] += 1.0;
        }
      }

      // cholesky decomposition
      var res = alglib.trfac.spdmatrixcholesky(ref l, n, false);
      if (!res) throw new ArgumentException("Matrix is not positive semidefinite");
      return l;
    }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new GaussianProcessModel(this, cloner);
    }

    // is called by the solution creator to set all parameter values of the covariance and mean function
    // to the optimized values (necessary to make the values visible in the GUI)
    public void FixParameters() {
      covarianceFunction.SetParameter(covarianceParameter);
      meanFunction.SetParameter(meanParameter);
      covarianceParameter = new double[0];
      meanParameter = new double[0];
    }

    #region IRegressionModel Members
    public override IEnumerable<double> GetEstimatedValues(IDataset dataset, IEnumerable<int> rows) {
      return GetEstimatedValuesHelper(dataset, rows);
    }
    public override IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      return new GaussianProcessRegressionSolution(this, new RegressionProblemData(problemData));
    }
    #endregion


    private IEnumerable<double> GetEstimatedValuesHelper(IDataset dataset, IEnumerable<int> rows) {
      try {
        if (x == null) {
          x = GetData(trainingDataset, allowedInputVariables, trainingRows, inputScaling);
        }
        int n = x.GetLength(0);

        double[,] newX = GetData(dataset, allowedInputVariables, rows, inputScaling);
        int newN = newX.GetLength(0);

        var Ks = new double[newN][];
        var columns = Enumerable.Range(0, newX.GetLength(1)).ToArray();
        var mean = meanFunction.GetParameterizedMeanFunction(meanParameter, columns);
        var ms = Enumerable.Range(0, newX.GetLength(0))
        .Select(r => mean.Mean(newX, r))
        .ToArray();
        var cov = covarianceFunction.GetParameterizedCovarianceFunction(covarianceParameter, columns);
        for (int i = 0; i < newN; i++) {
          Ks[i] = new double[n];
          for (int j = 0; j < n; j++) {
            Ks[i][j] = cov.CrossCovariance(x, newX, j, i);
          }
        }

        return Enumerable.Range(0, newN)
          .Select(i => ms[i] + Util.ScalarProd(Ks[i], alpha));
      } catch (alglib.alglibexception ae) {
        // wrap exception so that calling code doesn't have to know about alglib implementation
        throw new ArgumentException("There was a problem in the calculation of the Gaussian process model", ae);
      }
    }

    public IEnumerable<double> GetEstimatedVariances(IDataset dataset, IEnumerable<int> rows) {
      try {
        if (x == null) {
          x = GetData(trainingDataset, allowedInputVariables, trainingRows, inputScaling);
        }
        int n = x.GetLength(0);

        var newX = GetData(dataset, allowedInputVariables, rows, inputScaling);
        int newN = newX.GetLength(0);

        var kss = new double[newN];
        double[,] sWKs = new double[n, newN];
        var columns = Enumerable.Range(0, newX.GetLength(1)).ToArray();
        var cov = covarianceFunction.GetParameterizedCovarianceFunction(covarianceParameter, columns);

        if (l == null) {
          l = CalculateL(x, cov, sqrSigmaNoise);
        }

        // for stddev 
        for (int i = 0; i < newN; i++)
          kss[i] = cov.Covariance(newX, i, i);

        for (int i = 0; i < newN; i++) {
          for (int j = 0; j < n; j++) {
            sWKs[j, i] = cov.CrossCovariance(x, newX, j, i) / Math.Sqrt(sqrSigmaNoise);
          }
        }

        // for stddev 
        alglib.ablas.rmatrixlefttrsm(n, newN, l, 0, 0, false, false, 0, ref sWKs, 0, 0);

        for (int i = 0; i < newN; i++) {
          var col = Util.GetCol(sWKs, i).ToArray();
          var sumV = Util.ScalarProd(col, col);
          kss[i] += sqrSigmaNoise; // kss is V(f), add noise variance of predictive distibution to get V(y)
          kss[i] -= sumV;
          if (kss[i] < 0) kss[i] = 0;
        }
        return kss;
      } catch (alglib.alglibexception ae) {
        // wrap exception so that calling code doesn't have to know about alglib implementation
        throw new ArgumentException("There was a problem in the calculation of the Gaussian process model", ae);
      }
    }

  }
}
