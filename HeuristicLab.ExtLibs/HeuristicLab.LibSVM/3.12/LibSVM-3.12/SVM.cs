﻿/*
 * Copyright (c) 2000-2012 Chih-Chung Chang and Chih-Jen Lin
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 
 * 1. Redistributions of source code must retain the above copyright
 * notice, this list of conditions and the following disclaimer.
 * 
 * 2. Redistributions in binary form must reproduce the above copyright
 * notice, this list of conditions and the following disclaimer in the
 * documentation and/or other materials provided with the distribution.
 * 
 * 3. Neither name of copyright holders nor the names of its contributors
 * may be used to endorse or promote products derived from this software
 * without specific prior written permission.
 * 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE REGENTS OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 * C# port from the original java sources by Gabriel Kronberger (Sept. 2012)
 */

using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace LibSVM {
  //
  // Kernel Cache
  //
  // l is the number of total data items
  // size is the cache size limit in bytes
  //

  using System;

  class Cache {
    private readonly int l;
    private long size;
    private sealed class head_t {
      public head_t prev, next;	// a cicular list
      public float[] data;
      public int len;		// data[0,len) is cached in this entry
    }
    private readonly head_t[] head;
    private head_t lru_head;

    public Cache(int l_, long size_) {
      l = l_;
      size = size_;
      head = new head_t[l];
      for (int i = 0; i < l; i++) head[i] = new head_t();
      size /= 4;
      size -= l * (16 / 4);	// sizeof(head_t) == 16
      size = Math.Max(size, 2 * (long)l);  // cache must be large enough for two columns
      lru_head = new head_t();
      lru_head.next = lru_head.prev = lru_head;
    }

    private void lru_delete(head_t h) {
      // delete from current location
      h.prev.next = h.next;
      h.next.prev = h.prev;
    }

    private void lru_insert(head_t h) {
      // insert to last position
      h.next = lru_head;
      h.prev = lru_head.prev;
      h.prev.next = h;
      h.next.prev = h;
    }

    // request data [0,len)
    // return some position p where [p,len) need to be filled
    // (p >= len if nothing needs to be filled)
    // java: simulate pointer using single-element array
    public int get_data(int index, float[][] data, int len) {
      head_t h = head[index];
      if (h.len > 0) lru_delete(h);
      int more = len - h.len;

      if (more > 0) {
        // free old space
        while (size < more) {
          head_t old = lru_head.next;
          lru_delete(old);
          size += old.len;
          old.data = null;
          old.len = 0;
        }

        // allocate new space
        float[] new_data = new float[len];
        if (h.data != null) {
          Array.Copy(h.data, 0, new_data, 0, h.len);
        }
        h.data = new_data;
        size -= more; { int _ = h.len; h.len = len; len = _; }
      }

      lru_insert(h);
      data[0] = h.data;
      return len;
    }

    public void swap_index(int i, int j) {
      if (i == j) return;

      if (head[i].len > 0) lru_delete(head[i]);
      if (head[j].len > 0) lru_delete(head[j]); { float[] _ = head[i].data; head[i].data = head[j].data; head[j].data = _; }
      { int _ = head[i].len; head[i].len = head[j].len; head[j].len = _; }
      if (head[i].len > 0) lru_insert(head[i]);
      if (head[j].len > 0) lru_insert(head[j]);

      if (i > j) { int _ = i; i = j; j = _; }
      for (head_t h = lru_head.next; h != lru_head; h = h.next) {
        if (h.len > i) {
          if (h.len > j) { float _ = h.data[i]; h.data[i] = h.data[j]; h.data[j] = _; } else {
            // give up
            lru_delete(h);
            size += h.len;
            h.data = null;
            h.len = 0;
          }
        }
      }
    }
  }

  //
  // Kernel evaluation
  //
  // the static method k_function is for doing single kernel evaluation
  // the constructor of Kernel prepares to calculate the l*l kernel matrix
  // the member function get_Q is for getting one column from the Q Matrix
  //
  abstract class QMatrix {
    public abstract float[] get_Q(int column, int len);
    public abstract double[] get_QD();
    public abstract void swap_index(int i, int j);
  };

  abstract class Kernel : QMatrix {
    private svm_node[][] x;
    private readonly double[] x_square;

    // svm_parameter
    private readonly int kernel_type;
    private readonly int degree;
    private readonly double gamma;
    private readonly double coef0;

    public override abstract float[] get_Q(int column, int len);
    public override abstract double[] get_QD();

    public override void swap_index(int i, int j) {
      { svm_node[] _ = x[i]; x[i] = x[j]; x[j] = _; }
      if (x_square != null) { double _ = x_square[i]; x_square[i] = x_square[j]; x_square[j] = _; }
    }

    private static double powi(double @base, int times) {
      double tmp = @base, ret = 1.0;

      for (int t = times; t > 0; t /= 2) {
        if (t % 2 == 1) ret *= tmp;
        tmp = tmp * tmp;
      }
      return ret;
    }

    protected virtual double kernel_function(int i, int j) {
      switch (kernel_type) {
        case svm_parameter.LINEAR:
          return dot(x[i], x[j]);
        case svm_parameter.POLY:
          return powi(gamma * dot(x[i], x[j]) + coef0, degree);
        case svm_parameter.RBF:
          return Math.Exp(-gamma * (x_square[i] + x_square[j] - 2 * dot(x[i], x[j])));
        case svm_parameter.SIGMOID:
          return Math.Tanh(gamma * dot(x[i], x[j]) + coef0);
        case svm_parameter.PRECOMPUTED:
          return x[i][(int)(x[j][0].value)].value;
        default:
          return 0;	// java
      }
    }

    public Kernel(int l, svm_node[][] x_, svm_parameter param) {
      this.kernel_type = param.kernel_type;
      this.degree = param.degree;
      this.gamma = param.gamma;
      this.coef0 = param.coef0;

      x = (svm_node[][])x_.Clone();

      if (kernel_type == svm_parameter.RBF) {
        x_square = new double[l];
        for (int i = 0; i < l; i++)
          x_square[i] = dot(x[i], x[i]);
      } else x_square = null;
    }

    static double dot(svm_node[] x, svm_node[] y) {
      double sum = 0;
      int xlen = x.Length;
      int ylen = y.Length;
      int i = 0;
      int j = 0;
      while (i < xlen && j < ylen) {
        if (x[i].index == y[j].index)
          sum += x[i++].value * y[j++].value;
        else {
          if (x[i].index > y[j].index)
            ++j;
          else
            ++i;
        }
      }
      return sum;
    }

    public static double k_function(svm_node[] x, svm_node[] y,
            svm_parameter param) {
      switch (param.kernel_type) {
        case svm_parameter.LINEAR:
          return dot(x, y);
        case svm_parameter.POLY:
          return powi(param.gamma * dot(x, y) + param.coef0, param.degree);
        case svm_parameter.RBF: {
            double sum = 0;
            int xlen = x.Length;
            int ylen = y.Length;
            int i = 0;
            int j = 0;
            while (i < xlen && j < ylen) {
              if (x[i].index == y[j].index) {
                double d = x[i++].value - y[j++].value;
                sum += d * d;
              } else if (x[i].index > y[j].index) {
                sum += y[j].value * y[j].value;
                ++j;
              } else {
                sum += x[i].value * x[i].value;
                ++i;
              }
            }

            while (i < xlen) {
              sum += x[i].value * x[i].value;
              ++i;
            }

            while (j < ylen) {
              sum += y[j].value * y[j].value;
              ++j;
            }

            return Math.Exp(-param.gamma * sum);
          }
        case svm_parameter.SIGMOID:
          return Math.Tanh(param.gamma * dot(x, y) + param.coef0);
        case svm_parameter.PRECOMPUTED:
          return x[(int)(y[0].value)].value;
        default:
          return 0;	// java
      }
    }
  }

  // An SMO algorithm in Fan et al., JMLR 6(2005), p. 1889--1918
  // Solves:
  //
  //	min 0.5(\alpha^T Q \alpha) + p^T \alpha
  //
  //		y^T \alpha = \delta
  //		y_i = +1 or -1
  //		0 <= alpha_i <= Cp for y_i = 1
  //		0 <= alpha_i <= Cn for y_i = -1
  //
  // Given:
  //
  //	Q, p, y, Cp, Cn, and an initial feasible point \alpha
  //	l is the size of vectors and matrices
  //	eps is the stopping tolerance
  //
  // solution will be put in \alpha, objective value will be put in obj
  //
  class Solver {
    protected int active_size;
    protected short[] y;
    protected double[] G;		// gradient of objective function
    protected const byte LOWER_BOUND = 0;
    protected const byte UPPER_BOUND = 1;
    protected const byte FREE = 2;
    protected byte[] alpha_status;	// LOWER_BOUND, UPPER_BOUND, FREE
    protected double[] alpha;
    protected QMatrix Q;
    protected double[] QD;
    protected double eps;
    protected double Cp, Cn;
    protected double[] p;
    protected int[] active_set;
    protected double[] G_bar;		// gradient, if we treat free variables as 0
    protected int l;
    protected bool unshrink;	// XXX

    protected const double INF = double.PositiveInfinity;

    protected virtual double get_C(int i) {
      return (y[i] > 0) ? Cp : Cn;
    }
    protected virtual void update_alpha_status(int i) {
      if (alpha[i] >= get_C(i))
        alpha_status[i] = UPPER_BOUND;
      else if (alpha[i] <= 0)
        alpha_status[i] = LOWER_BOUND;
      else alpha_status[i] = FREE;
    }
    protected virtual bool is_upper_bound(int i) { return alpha_status[i] == UPPER_BOUND; }
    protected virtual bool is_lower_bound(int i) { return alpha_status[i] == LOWER_BOUND; }
    protected virtual bool is_free(int i) { return alpha_status[i] == FREE; }

    // java: information about solution except alpha,
    // because we cannot return multiple values otherwise...
    public class SolutionInfo {
      public double obj;
      public double rho;
      public double upper_bound_p;
      public double upper_bound_n;
      public double r;	// for Solver_NU
    }

    protected virtual void swap_index(int i, int j) {
      Q.swap_index(i, j); { short _ = y[i]; y[i] = y[j]; y[j] = _; } 
      { double _ = G[i]; G[i] = G[j]; G[j] = _; } 
      { byte _ = alpha_status[i]; alpha_status[i] = alpha_status[j]; alpha_status[j] = _; } 
      { double _ = alpha[i]; alpha[i] = alpha[j]; alpha[j] = _; } 
      { double _ = p[i]; p[i] = p[j]; p[j] = _; } 
      { int _ = active_set[i]; active_set[i] = active_set[j]; active_set[j] = _; } 
      { double _ = G_bar[i]; G_bar[i] = G_bar[j]; G_bar[j] = _; }
    }

    protected virtual void reconstruct_gradient() {
      // reconstruct inactive elements of G from G_bar and free variables

      if (active_size == l) return;

      int i, j;
      int nr_free = 0;

      for (j = active_size; j < l; j++)
        G[j] = G_bar[j] + p[j];

      for (j = 0; j < active_size; j++)
        if (is_free(j))
          nr_free++;

      if (2 * nr_free < active_size)
        svm.info("WARNING: using -h 0 may be faster" + Environment.NewLine + Environment.NewLine);

      if (nr_free * l > 2 * active_size * (l - active_size)) {
        for (i = active_size; i < l; i++) {
          float[] Q_i = Q.get_Q(i, active_size);
          for (j = 0; j < active_size; j++)
            if (is_free(j))
              G[i] += alpha[j] * Q_i[j];
        }
      } else {
        for (i = 0; i < active_size; i++)
          if (is_free(i)) {
            float[] Q_i = Q.get_Q(i, l);
            double alpha_i = alpha[i];
            for (j = active_size; j < l; j++)
              G[j] += alpha_i * Q_i[j];
          }
      }
    }

    public virtual void Solve(int l, QMatrix Q, double[] p_, short[] y_,
         double[] alpha_, double Cp, double Cn, double eps, SolutionInfo si, int shrinking) {
      this.l = l;
      this.Q = Q;
      QD = Q.get_QD();
      p = (double[])p_.Clone();
      y = (short[])y_.Clone();
      alpha = (double[])alpha_.Clone();
      this.Cp = Cp;
      this.Cn = Cn;
      this.eps = eps;
      this.unshrink = false;

      // initialize alpha_status
      {
        alpha_status = new byte[l];
        for (int i = 0; i < l; i++)
          update_alpha_status(i);
      }

      // initialize active set (for shrinking)
      {
        active_set = new int[l];
        for (int i = 0; i < l; i++)
          active_set[i] = i;
        active_size = l;
      }

      // initialize gradient
      {
        G = new double[l];
        G_bar = new double[l];
        int i;
        for (i = 0; i < l; i++) {
          G[i] = p[i];
          G_bar[i] = 0;
        }
        for (i = 0; i < l; i++)
          if (!is_lower_bound(i)) {
            float[] Q_i = Q.get_Q(i, l);
            double alpha_i = alpha[i];
            int j;
            for (j = 0; j < l; j++)
              G[j] += alpha_i * Q_i[j];
            if (is_upper_bound(i))
              for (j = 0; j < l; j++)
                G_bar[j] += get_C(i) * Q_i[j];
          }
      }

      // optimization step

      int iter = 0;
      int max_iter = Math.Max(10000000, l > int.MaxValue / 100 ? int.MaxValue : 100 * l);
      int counter = Math.Min(l, 1000) + 1;
      int[] working_set = new int[2];

      while (iter < max_iter) {
        // show progress and do shrinking

        if (--counter == 0) {
          counter = Math.Min(l, 1000);
          if (shrinking != 0) do_shrinking();
          svm.info(".");
        }

        if (select_working_set(working_set) != 0) {
          // reconstruct the whole gradient
          reconstruct_gradient();
          // reset active set size and check
          active_size = l;
          svm.info("*");
          if (select_working_set(working_set) != 0)
            break;
          else
            counter = 1;	// do shrinking next iteration
        }

        int i = working_set[0];
        int j = working_set[1];

        ++iter;

        // update alpha[i] and alpha[j], handle bounds carefully

        float[] Q_i = Q.get_Q(i, active_size);
        float[] Q_j = Q.get_Q(j, active_size);

        double C_i = get_C(i);
        double C_j = get_C(j);

        double old_alpha_i = alpha[i];
        double old_alpha_j = alpha[j];

        if (y[i] != y[j]) {
          double quad_coef = QD[i] + QD[j] + 2 * Q_i[j];
          if (quad_coef <= 0)
            quad_coef = 1e-12;
          double delta = (-G[i] - G[j]) / quad_coef;
          double diff = alpha[i] - alpha[j];
          alpha[i] += delta;
          alpha[j] += delta;

          if (diff > 0) {
            if (alpha[j] < 0) {
              alpha[j] = 0;
              alpha[i] = diff;
            }
          } else {
            if (alpha[i] < 0) {
              alpha[i] = 0;
              alpha[j] = -diff;
            }
          }
          if (diff > C_i - C_j) {
            if (alpha[i] > C_i) {
              alpha[i] = C_i;
              alpha[j] = C_i - diff;
            }
          } else {
            if (alpha[j] > C_j) {
              alpha[j] = C_j;
              alpha[i] = C_j + diff;
            }
          }
        } else {
          double quad_coef = QD[i] + QD[j] - 2 * Q_i[j];
          if (quad_coef <= 0)
            quad_coef = 1e-12;
          double delta = (G[i] - G[j]) / quad_coef;
          double sum = alpha[i] + alpha[j];
          alpha[i] -= delta;
          alpha[j] += delta;

          if (sum > C_i) {
            if (alpha[i] > C_i) {
              alpha[i] = C_i;
              alpha[j] = sum - C_i;
            }
          } else {
            if (alpha[j] < 0) {
              alpha[j] = 0;
              alpha[i] = sum;
            }
          }
          if (sum > C_j) {
            if (alpha[j] > C_j) {
              alpha[j] = C_j;
              alpha[i] = sum - C_j;
            }
          } else {
            if (alpha[i] < 0) {
              alpha[i] = 0;
              alpha[j] = sum;
            }
          }
        }

        // update G

        double delta_alpha_i = alpha[i] - old_alpha_i;
        double delta_alpha_j = alpha[j] - old_alpha_j;

        for (int k = 0; k < active_size; k++) {
          G[k] += Q_i[k] * delta_alpha_i + Q_j[k] * delta_alpha_j;
        }

        // update alpha_status and G_bar

        {
          bool ui = is_upper_bound(i);
          bool uj = is_upper_bound(j);
          update_alpha_status(i);
          update_alpha_status(j);
          int k;
          if (ui != is_upper_bound(i)) {
            Q_i = Q.get_Q(i, l);
            if (ui)
              for (k = 0; k < l; k++)
                G_bar[k] -= C_i * Q_i[k];
            else
              for (k = 0; k < l; k++)
                G_bar[k] += C_i * Q_i[k];
          }

          if (uj != is_upper_bound(j)) {
            Q_j = Q.get_Q(j, l);
            if (uj)
              for (k = 0; k < l; k++)
                G_bar[k] -= C_j * Q_j[k];
            else
              for (k = 0; k < l; k++)
                G_bar[k] += C_j * Q_j[k];
          }
        }

      }

      if (iter >= max_iter) {
        if (active_size < l) {
          // reconstruct the whole gradient to calculate objective value
          reconstruct_gradient();
          active_size = l;
          svm.info("*");
        }
        svm.info("WARNING: reaching max number of iterations" + Environment.NewLine);
      }

      // calculate rho

      si.rho = calculate_rho();

      // calculate objective value
      {
        double v = 0;
        int i;
        for (i = 0; i < l; i++)
          v += alpha[i] * (G[i] + p[i]);

        si.obj = v / 2;
      }

      // put back the solution
      {
        for (int i = 0; i < l; i++)
          alpha_[active_set[i]] = alpha[i];
      }

      si.upper_bound_p = Cp;
      si.upper_bound_n = Cn;

      svm.info("optimization finished, #iter = " + iter + Environment.NewLine);
    }

    // return 1 if already optimal, return 0 otherwise
    protected virtual int select_working_set(int[] working_set) {
      // return i,j such that
      // i: maximizes -y_i * grad(f)_i, i in I_up(\alpha)
      // j: mimimizes the decrease of obj value
      //    (if quadratic coefficeint <= 0, replace it with tau)
      //    -y_j*grad(f)_j < -y_i*grad(f)_i, j in I_low(\alpha)

      double Gmax = -INF;
      double Gmax2 = -INF;
      int Gmax_idx = -1;
      int Gmin_idx = -1;
      double obj_diff_min = INF;

      for (int t = 0; t < active_size; t++)
        if (y[t] == +1) {
          if (!is_upper_bound(t))
            if (-G[t] >= Gmax) {
              Gmax = -G[t];
              Gmax_idx = t;
            }
        } else {
          if (!is_lower_bound(t))
            if (G[t] >= Gmax) {
              Gmax = G[t];
              Gmax_idx = t;
            }
        }

      int i = Gmax_idx;
      float[] Q_i = null;
      if (i != -1) // null Q_i not accessed: Gmax=-INF if i=-1
        Q_i = Q.get_Q(i, active_size);

      for (int j = 0; j < active_size; j++) {
        if (y[j] == +1) {
          if (!is_lower_bound(j)) {
            double grad_diff = Gmax + G[j];
            if (G[j] >= Gmax2)
              Gmax2 = G[j];
            if (grad_diff > 0) {
              double obj_diff;
              double quad_coef = QD[i] + QD[j] - 2.0 * y[i] * Q_i[j];
              if (quad_coef > 0)
                obj_diff = -(grad_diff * grad_diff) / quad_coef;
              else
                obj_diff = -(grad_diff * grad_diff) / 1e-12;

              if (obj_diff <= obj_diff_min) {
                Gmin_idx = j;
                obj_diff_min = obj_diff;
              }
            }
          }
        } else {
          if (!is_upper_bound(j)) {
            double grad_diff = Gmax - G[j];
            if (-G[j] >= Gmax2)
              Gmax2 = -G[j];
            if (grad_diff > 0) {
              double obj_diff;
              double quad_coef = QD[i] + QD[j] + 2.0 * y[i] * Q_i[j];
              if (quad_coef > 0)
                obj_diff = -(grad_diff * grad_diff) / quad_coef;
              else
                obj_diff = -(grad_diff * grad_diff) / 1e-12;

              if (obj_diff <= obj_diff_min) {
                Gmin_idx = j;
                obj_diff_min = obj_diff;
              }
            }
          }
        }
      }

      if (Gmax + Gmax2 < eps)
        return 1;

      working_set[0] = Gmax_idx;
      working_set[1] = Gmin_idx;
      return 0;
    }

    private bool be_shrunk(int i, double Gmax1, double Gmax2) {
      if (is_upper_bound(i)) {
        if (y[i] == +1)
          return (-G[i] > Gmax1);
        else
          return (-G[i] > Gmax2);
      } else if (is_lower_bound(i)) {
        if (y[i] == +1)
          return (G[i] > Gmax2);
        else
          return (G[i] > Gmax1);
      } else
        return (false);
    }

    protected virtual void do_shrinking() {
      int i;
      double Gmax1 = -INF;		// max { -y_i * grad(f)_i | i in I_up(\alpha) }
      double Gmax2 = -INF;		// max { y_i * grad(f)_i | i in I_low(\alpha) }

      // find maximal violating pair first
      for (i = 0; i < active_size; i++) {
        if (y[i] == +1) {
          if (!is_upper_bound(i)) {
            if (-G[i] >= Gmax1)
              Gmax1 = -G[i];
          }
          if (!is_lower_bound(i)) {
            if (G[i] >= Gmax2)
              Gmax2 = G[i];
          }
        } else {
          if (!is_upper_bound(i)) {
            if (-G[i] >= Gmax2)
              Gmax2 = -G[i];
          }
          if (!is_lower_bound(i)) {
            if (G[i] >= Gmax1)
              Gmax1 = G[i];
          }
        }
      }

      if (unshrink == false && Gmax1 + Gmax2 <= eps * 10) {
        unshrink = true;
        reconstruct_gradient();
        active_size = l;
      }

      for (i = 0; i < active_size; i++)
        if (be_shrunk(i, Gmax1, Gmax2)) {
          active_size--;
          while (active_size > i) {
            if (!be_shrunk(active_size, Gmax1, Gmax2)) {
              swap_index(i, active_size);
              break;
            }
            active_size--;
          }
        }
    }

    protected virtual double calculate_rho() {
      double r;
      int nr_free = 0;
      double ub = INF, lb = -INF, sum_free = 0;
      for (int i = 0; i < active_size; i++) {
        double yG = y[i] * G[i];

        if (is_lower_bound(i)) {
          if (y[i] > 0)
            ub = Math.Min(ub, yG);
          else
            lb = Math.Max(lb, yG);
        } else if (is_upper_bound(i)) {
          if (y[i] < 0)
            ub = Math.Min(ub, yG);
          else
            lb = Math.Max(lb, yG);
        } else {
          ++nr_free;
          sum_free += yG;
        }
      }

      if (nr_free > 0)
        r = sum_free / nr_free;
      else
        r = (ub + lb) / 2;

      return r;
    }

  }

  //
  // Solver for nu-svm classification and regression
  //
  // additional constraint: e^T \alpha = constant
  //
  internal sealed class Solver_NU : Solver {
    private SolutionInfo si;

    public override void Solve(int l, QMatrix Q, double[] p, short[] y,
         double[] alpha, double Cp, double Cn, double eps,
         SolutionInfo si, int shrinking) {
      this.si = si;
      base.Solve(l, Q, p, y, alpha, Cp, Cn, eps, si, shrinking);
    }

    // return 1 if already optimal, return 0 otherwise
    protected override int select_working_set(int[] working_set) {
      // return i,j such that y_i = y_j and
      // i: maximizes -y_i * grad(f)_i, i in I_up(\alpha)
      // j: minimizes the decrease of obj value
      //    (if quadratic coefficeint <= 0, replace it with tau)
      //    -y_j*grad(f)_j < -y_i*grad(f)_i, j in I_low(\alpha)

      double Gmaxp = -INF;
      double Gmaxp2 = -INF;
      int Gmaxp_idx = -1;

      double Gmaxn = -INF;
      double Gmaxn2 = -INF;
      int Gmaxn_idx = -1;

      int Gmin_idx = -1;
      double obj_diff_min = INF;

      for (int t = 0; t < active_size; t++)
        if (y[t] == +1) {
          if (!is_upper_bound(t))
            if (-G[t] >= Gmaxp) {
              Gmaxp = -G[t];
              Gmaxp_idx = t;
            }
        } else {
          if (!is_lower_bound(t))
            if (G[t] >= Gmaxn) {
              Gmaxn = G[t];
              Gmaxn_idx = t;
            }
        }

      int ip = Gmaxp_idx;
      int @in = Gmaxn_idx;
      float[] Q_ip = null;
      float[] Q_in = null;
      if (ip != -1) // null Q_ip not accessed: Gmaxp=-INF if ip=-1
        Q_ip = Q.get_Q(ip, active_size);
      if (@in != -1)
        Q_in = Q.get_Q(@in, active_size);

      for (int j = 0; j < active_size; j++) {
        if (y[j] == +1) {
          if (!is_lower_bound(j)) {
            double grad_diff = Gmaxp + G[j];
            if (G[j] >= Gmaxp2)
              Gmaxp2 = G[j];
            if (grad_diff > 0) {
              double obj_diff;
              double quad_coef = QD[ip] + QD[j] - 2 * Q_ip[j];
              if (quad_coef > 0)
                obj_diff = -(grad_diff * grad_diff) / quad_coef;
              else
                obj_diff = -(grad_diff * grad_diff) / 1e-12;

              if (obj_diff <= obj_diff_min) {
                Gmin_idx = j;
                obj_diff_min = obj_diff;
              }
            }
          }
        } else {
          if (!is_upper_bound(j)) {
            double grad_diff = Gmaxn - G[j];
            if (-G[j] >= Gmaxn2)
              Gmaxn2 = -G[j];
            if (grad_diff > 0) {
              double obj_diff;
              double quad_coef = QD[@in] + QD[j] - 2 * Q_in[j];
              if (quad_coef > 0)
                obj_diff = -(grad_diff * grad_diff) / quad_coef;
              else
                obj_diff = -(grad_diff * grad_diff) / 1e-12;

              if (obj_diff <= obj_diff_min) {
                Gmin_idx = j;
                obj_diff_min = obj_diff;
              }
            }
          }
        }
      }

      if (Math.Max(Gmaxp + Gmaxp2, Gmaxn + Gmaxn2) < eps)
        return 1;

      if (y[Gmin_idx] == +1)
        working_set[0] = Gmaxp_idx;
      else
        working_set[0] = Gmaxn_idx;
      working_set[1] = Gmin_idx;

      return 0;
    }

    private bool be_shrunk(int i, double Gmax1, double Gmax2, double Gmax3, double Gmax4) {
      if (is_upper_bound(i)) {
        if (y[i] == +1)
          return (-G[i] > Gmax1);
        else
          return (-G[i] > Gmax4);
      } else if (is_lower_bound(i)) {
        if (y[i] == +1)
          return (G[i] > Gmax2);
        else
          return (G[i] > Gmax3);
      } else
        return (false);
    }

    protected override void do_shrinking() {
      double Gmax1 = -INF;	// max { -y_i * grad(f)_i | y_i = +1, i in I_up(\alpha) }
      double Gmax2 = -INF;	// max { y_i * grad(f)_i | y_i = +1, i in I_low(\alpha) }
      double Gmax3 = -INF;	// max { -y_i * grad(f)_i | y_i = -1, i in I_up(\alpha) }
      double Gmax4 = -INF;	// max { y_i * grad(f)_i | y_i = -1, i in I_low(\alpha) }

      // find maximal violating pair first
      int i;
      for (i = 0; i < active_size; i++) {
        if (!is_upper_bound(i)) {
          if (y[i] == +1) {
            if (-G[i] > Gmax1) Gmax1 = -G[i];
          } else if (-G[i] > Gmax4) Gmax4 = -G[i];
        }
        if (!is_lower_bound(i)) {
          if (y[i] == +1) {
            if (G[i] > Gmax2) Gmax2 = G[i];
          } else if (G[i] > Gmax3) Gmax3 = G[i];
        }
      }

      if (unshrink == false && Math.Max(Gmax1 + Gmax2, Gmax3 + Gmax4) <= eps * 10) {
        unshrink = true;
        reconstruct_gradient();
        active_size = l;
      }

      for (i = 0; i < active_size; i++)
        if (be_shrunk(i, Gmax1, Gmax2, Gmax3, Gmax4)) {
          active_size--;
          while (active_size > i) {
            if (!be_shrunk(active_size, Gmax1, Gmax2, Gmax3, Gmax4)) {
              swap_index(i, active_size);
              break;
            }
            active_size--;
          }
        }
    }

    protected override double calculate_rho() {
      int nr_free1 = 0, nr_free2 = 0;
      double ub1 = INF, ub2 = INF;
      double lb1 = -INF, lb2 = -INF;
      double sum_free1 = 0, sum_free2 = 0;

      for (int i = 0; i < active_size; i++) {
        if (y[i] == +1) {
          if (is_lower_bound(i))
            ub1 = Math.Min(ub1, G[i]);
          else if (is_upper_bound(i))
            lb1 = Math.Max(lb1, G[i]);
          else {
            ++nr_free1;
            sum_free1 += G[i];
          }
        } else {
          if (is_lower_bound(i))
            ub2 = Math.Min(ub2, G[i]);
          else if (is_upper_bound(i))
            lb2 = Math.Max(lb2, G[i]);
          else {
            ++nr_free2;
            sum_free2 += G[i];
          }
        }
      }

      double r1, r2;
      if (nr_free1 > 0)
        r1 = sum_free1 / nr_free1;
      else
        r1 = (ub1 + lb1) / 2;

      if (nr_free2 > 0)
        r2 = sum_free2 / nr_free2;
      else
        r2 = (ub2 + lb2) / 2;

      si.r = (r1 + r2) / 2;
      return (r1 - r2) / 2;
    }
  }

  //
  // Q matrices for various formulations
  //
  class SVC_Q : Kernel {
    private readonly short[] y;
    private readonly Cache cache;
    private readonly double[] QD;

    public SVC_Q(svm_problem prob, svm_parameter param, short[] y_)
      : base(prob.l, prob.x, param) {
      y = (short[])y_.Clone();
      cache = new Cache(prob.l, (long)(param.cache_size * (1 << 20)));
      QD = new double[prob.l];
      for (int i = 0; i < prob.l; i++)
        QD[i] = kernel_function(i, i);
    }

    public override float[] get_Q(int i, int len) {
      float[][] data = new float[1][];
      int start, j;
      if ((start = cache.get_data(i, data, len)) < len) {
        for (j = start; j < len; j++)
          data[0][j] = (float)(y[i] * y[j] * kernel_function(i, j));
      }
      return data[0];
    }

    public override double[] get_QD() {
      return QD;
    }

    public override void swap_index(int i, int j) {
      cache.swap_index(i, j);
      base.swap_index(i, j); { short _ = y[i]; y[i] = y[j]; y[j] = _; } 
    { double _ = QD[i]; QD[i] = QD[j]; QD[j] = _; }
    }
  }

  class ONE_CLASS_Q : Kernel {
    private readonly Cache cache;
    private readonly double[] QD;

    public ONE_CLASS_Q(svm_problem prob, svm_parameter param)
      : base(prob.l, prob.x, param) {
      cache = new Cache(prob.l, (long)(param.cache_size * (1 << 20)));
      QD = new double[prob.l];
      for (int i = 0; i < prob.l; i++)
        QD[i] = kernel_function(i, i);
    }

    public override float[] get_Q(int i, int len) {
      float[][] data = new float[1][];
      int start, j;
      if ((start = cache.get_data(i, data, len)) < len) {
        for (j = start; j < len; j++)
          data[0][j] = (float)kernel_function(i, j);
      }
      return data[0];
    }

    public override double[] get_QD() {
      return QD;
    }

    public override void swap_index(int i, int j) {
      cache.swap_index(i, j);
      base.swap_index(i, j); { double _ = QD[i]; QD[i] = QD[j]; QD[j] = _; }
    }
  }

  class SVR_Q : Kernel {
    private int l;
    private Cache cache;
    private short[] sign;
    private int[] index;
    private int next_buffer;
    private float[][] buffer;
    private readonly double[] QD;

    public SVR_Q(svm_problem prob, svm_parameter param)
      : base(prob.l, prob.x, param) {
      l = prob.l;
      cache = new Cache(l, (long)(param.cache_size * (1 << 20)));
      QD = new double[2 * l];
      sign = new short[2 * l];
      index = new int[2 * l];
      for (int k = 0; k < l; k++) {
        sign[k] = 1;
        sign[k + l] = -1;
        index[k] = k;
        index[k + l] = k;
        QD[k] = kernel_function(k, k);
        QD[k + l] = QD[k];
      }
      buffer = new float[2][];
      buffer[0] = new float[2 * l];
      buffer[1] = new float[2 * l];
      next_buffer = 0;
    }

    public override void swap_index(int i, int j) {
      { short _ = sign[i]; sign[i] = sign[j]; sign[j] = _; }
      { int _ = index[i]; index[i] = index[j]; index[j] = _; }
      { double _ = QD[i]; QD[i] = QD[j]; QD[j] = _; }
    }

    public override float[] get_Q(int i, int len) {
      float[][] data = new float[1][];
      int j, real_i = index[i];
      if (cache.get_data(real_i, data, l) < l) {
        for (j = 0; j < l; j++)
          data[0][j] = (float)kernel_function(real_i, j);
      }

      // reorder and copy
      float[] buf = buffer[next_buffer];
      next_buffer = 1 - next_buffer;
      short si = sign[i];
      for (j = 0; j < len; j++)
        buf[j] = (float)si * sign[j] * data[0][index[j]];
      return buf;
    }

    public override double[] get_QD() {
      return QD;
    }
  }

  public class svm {
    //
    // construct and solve various formulations
    //
    public static readonly int LIBSVM_VERSION = 312;
    public static readonly Random rand = new Random();

    private static Action<string> svm_print_string = (s) => {
      Console.Out.Write(s);
      Console.Out.Flush();
    };

    public static void info(String s) {
      svm_print_string(s);
    }

    private static void solve_c_svc(svm_problem prob, svm_parameter param,
                                    double[] alpha, Solver.SolutionInfo si,
                                    double Cp, double Cn) {
      int l = prob.l;
      double[] minus_ones = new double[l];
      short[] y = new short[l];

      int i;

      for (i = 0; i < l; i++) {
        alpha[i] = 0;
        minus_ones[i] = -1;
        if (prob.y[i] > 0) y[i] = +1;
        else y[i] = -1;
      }

      Solver s = new Solver();
      s.Solve(l, new SVC_Q(prob, param, y), minus_ones, y,
              alpha, Cp, Cn, param.eps, si, param.shrinking);

      double sum_alpha = 0;
      for (i = 0; i < l; i++)
        sum_alpha += alpha[i];

      if (Cp == Cn)
        svm.info("nu = " + sum_alpha / (Cp * prob.l) + Environment.NewLine);

      for (i = 0; i < l; i++)
        alpha[i] *= y[i];
    }

    private static void solve_nu_svc(svm_problem prob, svm_parameter param,
                                     double[] alpha, Solver.SolutionInfo si) {
      int i;
      int l = prob.l;
      double nu = param.nu;

      short[] y = new short[l];

      for (i = 0; i < l; i++)
        if (prob.y[i] > 0)
          y[i] = +1;
        else
          y[i] = -1;

      double sum_pos = nu * l / 2;
      double sum_neg = nu * l / 2;

      for (i = 0; i < l; i++)
        if (y[i] == +1) {
          alpha[i] = Math.Min(1.0, sum_pos);
          sum_pos -= alpha[i];
        } else {
          alpha[i] = Math.Min(1.0, sum_neg);
          sum_neg -= alpha[i];
        }

      double[] zeros = new double[l];

      for (i = 0; i < l; i++)
        zeros[i] = 0;

      Solver_NU s = new Solver_NU();
      s.Solve(l, new SVC_Q(prob, param, y), zeros, y,
              alpha, 1.0, 1.0, param.eps, si, param.shrinking);
      double r = si.r;

      svm.info("C = " + 1 / r + Environment.NewLine);

      for (i = 0; i < l; i++)
        alpha[i] *= y[i] / r;

      si.rho /= r;
      si.obj /= (r * r);
      si.upper_bound_p = 1 / r;
      si.upper_bound_n = 1 / r;
    }

    private static void solve_one_class(svm_problem prob, svm_parameter param,
                                        double[] alpha, Solver.SolutionInfo si) {
      int l = prob.l;
      double[] zeros = new double[l];
      short[] ones = new short[l];
      int i;

      int n = (int)(param.nu * prob.l); // # of alpha's at upper bound

      for (i = 0; i < n; i++)
        alpha[i] = 1;
      if (n < prob.l)
        alpha[n] = param.nu * prob.l - n;
      for (i = n + 1; i < l; i++)
        alpha[i] = 0;

      for (i = 0; i < l; i++) {
        zeros[i] = 0;
        ones[i] = 1;
      }

      Solver s = new Solver();
      s.Solve(l, new ONE_CLASS_Q(prob, param), zeros, ones,
              alpha, 1.0, 1.0, param.eps, si, param.shrinking);
    }

    private static void solve_epsilon_svr(svm_problem prob, svm_parameter param,
                                          double[] alpha, Solver.SolutionInfo si) {
      int l = prob.l;
      double[] alpha2 = new double[2 * l];
      double[] linear_term = new double[2 * l];
      short[] y = new short[2 * l];
      int i;

      for (i = 0; i < l; i++) {
        alpha2[i] = 0;
        linear_term[i] = param.p - prob.y[i];
        y[i] = 1;

        alpha2[i + l] = 0;
        linear_term[i + l] = param.p + prob.y[i];
        y[i + l] = -1;
      }

      Solver s = new Solver();
      s.Solve(2 * l, new SVR_Q(prob, param), linear_term, y,
              alpha2, param.C, param.C, param.eps, si, param.shrinking);

      double sum_alpha = 0;
      for (i = 0; i < l; i++) {
        alpha[i] = alpha2[i] - alpha2[i + l];
        sum_alpha += Math.Abs(alpha[i]);
      }
      svm.info("nu = " + sum_alpha / (param.C * l) + Environment.NewLine);
    }

    private static void solve_nu_svr(svm_problem prob, svm_parameter param,
                                     double[] alpha, Solver.SolutionInfo si) {
      int l = prob.l;
      double C = param.C;
      double[] alpha2 = new double[2 * l];
      double[] linear_term = new double[2 * l];
      short[] y = new short[2 * l];
      int i;

      double sum = C * param.nu * l / 2;
      for (i = 0; i < l; i++) {
        alpha2[i] = alpha2[i + l] = Math.Min(sum, C);
        sum -= alpha2[i];

        linear_term[i] = -prob.y[i];
        y[i] = 1;

        linear_term[i + l] = prob.y[i];
        y[i + l] = -1;
      }

      Solver_NU s = new Solver_NU();
      s.Solve(2 * l, new SVR_Q(prob, param), linear_term, y,
              alpha2, C, C, param.eps, si, param.shrinking);

      svm.info("epsilon = " + (-si.r) + Environment.NewLine);

      for (i = 0; i < l; i++)
        alpha[i] = alpha2[i] - alpha2[i + l];
    }

    //
    // decision_function
    //
    private sealed class decision_function {
      public double[] alpha;
      public double rho;
    };

    private static decision_function svm_train_one(
      svm_problem prob, svm_parameter param,
      double Cp, double Cn) {
      double[] alpha = new double[prob.l];
      Solver.SolutionInfo si = new Solver.SolutionInfo();
      switch (param.svm_type) {
        case svm_parameter.C_SVC:
          solve_c_svc(prob, param, alpha, si, Cp, Cn);
          break;
        case svm_parameter.NU_SVC:
          solve_nu_svc(prob, param, alpha, si);
          break;
        case svm_parameter.ONE_CLASS:
          solve_one_class(prob, param, alpha, si);
          break;
        case svm_parameter.EPSILON_SVR:
          solve_epsilon_svr(prob, param, alpha, si);
          break;
        case svm_parameter.NU_SVR:
          solve_nu_svr(prob, param, alpha, si);
          break;
      }

      svm.info("obj = " + si.obj + ", rho = " + si.rho + Environment.NewLine);

      // output SVs

      int nSV = 0;
      int nBSV = 0;
      for (int i = 0; i < prob.l; i++) {
        if (Math.Abs(alpha[i]) > 0) {
          ++nSV;
          if (prob.y[i] > 0) {
            if (Math.Abs(alpha[i]) >= si.upper_bound_p)
              ++nBSV;
          } else {
            if (Math.Abs(alpha[i]) >= si.upper_bound_n)
              ++nBSV;
          }
        }
      }

      svm.info("nSV = " + nSV + ", nBSV = " + nBSV + Environment.NewLine);

      decision_function f = new decision_function();
      f.alpha = alpha;
      f.rho = si.rho;
      return f;
    }

    // Platt's binary SVM Probablistic Output: an improvement from Lin et al.
    private static void sigmoid_train(int l, double[] dec_values, double[] labels,
                                      double[] probAB) {
      double A, B;
      double prior1 = 0, prior0 = 0;
      int i;

      for (i = 0; i < l; i++)
        if (labels[i] > 0) prior1 += 1;
        else prior0 += 1;

      int max_iter = 100; // Maximal number of iterations
      double min_step = 1e-10; // Minimal step taken in line search
      double sigma = 1e-12; // For numerically strict PD of Hessian
      double eps = 1e-5;
      double hiTarget = (prior1 + 1.0) / (prior1 + 2.0);
      double loTarget = 1 / (prior0 + 2.0);
      double[] t = new double[l];
      double fApB, p, q, h11, h22, h21, g1, g2, det, dA, dB, gd, stepsize;
      double newA, newB, newf, d1, d2;
      int iter;

      // Initial Point and Initial Fun Value
      A = 0.0;
      B = Math.Log((prior0 + 1.0) / (prior1 + 1.0));
      double fval = 0.0;

      for (i = 0; i < l; i++) {
        if (labels[i] > 0) t[i] = hiTarget;
        else t[i] = loTarget;
        fApB = dec_values[i] * A + B;
        if (fApB >= 0)
          fval += t[i] * fApB + Math.Log(1 + Math.Exp(-fApB));
        else
          fval += (t[i] - 1) * fApB + Math.Log(1 + Math.Exp(fApB));
      }
      for (iter = 0; iter < max_iter; iter++) {
        // Update Gradient and Hessian (use H' = H + sigma I)
        h11 = sigma; // numerically ensures strict PD
        h22 = sigma;
        h21 = 0.0;
        g1 = 0.0;
        g2 = 0.0;
        for (i = 0; i < l; i++) {
          fApB = dec_values[i] * A + B;
          if (fApB >= 0) {
            p = Math.Exp(-fApB) / (1.0 + Math.Exp(-fApB));
            q = 1.0 / (1.0 + Math.Exp(-fApB));
          } else {
            p = 1.0 / (1.0 + Math.Exp(fApB));
            q = Math.Exp(fApB) / (1.0 + Math.Exp(fApB));
          }
          d2 = p * q;
          h11 += dec_values[i] * dec_values[i] * d2;
          h22 += d2;
          h21 += dec_values[i] * d2;
          d1 = t[i] - p;
          g1 += dec_values[i] * d1;
          g2 += d1;
        }

        // Stopping Criteria
        if (Math.Abs(g1) < eps && Math.Abs(g2) < eps)
          break;

        // Finding Newton direction: -inv(H') * g
        det = h11 * h22 - h21 * h21;
        dA = -(h22 * g1 - h21 * g2) / det;
        dB = -(-h21 * g1 + h11 * g2) / det;
        gd = g1 * dA + g2 * dB;


        stepsize = 1; // Line Search
        while (stepsize >= min_step) {
          newA = A + stepsize * dA;
          newB = B + stepsize * dB;

          // New function value
          newf = 0.0;
          for (i = 0; i < l; i++) {
            fApB = dec_values[i] * newA + newB;
            if (fApB >= 0)
              newf += t[i] * fApB + Math.Log(1 + Math.Exp(-fApB));
            else
              newf += (t[i] - 1) * fApB + Math.Log(1 + Math.Exp(fApB));
          }
          // Check sufficient decrease
          if (newf < fval + 0.0001 * stepsize * gd) {
            A = newA;
            B = newB;
            fval = newf;
            break;
          } else
            stepsize = stepsize / 2.0;
        }

        if (stepsize < min_step) {
          svm.info("Line search fails in two-class probability estimates" + Environment.NewLine);
          break;
        }
      }

      if (iter >= max_iter)
        svm.info("Reaching maximal iterations in two-class probability estimates" + Environment.NewLine);
      probAB[0] = A;
      probAB[1] = B;
    }

    private static double sigmoid_predict(double decision_value, double A, double B) {
      double fApB = decision_value * A + B;
      if (fApB >= 0)
        return Math.Exp(-fApB) / (1.0 + Math.Exp(-fApB));
      else
        return 1.0 / (1 + Math.Exp(fApB));
    }

    // Method 2 from the multiclass_prob paper by Wu, Lin, and Weng
    private static void multiclass_probability(int k, double[][] r, double[] p) {
      int t, j;
      int iter = 0, max_iter = Math.Max(100, k);
      double[][] Q = new double[k][];
      double[] Qp = new double[k];
      double pQp, eps = 0.005 / k;

      for (t = 0; t < k; t++) {
        Q[t] = new double[k];
        p[t] = 1.0 / k; // Valid if k = 1
        Q[t][t] = 0;
        for (j = 0; j < t; j++) {
          Q[t][t] += r[j][t] * r[j][t];
          Q[t][j] = Q[j][t];
        }
        for (j = t + 1; j < k; j++) {
          Q[t][t] += r[j][t] * r[j][t];
          Q[t][j] = -r[j][t] * r[t][j];
        }
      }
      for (iter = 0; iter < max_iter; iter++) {
        // stopping condition, recalculate QP,pQP for numerical accuracy
        pQp = 0;
        for (t = 0; t < k; t++) {
          Qp[t] = 0;
          for (j = 0; j < k; j++)
            Qp[t] += Q[t][j] * p[j];
          pQp += p[t] * Qp[t];
        }
        double max_error = 0;
        for (t = 0; t < k; t++) {
          double error = Math.Abs(Qp[t] - pQp);
          if (error > max_error)
            max_error = error;
        }
        if (max_error < eps) break;

        for (t = 0; t < k; t++) {
          double diff = (-Qp[t] + pQp) / Q[t][t];
          p[t] += diff;
          pQp = (pQp + diff * (diff * Q[t][t] + 2 * Qp[t])) / (1 + diff) / (1 + diff);
          for (j = 0; j < k; j++) {
            Qp[j] = (Qp[j] + diff * Q[t][j]) / (1 + diff);
            p[j] /= (1 + diff);
          }
        }
      }
      if (iter >= max_iter)
        svm.info("Exceeds max_iter in multiclass_prob" + Environment.NewLine);
    }

    // Cross-validation decision values for probability estimates
    private static void svm_binary_svc_probability(svm_problem prob, svm_parameter param, double Cp, double Cn,
                                                   double[] probAB) {
      int i;
      int nr_fold = 5;
      int[] perm = new int[prob.l];
      double[] dec_values = new double[prob.l];

      // random shuffle
      for (i = 0; i < prob.l; i++) perm[i] = i;
      for (i = 0; i < prob.l; i++) {
        int j = i + rand.Next(prob.l - i);
        {
          int _ = perm[i];
          perm[i] = perm[j];
          perm[j] = _;
        }
      }
      for (i = 0; i < nr_fold; i++) {
        int begin = i * prob.l / nr_fold;
        int end = (i + 1) * prob.l / nr_fold;
        int j, k;
        svm_problem subprob = new svm_problem();

        subprob.l = prob.l - (end - begin);
        subprob.x = new svm_node[subprob.l][];
        subprob.y = new double[subprob.l];

        k = 0;
        for (j = 0; j < begin; j++) {
          subprob.x[k] = prob.x[perm[j]];
          subprob.y[k] = prob.y[perm[j]];
          ++k;
        }
        for (j = end; j < prob.l; j++) {
          subprob.x[k] = prob.x[perm[j]];
          subprob.y[k] = prob.y[perm[j]];
          ++k;
        }
        int p_count = 0, n_count = 0;
        for (j = 0; j < k; j++)
          if (subprob.y[j] > 0)
            p_count++;
          else
            n_count++;

        if (p_count == 0 && n_count == 0)
          for (j = begin; j < end; j++)
            dec_values[perm[j]] = 0;
        else if (p_count > 0 && n_count == 0)
          for (j = begin; j < end; j++)
            dec_values[perm[j]] = 1;
        else if (p_count == 0 && n_count > 0)
          for (j = begin; j < end; j++)
            dec_values[perm[j]] = -1;
        else {
          svm_parameter subparam = (svm_parameter)param.Clone();
          subparam.probability = 0;
          subparam.C = 1.0;
          subparam.nr_weight = 2;
          subparam.weight_label = new int[2];
          subparam.weight = new double[2];
          subparam.weight_label[0] = +1;
          subparam.weight_label[1] = -1;
          subparam.weight[0] = Cp;
          subparam.weight[1] = Cn;
          svm_model submodel = svm_train(subprob, subparam);
          for (j = begin; j < end; j++) {
            double[] dec_value = new double[1];
            svm_predict_values(submodel, prob.x[perm[j]], dec_value);
            dec_values[perm[j]] = dec_value[0];
            // ensure +1 -1 order; reason not using CV subroutine
            dec_values[perm[j]] *= submodel.label[0];
          }
        }
      }
      sigmoid_train(prob.l, dec_values, prob.y, probAB);
    }

    // Return parameter of a Laplace distribution 
    private static double svm_svr_probability(svm_problem prob, svm_parameter param) {
      int i;
      int nr_fold = 5;
      double[] ymv = new double[prob.l];
      double mae = 0;

      svm_parameter newparam = (svm_parameter)param.Clone();
      newparam.probability = 0;
      svm_cross_validation(prob, newparam, nr_fold, ymv);
      for (i = 0; i < prob.l; i++) {
        ymv[i] = prob.y[i] - ymv[i];
        mae += Math.Abs(ymv[i]);
      }
      mae /= prob.l;
      double std = Math.Sqrt(2 * mae * mae);
      int count = 0;
      mae = 0;
      for (i = 0; i < prob.l; i++)
        if (Math.Abs(ymv[i]) > 5 * std)
          count = count + 1;
        else
          mae += Math.Abs(ymv[i]);
      mae /= (prob.l - count);
      svm.info("Prob. model for test data: target value = predicted value + z, " + Environment.NewLine
               + "z: Laplace distribution e^(-|z|/sigma)/(2sigma),sigma=" + mae + Environment.NewLine);
      return mae;
    }

    // label: label name, start: begin of each class, count: #data of classes, perm: indices to the original data
    // perm, length l, must be allocated before calling this subroutine
    private static void svm_group_classes(svm_problem prob, int[] nr_class_ret, int[][] label_ret, int[][] start_ret,
                                          int[][] count_ret, int[] perm) {
      int l = prob.l;
      int max_nr_class = 16;
      int nr_class = 0;
      int[] label = new int[max_nr_class];
      int[] count = new int[max_nr_class];
      int[] data_label = new int[l];
      int i;

      for (i = 0; i < l; i++) {
        int this_label = (int)(prob.y[i]);
        int j;
        for (j = 0; j < nr_class; j++) {
          if (this_label == label[j]) {
            ++count[j];
            break;
          }
        }
        data_label[i] = j;
        if (j == nr_class) {
          if (nr_class == max_nr_class) {
            max_nr_class *= 2;
            int[] new_data = new int[max_nr_class];
            Array.Copy(label, 0, new_data, 0, label.Length);
            label = new_data;
            new_data = new int[max_nr_class];
            Array.Copy(count, 0, new_data, 0, count.Length);
            count = new_data;
          }
          label[nr_class] = this_label;
          count[nr_class] = 1;
          ++nr_class;
        }
      }

      int[] start = new int[nr_class];
      start[0] = 0;
      for (i = 1; i < nr_class; i++)
        start[i] = start[i - 1] + count[i - 1];
      for (i = 0; i < l; i++) {
        perm[start[data_label[i]]] = i;
        ++start[data_label[i]];
      }
      start[0] = 0;
      for (i = 1; i < nr_class; i++)
        start[i] = start[i - 1] + count[i - 1];

      nr_class_ret[0] = nr_class;
      label_ret[0] = label;
      start_ret[0] = start;
      count_ret[0] = count;
    }

    //
    // Interface functions
    //
    public static svm_model svm_train(svm_problem prob, svm_parameter param) {
      svm_model model = new svm_model();
      model.param = param;

      if (param.svm_type == svm_parameter.ONE_CLASS ||
          param.svm_type == svm_parameter.EPSILON_SVR ||
          param.svm_type == svm_parameter.NU_SVR) {
        // regression or one-class-svm
        model.nr_class = 2;
        model.label = null;
        model.nSV = null;
        model.probA = null;
        model.probB = null;
        model.sv_coef = new double[1][];

        if (param.probability == 1 &&
            (param.svm_type == svm_parameter.EPSILON_SVR ||
             param.svm_type == svm_parameter.NU_SVR)) {
          model.probA = new double[1];
          model.probA[0] = svm_svr_probability(prob, param);
        }

        decision_function f = svm_train_one(prob, param, 0, 0);
        model.rho = new double[1];
        model.rho[0] = f.rho;

        int nSV = 0;
        int i;
        for (i = 0; i < prob.l; i++)
          if (Math.Abs(f.alpha[i]) > 0) ++nSV;
        model.l = nSV;
        model.SV = new svm_node[nSV][];
        model.sv_coef[0] = new double[nSV];
        int j = 0;
        for (i = 0; i < prob.l; i++)
          if (Math.Abs(f.alpha[i]) > 0) {
            model.SV[j] = prob.x[i];
            model.sv_coef[0][j] = f.alpha[i];
            ++j;
          }
      } else {
        // classification
        int l = prob.l;
        int[] tmp_nr_class = new int[1];
        int[][] tmp_label = new int[1][];
        int[][] tmp_start = new int[1][];
        int[][] tmp_count = new int[1][];
        int[] perm = new int[l];

        // group training data of the same class
        svm_group_classes(prob, tmp_nr_class, tmp_label, tmp_start, tmp_count, perm);
        int nr_class = tmp_nr_class[0];
        int[] label = tmp_label[0];
        int[] start = tmp_start[0];
        int[] count = tmp_count[0];

        if (nr_class == 1)
          svm.info("WARNING: training data in only one class. See README for details." + Environment.NewLine);

        svm_node[][] x = new svm_node[l][];
        int i;
        for (i = 0; i < l; i++)
          x[i] = prob.x[perm[i]];

        // calculate weighted C

        double[] weighted_C = new double[nr_class];
        for (i = 0; i < nr_class; i++)
          weighted_C[i] = param.C;
        for (i = 0; i < param.nr_weight; i++) {
          int j;
          for (j = 0; j < nr_class; j++)
            if (param.weight_label[i] == label[j])
              break;
          if (j == nr_class)
            Console.Error.WriteLine("WARNING: class label " + param.weight_label[i] +
                                    " specified in weight is not found");
          else
            weighted_C[j] *= param.weight[i];
        }

        // train k*(k-1)/2 models

        bool[] nonzero = new bool[l];
        for (i = 0; i < l; i++)
          nonzero[i] = false;
        decision_function[] f = new decision_function[nr_class * (nr_class - 1) / 2];

        double[] probA = null, probB = null;
        if (param.probability == 1) {
          probA = new double[nr_class * (nr_class - 1) / 2];
          probB = new double[nr_class * (nr_class - 1) / 2];
        }

        int p = 0;
        for (i = 0; i < nr_class; i++)
          for (int j = i + 1; j < nr_class; j++) {
            svm_problem sub_prob = new svm_problem();
            int si = start[i], sj = start[j];
            int ci = count[i], cj = count[j];
            sub_prob.l = ci + cj;
            sub_prob.x = new svm_node[sub_prob.l][];
            sub_prob.y = new double[sub_prob.l];
            int k;
            for (k = 0; k < ci; k++) {
              sub_prob.x[k] = x[si + k];
              sub_prob.y[k] = +1;
            }
            for (k = 0; k < cj; k++) {
              sub_prob.x[ci + k] = x[sj + k];
              sub_prob.y[ci + k] = -1;
            }

            if (param.probability == 1) {
              double[] probAB = new double[2];
              svm_binary_svc_probability(sub_prob, param, weighted_C[i], weighted_C[j], probAB);
              probA[p] = probAB[0];
              probB[p] = probAB[1];
            }

            f[p] = svm_train_one(sub_prob, param, weighted_C[i], weighted_C[j]);
            for (k = 0; k < ci; k++)
              if (!nonzero[si + k] && Math.Abs(f[p].alpha[k]) > 0)
                nonzero[si + k] = true;
            for (k = 0; k < cj; k++)
              if (!nonzero[sj + k] && Math.Abs(f[p].alpha[ci + k]) > 0)
                nonzero[sj + k] = true;
            ++p;
          }

        // build output

        model.nr_class = nr_class;

        model.label = new int[nr_class];
        for (i = 0; i < nr_class; i++)
          model.label[i] = label[i];

        model.rho = new double[nr_class * (nr_class - 1) / 2];
        for (i = 0; i < nr_class * (nr_class - 1) / 2; i++)
          model.rho[i] = f[i].rho;

        if (param.probability == 1) {
          model.probA = new double[nr_class * (nr_class - 1) / 2];
          model.probB = new double[nr_class * (nr_class - 1) / 2];
          for (i = 0; i < nr_class * (nr_class - 1) / 2; i++) {
            model.probA[i] = probA[i];
            model.probB[i] = probB[i];
          }
        } else {
          model.probA = null;
          model.probB = null;
        }

        int nnz = 0;
        int[] nz_count = new int[nr_class];
        model.nSV = new int[nr_class];
        for (i = 0; i < nr_class; i++) {
          int nSV = 0;
          for (int j = 0; j < count[i]; j++)
            if (nonzero[start[i] + j]) {
              ++nSV;
              ++nnz;
            }
          model.nSV[i] = nSV;
          nz_count[i] = nSV;
        }

        svm.info("Total nSV = " + nnz + Environment.NewLine);

        model.l = nnz;
        model.SV = new svm_node[nnz][];
        p = 0;
        for (i = 0; i < l; i++)
          if (nonzero[i]) model.SV[p++] = x[i];

        int[] nz_start = new int[nr_class];
        nz_start[0] = 0;
        for (i = 1; i < nr_class; i++)
          nz_start[i] = nz_start[i - 1] + nz_count[i - 1];

        model.sv_coef = new double[nr_class - 1][];
        for (i = 0; i < nr_class - 1; i++)
          model.sv_coef[i] = new double[nnz];

        p = 0;
        for (i = 0; i < nr_class; i++)
          for (int j = i + 1; j < nr_class; j++) {
            // classifier (i,j): coefficients with
            // i are in sv_coef[j-1][nz_start[i]...],
            // j are in sv_coef[i][nz_start[j]...]

            int si = start[i];
            int sj = start[j];
            int ci = count[i];
            int cj = count[j];

            int q = nz_start[i];
            int k;
            for (k = 0; k < ci; k++)
              if (nonzero[si + k])
                model.sv_coef[j - 1][q++] = f[p].alpha[k];
            q = nz_start[j];
            for (k = 0; k < cj; k++)
              if (nonzero[sj + k])
                model.sv_coef[i][q++] = f[p].alpha[ci + k];
            ++p;
          }
      }
      return model;
    }

    // Stratified cross validation
    public static void svm_cross_validation(svm_problem prob, svm_parameter param, int nr_fold, double[] target) {
      int i;
      int[] fold_start = new int[nr_fold + 1];
      int l = prob.l;
      int[] perm = new int[l];

      // stratified cv may not give leave-one-out rate
      // Each class to l folds -> some folds may have zero elements
      if ((param.svm_type == svm_parameter.C_SVC ||
           param.svm_type == svm_parameter.NU_SVC) && nr_fold < l) {
        int[] tmp_nr_class = new int[1];
        int[][] tmp_label = new int[1][];
        int[][] tmp_start = new int[1][];
        int[][] tmp_count = new int[1][];

        svm_group_classes(prob, tmp_nr_class, tmp_label, tmp_start, tmp_count, perm);

        int nr_class = tmp_nr_class[0];
        int[] start = tmp_start[0];
        int[] count = tmp_count[0];

        // random shuffle and then data grouped by fold using the array perm
        int[] fold_count = new int[nr_fold];
        int c;
        int[] index = new int[l];
        for (i = 0; i < l; i++)
          index[i] = perm[i];
        for (c = 0; c < nr_class; c++)
          for (i = 0; i < count[c]; i++) {
            int j = i + rand.Next(count[c] - i);
            {
              int _ = index[start[c] + j];
              index[start[c] + j] = index[start[c] + i];
              index[start[c] + i] = _;
            }
          }
        for (i = 0; i < nr_fold; i++) {
          fold_count[i] = 0;
          for (c = 0; c < nr_class; c++)
            fold_count[i] += (i + 1) * count[c] / nr_fold - i * count[c] / nr_fold;
        }
        fold_start[0] = 0;
        for (i = 1; i <= nr_fold; i++)
          fold_start[i] = fold_start[i - 1] + fold_count[i - 1];
        for (c = 0; c < nr_class; c++)
          for (i = 0; i < nr_fold; i++) {
            int begin = start[c] + i * count[c] / nr_fold;
            int end = start[c] + (i + 1) * count[c] / nr_fold;
            for (int j = begin; j < end; j++) {
              perm[fold_start[i]] = index[j];
              fold_start[i]++;
            }
          }
        fold_start[0] = 0;
        for (i = 1; i <= nr_fold; i++)
          fold_start[i] = fold_start[i - 1] + fold_count[i - 1];
      } else {
        for (i = 0; i < l; i++) perm[i] = i;
        for (i = 0; i < l; i++) {
          int j = i + rand.Next(l - i);
          {
            int _ = perm[i];
            perm[i] = perm[j];
            perm[j] = _;
          }
        }
        for (i = 0; i <= nr_fold; i++)
          fold_start[i] = i * l / nr_fold;
      }

      for (i = 0; i < nr_fold; i++) {
        int begin = fold_start[i];
        int end = fold_start[i + 1];
        int j, k;
        svm_problem subprob = new svm_problem();

        subprob.l = l - (end - begin);
        subprob.x = new svm_node[subprob.l][];
        subprob.y = new double[subprob.l];

        k = 0;
        for (j = 0; j < begin; j++) {
          subprob.x[k] = prob.x[perm[j]];
          subprob.y[k] = prob.y[perm[j]];
          ++k;
        }
        for (j = end; j < l; j++) {
          subprob.x[k] = prob.x[perm[j]];
          subprob.y[k] = prob.y[perm[j]];
          ++k;
        }
        svm_model submodel = svm_train(subprob, param);
        if (param.probability == 1 &&
            (param.svm_type == svm_parameter.C_SVC ||
             param.svm_type == svm_parameter.NU_SVC)) {
          double[] prob_estimates = new double[svm_get_nr_class(submodel)];
          for (j = begin; j < end; j++)
            target[perm[j]] = svm_predict_probability(submodel, prob.x[perm[j]], prob_estimates);
        } else
          for (j = begin; j < end; j++)
            target[perm[j]] = svm_predict(submodel, prob.x[perm[j]]);
      }
    }

    public static int svm_get_svm_type(svm_model model) {
      return model.param.svm_type;
    }

    public static int svm_get_nr_class(svm_model model) {
      return model.nr_class;
    }

    public static void svm_get_labels(svm_model model, int[] label) {
      if (model.label != null)
        for (int i = 0; i < model.nr_class; i++)
          label[i] = model.label[i];
    }

    public static double svm_get_svr_probability(svm_model model) {
      if ((model.param.svm_type == svm_parameter.EPSILON_SVR || model.param.svm_type == svm_parameter.NU_SVR) &&
          model.probA != null)
        return model.probA[0];
      else {
        Console.Error.WriteLine("Model doesn't contain information for SVR probability inference");
        return 0;
      }
    }

    public static double svm_predict_values(svm_model model, svm_node[] x, double[] dec_values) {
      int i;
      if (model.param.svm_type == svm_parameter.ONE_CLASS ||
          model.param.svm_type == svm_parameter.EPSILON_SVR ||
          model.param.svm_type == svm_parameter.NU_SVR) {
        double[] sv_coef = model.sv_coef[0];
        double sum = 0;
        for (i = 0; i < model.l; i++)
          sum += sv_coef[i] * Kernel.k_function(x, model.SV[i], model.param);
        sum -= model.rho[0];
        dec_values[0] = sum;

        if (model.param.svm_type == svm_parameter.ONE_CLASS)
          return (sum > 0) ? 1 : -1;
        else
          return sum;
      } else {
        int nr_class = model.nr_class;
        int l = model.l;

        double[] kvalue = new double[l];
        for (i = 0; i < l; i++)
          kvalue[i] = Kernel.k_function(x, model.SV[i], model.param);

        int[] start = new int[nr_class];
        start[0] = 0;
        for (i = 1; i < nr_class; i++)
          start[i] = start[i - 1] + model.nSV[i - 1];

        int[] vote = new int[nr_class];
        for (i = 0; i < nr_class; i++)
          vote[i] = 0;

        int p = 0;
        for (i = 0; i < nr_class; i++)
          for (int j = i + 1; j < nr_class; j++) {
            double sum = 0;
            int si = start[i];
            int sj = start[j];
            int ci = model.nSV[i];
            int cj = model.nSV[j];

            int k;
            double[] coef1 = model.sv_coef[j - 1];
            double[] coef2 = model.sv_coef[i];
            for (k = 0; k < ci; k++)
              sum += coef1[si + k] * kvalue[si + k];
            for (k = 0; k < cj; k++)
              sum += coef2[sj + k] * kvalue[sj + k];
            sum -= model.rho[p];
            dec_values[p] = sum;

            if (dec_values[p] > 0)
              ++vote[i];
            else
              ++vote[j];
            p++;
          }

        int vote_max_idx = 0;
        for (i = 1; i < nr_class; i++)
          if (vote[i] > vote[vote_max_idx])
            vote_max_idx = i;

        return model.label[vote_max_idx];
      }
    }

    public static double svm_predict(svm_model model, svm_node[] x) {
      int nr_class = model.nr_class;
      double[] dec_values;
      if (model.param.svm_type == svm_parameter.ONE_CLASS ||
          model.param.svm_type == svm_parameter.EPSILON_SVR ||
          model.param.svm_type == svm_parameter.NU_SVR)
        dec_values = new double[1];
      else
        dec_values = new double[nr_class * (nr_class - 1) / 2];
      double pred_result = svm_predict_values(model, x, dec_values);
      return pred_result;
    }

    public static double svm_predict_probability(svm_model model, svm_node[] x, double[] prob_estimates) {
      if ((model.param.svm_type == svm_parameter.C_SVC || model.param.svm_type == svm_parameter.NU_SVC) &&
          model.probA != null && model.probB != null) {
        int i;
        int nr_class = model.nr_class;
        double[] dec_values = new double[nr_class * (nr_class - 1) / 2];
        svm_predict_values(model, x, dec_values);

        double min_prob = 1e-7;
        double[][] pairwise_prob = new double[nr_class][];

        int k = 0;
        for (i = 0; i < nr_class; i++)
          pairwise_prob[i] = new double[nr_class];
        for (int j = i + 1; j < nr_class; j++) {
          pairwise_prob[i][j] =
            Math.Min(Math.Max(sigmoid_predict(dec_values[k], model.probA[k], model.probB[k]), min_prob), 1 - min_prob);
          pairwise_prob[j][i] = 1 - pairwise_prob[i][j];
          k++;
        }
        multiclass_probability(nr_class, pairwise_prob, prob_estimates);

        int prob_max_idx = 0;
        for (i = 1; i < nr_class; i++)
          if (prob_estimates[i] > prob_estimates[prob_max_idx])
            prob_max_idx = i;
        return model.label[prob_max_idx];
      } else
        return svm_predict(model, x);
    }

    private static readonly string[] svm_type_table = new string[]
                                                        {
                                                          "c_svc", "nu_svc", "one_class", "epsilon_svr", "nu_svr",
                                                        };

    private static readonly string[] kernel_type_table = new string[]
                                                           {
                                                             "linear", "polynomial", "rbf", "sigmoid", "precomputed"
                                                           };


    public static void svm_save_model(string model_file_name, svm_model model) {
      //DataOutputStream fp = new DataOutputStream(new BufferedOutputStream(new FileOutputStream(model_file_name)));
      var writer = new StreamWriter(model_file_name);
      svm_save_model(writer, model);
    }


    public static void svm_save_model(StreamWriter writer, svm_model model) {

      var savedCulture = Thread.CurrentThread.CurrentCulture;
      Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
      svm_parameter param = model.param;

      writer.Write("svm_type " + svm_type_table[param.svm_type] + Environment.NewLine);
      writer.Write("kernel_type " + kernel_type_table[param.kernel_type] + Environment.NewLine);

      if (param.kernel_type == svm_parameter.POLY)
        writer.Write("degree " + param.degree + Environment.NewLine);

      if (param.kernel_type == svm_parameter.POLY ||
         param.kernel_type == svm_parameter.RBF ||
         param.kernel_type == svm_parameter.SIGMOID)
        writer.Write("gamma " + param.gamma.ToString("r") + Environment.NewLine);

      if (param.kernel_type == svm_parameter.POLY ||
         param.kernel_type == svm_parameter.SIGMOID)
        writer.Write("coef0 " + param.coef0.ToString("r") + Environment.NewLine);

      int nr_class = model.nr_class;
      int l = model.l;
      writer.Write("nr_class " + nr_class + Environment.NewLine);
      writer.Write("total_sv " + l + Environment.NewLine);

      {
        writer.Write("rho");
        for (int i = 0; i < nr_class * (nr_class - 1) / 2; i++)
          writer.Write(" " + model.rho[i].ToString("r"));
        writer.Write(Environment.NewLine);
      }

      if (model.label != null) {
        writer.Write("label");
        for (int i = 0; i < nr_class; i++)
          writer.Write(" " + model.label[i]);
        writer.Write(Environment.NewLine);
      }

      if (model.probA != null) // regression has probA only
      {
        writer.Write("probA");
        for (int i = 0; i < nr_class * (nr_class - 1) / 2; i++)
          writer.Write(" " + model.probA[i].ToString("r"));
        writer.Write(Environment.NewLine);
      }
      if (model.probB != null) {
        writer.Write("probB");
        for (int i = 0; i < nr_class * (nr_class - 1) / 2; i++)
          writer.Write(" " + model.probB[i].ToString("r"));
        writer.Write(Environment.NewLine);
      }

      if (model.nSV != null) {
        writer.Write("nr_sv");
        for (int i = 0; i < nr_class; i++)
          writer.Write(" " + model.nSV[i]);
        writer.Write(Environment.NewLine);
      }

      writer.WriteLine("SV");
      double[][] sv_coef = model.sv_coef;
      svm_node[][] SV = model.SV;

      for (int i = 0; i < l; i++) {
        for (int j = 0; j < nr_class - 1; j++)
          writer.Write(sv_coef[j][i].ToString("r") + " ");

        svm_node[] p = SV[i];
        if (param.kernel_type == svm_parameter.PRECOMPUTED)
          writer.Write("0:" + (int)(p[0].value));
        else
          for (int j = 0; j < p.Length; j++)
            writer.Write(p[j].index + ":" + p[j].value.ToString("r") + " ");
        writer.Write(Environment.NewLine);
      }

      writer.Flush();
      Thread.CurrentThread.CurrentCulture = savedCulture;
    }

    private static double atof(String s) {
      return double.Parse(s);
    }

    private static int atoi(String s) {
      return int.Parse(s);
    }


    public static svm_model svm_load_model(String model_file_name) {
      return svm_load_model(new StreamReader(model_file_name));
    }

    public static svm_model svm_load_model(StreamReader reader) {
      var savedCulture = Thread.CurrentThread.CurrentCulture;
      Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

      // read parameters

      svm_model model = new svm_model();
      svm_parameter param = new svm_parameter();
      model.param = param;
      model.rho = null;
      model.probA = null;
      model.probB = null;
      model.label = null;
      model.nSV = null;

      while (true) {
        String cmd = reader.ReadLine();
        String arg = cmd.Substring(cmd.IndexOf(' ') + 1);

        if (cmd.StartsWith("svm_type")) {
          int i;
          for (i = 0; i < svm_type_table.Length; i++) {
            if (arg.IndexOf(svm_type_table[i], StringComparison.InvariantCultureIgnoreCase) != -1) {
              param.svm_type = i;
              break;
            }
          }
          if (i == svm_type_table.Length) {
            Console.Error.WriteLine("unknown svm type.");
            return null;
          }
        } else if (cmd.StartsWith("kernel_type")) {
          int i;
          for (i = 0; i < kernel_type_table.Length; i++) {
            if (arg.IndexOf(kernel_type_table[i], StringComparison.InvariantCultureIgnoreCase) != -1) {
              param.kernel_type = i;
              break;
            }
          }
          if (i == kernel_type_table.Length) {
            Console.Error.WriteLine("unknown kernel function.");
            return null;
          }
        } else if (cmd.StartsWith("degree"))
          param.degree = atoi(arg);
        else if (cmd.StartsWith("gamma"))
          param.gamma = atof(arg);
        else if (cmd.StartsWith("coef0"))
          param.coef0 = atof(arg);
        else if (cmd.StartsWith("nr_class"))
          model.nr_class = atoi(arg);
        else if (cmd.StartsWith("total_sv"))
          model.l = atoi(arg);
        else if (cmd.StartsWith("rho")) {
          int n = model.nr_class * (model.nr_class - 1) / 2;
          model.rho = new double[n];
          var st = arg.Split(' ', '\t', '\n', '\r', '\f');
          for (int i = 0; i < n; i++)
            model.rho[i] = atof(st[i]);
        } else if (cmd.StartsWith("label")) {
          int n = model.nr_class;
          model.label = new int[n];
          var st = arg.Split(' ', '\t', '\n', '\r', '\f');
          for (int i = 0; i < n; i++)
            model.label[i] = atoi(st[i]);
        } else if (cmd.StartsWith("probA")) {
          int n = model.nr_class * (model.nr_class - 1) / 2;
          model.probA = new double[n];
          var st = arg.Split(' ', '\t', '\n', '\r', '\f');
          for (int i = 0; i < n; i++)
            model.probA[i] = atof(st[i]);
        } else if (cmd.StartsWith("probB")) {
          int n = model.nr_class * (model.nr_class - 1) / 2;
          model.probB = new double[n];
          var st = arg.Split(' ', '\t', '\n', '\r', '\f');
          for (int i = 0; i < n; i++)
            model.probB[i] = atof(st[i]);
        } else if (cmd.StartsWith("nr_sv")) {
          int n = model.nr_class;
          model.nSV = new int[n];
          var st = arg.Split(' ', '\t', '\n', '\r', '\f');
          for (int i = 0; i < n; i++)
            model.nSV[i] = atoi(st[i]);
        } else if (cmd.StartsWith("SV")) {
          break;
        } else {
          Console.Error.WriteLine("unknown text in model file: [" + cmd + "]");
          return null;
        }
      }

      // read sv_coef and SV

      int m = model.nr_class - 1;
      int l = model.l;
      model.sv_coef = new double[m][];
      for (int k = 0; k < m; k++)
        model.sv_coef[k] = new double[l];

      model.SV = new svm_node[l][];


      for (int i = 0; i < l; i++) {
        String line = reader.ReadLine();
        var st = line.Split(' ', '\t', '\n', '\r', '\f', ':');

        for (int k = 0; k < m; k++) {
          model.sv_coef[k][i] = atof(st[k]);
        }
        // skip y value
        st = st.Skip(1).ToArray();

        int n = st.Length / 2;
        model.SV[i] = new svm_node[n];
        for (int j = 0; j < n; j++) {
          model.SV[i][j] = new svm_node();
          model.SV[i][j].index = atoi(st[2 * j]);
          model.SV[i][j].value = atof(st[2 * j + 1]);
        }
      }

      Thread.CurrentThread.CurrentCulture = savedCulture;
      return model;
    }

    public static string svm_check_parameter(svm_problem prob, svm_parameter param) {
      // svm_type

      int svm_type = param.svm_type;
      if (svm_type != svm_parameter.C_SVC &&
         svm_type != svm_parameter.NU_SVC &&
         svm_type != svm_parameter.ONE_CLASS &&
         svm_type != svm_parameter.EPSILON_SVR &&
         svm_type != svm_parameter.NU_SVR)
        return "unknown svm type";

      // kernel_type, degree

      int kernel_type = param.kernel_type;
      if (kernel_type != svm_parameter.LINEAR &&
         kernel_type != svm_parameter.POLY &&
         kernel_type != svm_parameter.RBF &&
         kernel_type != svm_parameter.SIGMOID &&
         kernel_type != svm_parameter.PRECOMPUTED)
        return "unknown kernel type";

      if (param.gamma < 0)
        return "gamma < 0";

      if (param.degree < 0)
        return "degree of polynomial kernel < 0";

      // cache_size,eps,C,nu,p,shrinking

      if (param.cache_size <= 0)
        return "cache_size <= 0";

      if (param.eps <= 0)
        return "eps <= 0";

      if (svm_type == svm_parameter.C_SVC ||
         svm_type == svm_parameter.EPSILON_SVR ||
         svm_type == svm_parameter.NU_SVR)
        if (param.C <= 0)
          return "C <= 0";

      if (svm_type == svm_parameter.NU_SVC ||
         svm_type == svm_parameter.ONE_CLASS ||
         svm_type == svm_parameter.NU_SVR)
        if (param.nu <= 0 || param.nu > 1)
          return "nu <= 0 or nu > 1";

      if (svm_type == svm_parameter.EPSILON_SVR)
        if (param.p < 0)
          return "p < 0";

      if (param.shrinking != 0 &&
         param.shrinking != 1)
        return "shrinking != 0 and shrinking != 1";

      if (param.probability != 0 &&
         param.probability != 1)
        return "probability != 0 and probability != 1";

      if (param.probability == 1 &&
         svm_type == svm_parameter.ONE_CLASS)
        return "one-class SVM probability output not supported yet";

      // check whether nu-svc is feasible

      if (svm_type == svm_parameter.NU_SVC) {
        int l = prob.l;
        int max_nr_class = 16;
        int nr_class = 0;
        int[] label = new int[max_nr_class];
        int[] count = new int[max_nr_class];

        int i;
        for (i = 0; i < l; i++) {
          int this_label = (int)prob.y[i];
          int j;
          for (j = 0; j < nr_class; j++)
            if (this_label == label[j]) {
              ++count[j];
              break;
            }

          if (j == nr_class) {
            if (nr_class == max_nr_class) {
              max_nr_class *= 2;
              int[] new_data = new int[max_nr_class];
              Array.Copy(label, 0, new_data, 0, label.Length);
              label = new_data;

              new_data = new int[max_nr_class];
              Array.Copy(count, 0, new_data, 0, count.Length);
              count = new_data;
            }
            label[nr_class] = this_label;
            count[nr_class] = 1;
            ++nr_class;
          }
        }

        for (i = 0; i < nr_class; i++) {
          int n1 = count[i];
          for (int j = i + 1; j < nr_class; j++) {
            int n2 = count[j];
            if (param.nu * (n1 + n2) / 2 > Math.Min(n1, n2))
              return "specified nu is infeasible";
          }
        }
      }

      return null;
    }

    public static int svm_check_probability_model(svm_model model) {
      if (((model.param.svm_type == svm_parameter.C_SVC || model.param.svm_type == svm_parameter.NU_SVC) &&
      model.probA != null && model.probB != null) ||
      ((model.param.svm_type == svm_parameter.EPSILON_SVR || model.param.svm_type == svm_parameter.NU_SVR) &&
       model.probA != null))
        return 1;
      else
        return 0;
    }

    public static void svm_set_print_string_function(Action<string> print_func) {
      /*if (print_func == null)
        svm_print_string = svm_print_stdout;
      else
        svm_print_string = print_func;
       */
      if (print_func != null) svm_print_string = print_func;
    }
  }

  public class svm_node : ICloneable {
    public int index;
    public double value;
    public object Clone() {
      var clone = new svm_node();
      clone.index = index;
      clone.value = value;
      return clone;
    }
  }

  public class svm_model {
    public svm_parameter param;	// parameter
    public int nr_class;		// number of classes, = 2 in regression/one class svm
    public int l;			// total #SV
    public svm_node[][] SV;	// SVs (SV[l])
    public double[][] sv_coef;	// coefficients for SVs in decision functions (sv_coef[k-1][l])
    public double[] rho;		// constants in decision functions (rho[k*(k-1)/2])
    public double[] probA;         // pariwise probability information
    public double[] probB;

    // for classification only

    public int[] label;		// label of each class (label[k])
    public int[] nSV;		// number of SVs for each class (nSV[k])
    // nSV[0] + nSV[1] + ... + nSV[k-1] = l
  };

  public class svm_problem {
    public int l;
    public double[] y;
    public svm_node[][] x;
  }

  public interface svm_print_interface {
    void print(String s);
  }

  public class svm_parameter : ICloneable {
    /* svm_type */
    public const int C_SVC = 0;
    public const int NU_SVC = 1;
    public const int ONE_CLASS = 2;
    public const int EPSILON_SVR = 3;
    public const int NU_SVR = 4;

    /* kernel_type */
    public const int LINEAR = 0;
    public const int POLY = 1;
    public const int RBF = 2;
    public const int SIGMOID = 3;
    public const int PRECOMPUTED = 4;

    public int svm_type;
    public int kernel_type;
    public int degree;	// for poly
    public double gamma;	// for poly/rbf/sigmoid
    public double coef0;	// for poly/sigmoid

    // these are for training only
    public double cache_size; // in MB
    public double eps;	// stopping criteria
    public double C;	// for C_SVC, EPSILON_SVR and NU_SVR
    public int nr_weight;		// for C_SVC
    public int[] weight_label;	// for C_SVC
    public double[] weight;		// for C_SVC
    public double nu;	// for NU_SVC, ONE_CLASS, and NU_SVR
    public double p;	// for EPSILON_SVR
    public int shrinking;	// use the shrinking heuristics
    public int probability; // do probability estimates

    public virtual object Clone() {
      var clone = new svm_parameter();
      clone.svm_type = svm_type;
      clone.kernel_type = kernel_type;
      clone.degree = degree;
      clone.gamma = gamma;
      clone.coef0 = coef0;
      clone.cache_size = cache_size;
      clone.eps = eps;
      clone.C = C;
      clone.nr_weight = nr_weight;
      if (weight_label != null) {
        clone.weight_label = new int[weight_label.Length];
        Array.Copy(weight_label, clone.weight_label, weight_label.Length);
      }
      if (weight != null) {
        clone.weight = new double[weight.Length];
        Array.Copy(weight, clone.weight, weight.Length);
      }
      clone.nu = nu;
      clone.p = p;
      clone.shrinking = shrinking;
      clone.probability = probability;
      return clone;
    }
  }
}
