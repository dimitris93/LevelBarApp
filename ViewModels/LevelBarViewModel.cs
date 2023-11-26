// <copyright file="LevelBarViewModel.cs" company="VIBES.technology">
// Copyright (c) VIBES.technology. All rights reserved.
// </copyright>

namespace LevelBarApp.ViewModels
{
    using GalaSoft.MvvmLight;

    /// <summary>
    /// Represents a level bar for a channel
    /// </summary>
    /// <seealso cref="ViewModelBase" />
    public class LevelBarViewModel : ViewModelBase
    {
        // Fields
        private string name = string.Empty;
        private float level = 0.0f;
        private float maxLevel = 0.0f;
        private int id;

        // Properties

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
    }
}