// <copyright file="LevelBarViewModel.cs" company="VIBES.technology">
// Copyright (c) VIBES.technology. All rights reserved.
// </copyright>

namespace LevelBarApp.ViewModels
{
    using GalaSoft.MvvmLight;
    using System;

    /// <summary>
    /// Represents a level bar for a channel
    /// </summary>
    /// <seealso cref="ViewModelBase" />
    public class LevelBarViewModel : ViewModelBase
    {
        #region Private fields
        private int id;
        private string name;
        private float level;
        private float maxLevel;
        #endregion


        #region Constructor
        public LevelBarViewModel(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
        #endregion


        #region Properties
        /// <summary>
        /// Represents how long the Peakhold lasts, before it starts to reset back to zero.
        /// </summary>
        public TimeSpan PeakholdDuration { get; set; } = TimeSpan.FromSeconds(2);

        /// <summary>
        /// Represents how fast the Peakhold resets back to zero. 
        /// For example: If speed=v, this means that it takes 1/v seconds to reset a value of 1 back to 0.
        /// </summary>
        public float PeakholdResetSpeed { get; set; } = 1.0f;

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id
        {
            get => id;
            set
            {
                id = value;
                RaisePropertyChanged(nameof(Id));
            }
        }

        /// <summary>
        /// Gets or sets the name of the channel.
        /// </summary>
        /// <value>
        /// The name of the channel.
        /// </value>
        public string Name
        {
            get => name;
            set
            {
                name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        public float Level
        {
            get => level;
            set
            {
                level = value;
                RaisePropertyChanged(nameof(Level));
            }
        }

        /// <summary>
        /// Gets or sets the maximum level used of the peakhold.
        /// </summary>
        /// <value>
        /// The maximum level.
        /// </value>
        public float MaxLevel
        {
            get => maxLevel;
            set
            {
                maxLevel = value;
                RaisePropertyChanged(nameof(MaxLevel));
            }
        }

        public DateTime MaxLevelLastUpdate { get; set; } = DateTime.Now;
        #endregion
    }
}