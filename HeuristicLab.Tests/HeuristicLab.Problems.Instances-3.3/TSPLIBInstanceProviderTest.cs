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
using System.Text;
using HeuristicLab.Problems.Instances.TSPLIB;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Problems.Instances.Tests {
  [TestClass()]
  public class TSPLIBInstanceProviderTest {

    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "medium")]
    public void GetTSPLIBTSPInstanceTest() {
      var target = new TSPLIBTSPInstanceProvider();
      StringBuilder erroneousInstances = new StringBuilder();
      int count = 0;
      foreach (var id in target.GetDataDescriptors()) {
        try {
          target.LoadData(id);
        } catch (Exception ex) {
          erroneousInstances.AppendLine(id.Name + ": " + ex.Message);
        }
        count++;
      }
      Assert.IsTrue(count > 0, "No problem instances were found.");
      Assert.IsTrue(erroneousInstances.Length == 0, "Some instances could not be parsed: " + Environment.NewLine + erroneousInstances.ToString());
    }

    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void GetTSPLIBATSPInstanceTest() {
      var target = new TSPLIBATSPInstanceProvider();
      StringBuilder erroneousInstances = new StringBuilder();
      int count = 0;
      foreach (var id in target.GetDataDescriptors()) {
        try {
          target.LoadData(id);
        } catch (Exception ex) {
          erroneousInstances.AppendLine(id.Name + ": " + ex.Message);
        }
        count++;
      }
      Assert.IsTrue(count > 0, "No problem instances were found.");
      Assert.IsTrue(erroneousInstances.Length == 0, "Some instances could not be parsed: " + Environment.NewLine + erroneousInstances.ToString());
    }

    [TestMethod]
    [TestCategory("Problems.Instances")]
    [TestProperty("Time", "short")]
    public void GetTSPLIBCVRPInstanceTest() {
      var target = new TSPLIBCVRPInstanceProvider();
      StringBuilder erroneousInstances = new StringBuilder();
      int count = 0;
      foreach (var id in target.GetDataDescriptors()) {
        try {
          target.LoadData(id);
        } catch (Exception ex) {
          erroneousInstances.AppendLine(id.Name + ": " + ex.Message);
        }
        count++;
      }
      Assert.IsTrue(count > 0, "No problem instances were found.");
      Assert.IsTrue(erroneousInstances.Length == 0, "Some instances could not be parsed: " + Environment.NewLine + erroneousInstances.ToString());
    }
  }
}
