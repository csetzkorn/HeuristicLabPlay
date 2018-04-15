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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective.Tests {
  [TestClass]
  public class GenerationalDistanceTest {

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    [TestCategory("Problems.TestFunctions.MultiObjective")]
    [TestProperty("Time", "short")]
    public void GenerationalDistanceTestEmptyOptimalFront() {

      double[] point = new double[2];
      point[0] = 0.5;
      point[1] = 0.5;
      double[][] front = { point };
      double[][] referencefront = { };
      GenerationalDistance.Calculate(front, referencefront, 1);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    [TestCategory("Problems.TestFunctions.MultiObjective")]
    [TestProperty("Time", "short")]
    public void GenerationalDistanceTestEmptyFront() {

      double[] point = new double[2];
      point[0] = 0.5;
      point[1] = 0.5;
      double[][] front = { };
      double[][] referencefront = { point };
      GenerationalDistance.Calculate(front, referencefront, 1);
    }

    [TestMethod]
    [TestCategory("Problems.TestFunctions.MultiObjective")]
    [TestProperty("Time", "short")]
    public void GenerationalDistanceTestSamePoint() {

      double[] point = new double[2];
      point[0] = 0.5;
      point[1] = 0.5;
      double[][] front = { point };
      double[] point1 = new double[2];
      point1[0] = 0.5;
      point1[1] = 0.5;
      double[][] referencefront = { point1 };
      double dist = GenerationalDistance.Calculate(front, referencefront, 1);
      Assert.AreEqual(0, dist);
    }

    [TestMethod]
    [TestCategory("Problems.TestFunctions.MultiObjective")]
    [TestProperty("Time", "short")]
    public void GenerationalDistanceTestSinglePoint() {
      double[] point = new double[2];
      point[0] = 0;
      point[1] = 0;
      double[][] front = { point };
      double[] point2 = new double[2];
      point2[0] = 1;
      point2[1] = 1;
      double[][] referencefront = { point2 };
      double dist = GenerationalDistance.Calculate(front, referencefront, 1);
      Assert.AreEqual(Math.Sqrt(2), dist);
    }

    [TestMethod]
    [TestCategory("Problems.TestFunctions.MultiObjective")]
    [TestProperty("Time", "short")]
    public void GenerationalDistanceTestDifferentSizes() {
      double[] point = new double[2];
      point[0] = 0;
      point[1] = 0;
      double[] point1 = new double[2];
      point1[0] = 1;
      point1[1] = 0.5;
      double[][] front = { point, point1 };
      double[] point2 = new double[2];
      point2[0] = 1;
      point2[1] = 0;
      double[][] referencefront = { point2 };
      double dist = GenerationalDistance.Calculate(front, referencefront, 1);
      Assert.AreEqual(0.75, dist);
    }

    [TestMethod]
    [TestCategory("Problems.TestFunctions.MultiObjective")]
    [TestProperty("Time", "short")]
    public void GenerationalDistanceTestQuadratic() {
      double[] point = new double[2];
      point[0] = 0;
      point[1] = 0;
      double[] point1 = new double[2];
      point1[0] = 0;
      point1[1] = 1;
      double[][] front = { point, point1 };
      double[] point2 = new double[2];
      point2[0] = 1;
      point2[1] = 0;
      double[] point3 = new double[2];
      point3[0] = 1;
      point3[1] = 1;
      double[][] referencefront = { point2, point3 };
      double dist = GenerationalDistance.Calculate(front, referencefront, 1);
      Assert.AreEqual(1, dist);
    }
  }
}
