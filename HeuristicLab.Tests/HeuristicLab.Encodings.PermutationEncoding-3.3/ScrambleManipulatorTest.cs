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

using HeuristicLab.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.PermutationEncoding.Tests {
  /// <summary>
  ///This is a test class for ScrambleManipulator and is intended
  ///to contain all ScrambleManipulator Unit Tests
  ///</summary>
  [TestClass()]
  public class ScrambleManipulatorTest {
    /// <summary>
    ///A test for Apply
    ///</summary>
    [TestMethod]
    [TestCategory("Encodings.Permutation")]
    [TestProperty("Time", "short")]
    public void ScrambleManipulatorApplyTest() {
      TestRandom random = new TestRandom();
      Permutation parent, expected;
      // The following test is based on an example from Larranaga, P. et al. 1999. Genetic Algorithms for the Travelling Salesman Problem: A Review of Representations and Operators. Artificial Intelligence Review, 13
      random.Reset();
      random.IntNumbers = new int[] { 3, 6, 1, 1, 1, 0 };
      parent = new Permutation(PermutationTypes.RelativeUndirected, new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });
      Assert.IsTrue(parent.Validate());

      expected = new Permutation(PermutationTypes.RelativeUndirected, new int[] { 0, 1, 2, 4, 5, 6, 3, 7 });
      Assert.IsTrue(expected.Validate());
      ScrambleManipulator.Apply(random, parent);
      Assert.IsTrue(parent.Validate());
      Assert.IsTrue(Auxiliary.PermutationIsEqualByPosition(expected, parent));
    }
  }
}
