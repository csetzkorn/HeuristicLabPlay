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

using HeuristicLab.Data;
using HeuristicLab.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.IntegerVectorEncoding.Tests {
  /// <summary>
  ///This is a test class for UniformOnePositionManipulator and is intended
  ///to contain all UniformOnePositionManipulator Unit Tests
  ///</summary>
  [TestClass()]
  public class UniformOnePositionManipulatorTest {
    /// <summary>
    ///A test for Apply
    ///</summary>
    [TestMethod()]
    [TestCategory("Encodings.IntegerVector")]
    [TestProperty("Time", "short")]
    public void UniformOnePositionManipulatorApplyTest() {
      TestRandom random = new TestRandom();
      IntegerVector parent, expected;
      IntMatrix bounds = new IntMatrix(1, 2);
      // The following test is not based on published examples
      random.Reset();
      random.IntNumbers = new int[] { 3, 3 };
      parent = new IntegerVector(new int[] { 2, 2, 3, 5, 1 });
      expected = new IntegerVector(new int[] { 2, 2, 3, 3, 1 });
      bounds[0, 0] = 2;
      bounds[0, 1] = 7;
      UniformOnePositionManipulator.Apply(random, parent, bounds);
      Assert.IsTrue(Auxiliary.IntegerVectorIsEqualByPosition(expected, parent));
    }
  }
}
