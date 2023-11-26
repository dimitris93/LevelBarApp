// <copyright file="GeneratorStateChangedEventArgs.cs" company="VIBES.technology">
// Copyright (c) VIBES.technology. All rights reserved.
// </copyright>

namespace LevelBarGeneration
{
    using System;

    /// <summary>
    /// GeneratorStateChangedEventArgs
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class GeneratorStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the state the LevelBarGenerator changed to.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public GeneratorState State { get; set; }
    }
}