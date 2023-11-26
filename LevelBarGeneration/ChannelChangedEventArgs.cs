// <copyright file="ChannelChangedEventArgs.cs" company="VIBES.technology">
// Copyright (c) VIBES.technology. All rights reserved.
// </copyright>

namespace LevelBarGeneration
{
    using System;

    /// <summary>
    /// ChannelChangedEventArgs
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class ChannelChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the channel identifier.
        /// </summary>
        /// <value>
        /// The channel identifier.
        /// </value>
        public int ChannelId { get; set; }
    }
}