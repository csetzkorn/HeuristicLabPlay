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
using HeuristicLab.Common;

namespace HeuristicLab.MainForm {
  public enum ProgressState { Started = 1, Canceled = 2, Finished = 3 };

  public interface IProgress : IContent {
    /// <summary>
    /// Gets or sets the currently associated status text with the progress.
    /// </summary>
    string Status { get; set; }
    /// <summary>
    /// Gets or sets the currently associated progress value in the range (0;1].
    ///  Values outside this range are permitted and need to be handled in some feasible manner.
    /// </summary>
    double ProgressValue { get; set; }
    /// <summary>
    /// Gets or sets the current state of the progress. Every progress starts in state
    /// Started and then becomes either Canceled or Finished.
    /// If it is reused it may be Started again.
    /// </summary>
    ProgressState ProgressState { get; }
    /// <summary>
    /// Returns whether the operation can be canceled or not.
    /// This can change during the course of the progress.
    /// </summary>
    bool CanBeCanceled { get; }

    /// <summary>
    /// Requests the operation behind the process to cancel.
    /// Check the !ProgressState property when the cancellation succeeded.
    /// The corresponding event will also notify of a success.
    /// </summary>
    /// <exception cref="NotSupportedException">Thrown when cancellation is not supported.</exception>
    void Cancel();
    /// <summary>
    /// Sets the ProgressValue to 1 and the ProgressState to Finished.
    /// </summary>
    void Finish();

    /// <summary>
    /// Starts or restarts a Progress. 
    /// </summary>
    void Start();

    void Start(string status);

    /// <summary>
    /// The status text changed.
    /// </summary>
    event EventHandler StatusChanged;
    /// <summary>
    /// The value of the progress changed. This is the (0;1] progress value from starting to finish. Values outside this range are permitted and need to be handled in some feasible manner.
    /// </summary>
    event EventHandler ProgressValueChanged;
    /// <summary>
    /// The state of the progress changed. The handler is supposed to query the ProgressState property.
    /// </summary>
    event EventHandler ProgressStateChanged;
    /// <summary>
    /// The progress' ability to cancel changed.
    /// </summary>
    event EventHandler CanBeCanceledChanged;
    /// <summary>
    /// A cancelation is requested.
    /// </summary>
    event EventHandler CancelRequested;
  }
}
