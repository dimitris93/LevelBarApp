// <copyright file="ChannelDataEventArgs.cs" company="VIBES.technology">
// Copyright (c) VIBES.technology. All rights reserved.
// </copyright>

namespace LevelBarGeneration
{
    using System;

    /// <summary>
    /// ChannelDataEventArgs
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class ChannelDataEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the channel ids.
        /// </summary>
        /// <value>
        /// The channel ids.
        /// </value>
        public int[] ChannelIds { get; set; }

        /// <summary>
        /// Gets or sets the levels.
        /// Each entry in the levels array corresponds to a level value
        /// for a respective channel ID in the ChannelIds array
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public float[] Levels { get; set; }
    }
}