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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Encodings.General;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.GVR {
  [Item("GVRCrossover", "The GVR crossover operation. It is implemented as described in Pereira, F.B. et al (2002). GVR: a New Genetic Representation for the Vehicle Routing Problem. AICS 2002, LNAI 2464, pp. 95-102.")]
  [StorableClass]
  public sealed class GVRCrossover : VRPCrossover, IStochasticOperator, IGVROperator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }

    [StorableConstructor]
    private GVRCrossover(bool deserializing) : base(deserializing) { }

    public GVRCrossover() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used for stochastic manipulation operators."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GVRCrossover(this, cloner);
    }

    private GVRCrossover(GVRCrossover original, Cloner cloner)
      : base(original, cloner) {
    }

    private GVREncoding Crossover(IRandom random, GVREncoding parent1, GVREncoding parent2) {
      GVREncoding child = parent1.Clone() as GVREncoding;

      Tour tour = parent2.Tours[random.Next(parent2.Tours.Count)];
      int breakPoint1 = random.Next(tour.Stops.Count);
      int length = random.Next(1, tour.Stops.Count - breakPoint1 + 1);
      List<int> subroute = tour.Stops.GetRange(breakPoint1, length);

      //remove duplicates
      List<Tour> toBeRemoved = new List<Tour>();

      foreach (Tour route in child.Tours) {
        foreach (int city in subroute) {
          route.Stops.Remove(city);
        }

        if (route.Stops.Count == 0)
          toBeRemoved.Add(route);
      }
      foreach (Tour route in toBeRemoved) {
        child.Tours.Remove(route);
      }

      //choose nearest customer
      double minDistance = -1;
      int customer = -1;
      for (int i = 1; i <= ProblemInstance.Cities.Value; i++) {
        if (!subroute.Contains(i)) {
          double distance = ProblemInstance.GetDistance(subroute[0], i, child);

          if (customer == -1 || distance < minDistance) {
            customer = i;
            minDistance = distance;
          }
        }
      }

      //insert
      if (customer != -1) {
        Tour newTour;
        int newPosition;
        child.FindCustomer(customer, out newTour, out newPosition);
        newTour.Stops.InsertRange(newPosition + 1, subroute);
      } else {
        //special case -> only one tour, whole tour has been chosen as subroute
        child = parent1.Clone() as GVREncoding;
      }

      return child;
    }

    public override IOperation InstrumentedApply() {
      ItemArray<IVRPEncoding> parents = new ItemArray<IVRPEncoding>(ParentsParameter.ActualValue.Length);
      for (int i = 0; i < ParentsParameter.ActualValue.Length; i++) {
        IVRPEncoding solution = ParentsParameter.ActualValue[i];
        if (!(solution is GVREncoding)) {
          parents[i] = GVREncoding.ConvertFrom(solution, ProblemInstance);
        } else {
          parents[i] = solution;
        }
      }
      ParentsParameter.ActualValue = parents;

      ChildParameter.ActualValue = Crossover(RandomParameter.ActualValue, parents[0] as GVREncoding, parents[1] as GVREncoding);

      return base.InstrumentedApply();
    }
  }
}
