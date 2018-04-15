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
using System.Data.Linq;
using System.Linq;

namespace HeuristicLab.Services.Hive.DataAccess.Daos {
  public class RequiredPluginDao : GenericDao<Guid, RequiredPlugin> {
    public RequiredPluginDao(DataContext dataContext) : base(dataContext) { }

    public override RequiredPlugin GetById(Guid id) {
      return GetByIdQuery(DataContext, id);
    }

    #region Compiled queries
    private static readonly Func<DataContext, Guid, RequiredPlugin> GetByIdQuery =
      CompiledQuery.Compile((DataContext db, Guid requiredPluginId) =>
        (from requiredPlugin in db.GetTable<RequiredPlugin>()
         where requiredPlugin.RequiredPluginId == requiredPluginId
         select requiredPlugin).SingleOrDefault());
    #endregion
  }
}
