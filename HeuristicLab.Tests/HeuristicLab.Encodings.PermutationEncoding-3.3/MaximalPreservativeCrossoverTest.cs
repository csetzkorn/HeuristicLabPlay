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
  ///This is a test class for MaximalPreservativeCrossover and is intended
  ///to contain all MaximalPreservativeCrossover Unit Tests
  ///</summary>
  [TestClass()]
  public class MaximalPreservativeCrossoverTest {
    /// <summary>
    ///A test for Apply
    ///</summary>
    [TestMethod]
    [TestCategory("Encodings.Permutation")]
    [TestProperty("Time", "short")]
    public void MaximalPreservativeCrossoverApplyTest() {
      TestRandom random = new TestRandom();
      Permutation parent1, parent2, expected, actual;
      // The following test is based on an example from Larranaga, 1999. Genetic Algorithms for the Traveling Salesman Problem.
      random.Reset();
      random.IntNumbers = new int[] { 3, 2 };
      parent1 = new Permutation(PermutationTypes.RelativeUndirected, new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });
      Assert.IsTrue(parent1.Validate());
      parent2 = new Permutation(PermutationTypes.RelativeUndirected, new int[] { 1, 3, 5, 7, 6, 4, 2, 0 });
      Assert.IsTrue(parent2.Validate());
      expected = new Permutation(PermutationTypes.RelativeUndirected, new int[] { 1, 0, 2, 3, 4, 5, 7, 6 });
      Assert.IsTrue(expected.Validate());
      actual = MaximalPreservativeCrossover.Apply(random, parent1, parent2);
      Assert.IsTrue(actual.Validate());
      Assert.IsTrue(Auxiliary.PermutationIsEqualByPosition(expected, actual));

      // perform a test when the two permutations are of unequal length
      random.Reset();
      bool exceptionFired = false;
      try {
        MaximalPreservativeCrossover.Apply(random, new Permutation(PermutationTypes.RelativeUndirected, 8), new Permutation(PermutationTypes.RelativeUndirected, 6));
      }
      catch (System.ArgumentException) {
        exceptionFired = true;
      }
      Assert.IsTrue(exceptionFired);
    }
  }
}
