// <copyright file="MainWindowViewModel.cs" company="VIBES.technology">
// Copyright (c) VIBES.technology. All rights reserved.
// </copyright>

namespace LevelBarApp.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows.Media;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;
    using LevelBarGeneration;

    /// <summary>
    /// MainWindowViewModel
    /// </summary>
    /// <seealso cref="ViewModelBase" />
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields
        private readonly LevelBarGenerator levelBarGenerator;
        private RelayCommand connectToGeneratorCommand;
        private RelayCommand disconnectFromGeneratorCommand;
        private GeneratorState generatorState;
        private TimeSpan lastRenderingTime;
        #endregion


        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            levelBarGenerator = LevelBarGenerator.Instance;

            levelBarGenerator.GeneratorStateChanged += LevelBarGenerator_GeneratorStateChanged;
            levelBarGenerator.ChannelAdded += LevelBarGenerator_ChannelAdded;
            levelBarGenerator.ChannelLevelDataReceived += LevelBarGenerator_ChannelDataReceived;
            levelBarGenerator.ChannelRemoved += LevelBarGenerator_ChannelRemoved;

            GeneratorState = GeneratorState.Stopped;

            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }
        #endregion


        #region Properties
        /// <summary>
        /// Gets or sets the level bars, one for each channel.
        /// </summary>
        /// <value>
        /// The level bars.
        /// </value>
        public ObservableCollection<LevelBarViewModel> LevelBars { get; set; } = new ObservableCollection<LevelBarViewModel>();

        /// <summary>
        /// Gets the command to connect the generator
        /// </summary>
        /// <value>
        /// The connect generator.
        /// </value>
        public RelayCommand ConnectGeneratorCommand => connectToGeneratorCommand ?? (connectToGeneratorCommand = new RelayCommand(new System.Action(async () => await levelBarGenerator.Connect())));

        /// <summary>
        /// Gets the command to disconnect the generator
        /// </summary>
        /// <value>
        /// The disconnect generator.
        /// </value>
        public RelayCommand DisconnectGeneratorCommand => disconnectFromGeneratorCommand ?? (disconnectFromGeneratorCommand = new RelayCommand(new System.Action(async () => await levelBarGenerator.Disconnect())));

        public GeneratorState GeneratorState
        {
            get => generatorState;
            set
            {
                generatorState = value;
                RaisePropertyChanged(nameof(GeneratorState));
            }
        }
        #endregion


        #region Private methods
        private void LevelBarGenerator_ChannelAdded(object sender, ChannelChangedEventArgs e)
        {
            LevelBars.Insert(e.ChannelId, new LevelBarViewModel(e.ChannelId, "Level bar #" + e.ChannelId));
        }

        private void LevelBarGenerator_ChannelRemoved(object sender, ChannelChangedEventArgs e)
        {
            LevelBars.RemoveAt(e.ChannelId);
        }

        private void LevelBarGenerator_GeneratorStateChanged(object sender, GeneratorStateChangedEventArgs e)
        {
            GeneratorState = e.State;
            RaisePropertyChanged(nameof(GeneratorState));
        }

        private void LevelBarGenerator_ChannelDataReceived(object sender, ChannelDataEventArgs e)
        {
            // For each level bar
            for (int i = 0; i < e.ChannelIds.Length; ++i)
            {
                int id = e.ChannelIds[i];
                var levelBar = LevelBars[id];

                float level = e.Levels[i];
                float transformedLevel = LevelBarGenerator.TransformValue(level);

                // Update Level
                levelBar.Level = transformedLevel;

                // Update MaxLevel
                if (levelBar.MaxLevel < levelBar.Level)
                {
                    levelBar.MaxLevel = levelBar.Level;
                    levelBar.MaxLevelLastUpdate = DateTime.Now;
                }
            }
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (e is RenderingEventArgs renderingEventArgs)
            {
                var nextRenderingTime = renderingEventArgs.RenderingTime;

                ResetPeakholds(nextRenderingTime);

                // Update lastRenderingTime
                lastRenderingTime = renderingEventArgs.RenderingTime;
            }
        }

        private void ResetPeakholds(TimeSpan nextRenderingTime)
        {
            foreach (var levelBar in LevelBars)
            {
                // If it's time to reset MaxLevel
                if ((DateTime.Now - levelBar.MaxLevelLastUpdate) >= levelBar.PeakholdDuration)
                {
                    // Reset MaxLevel by reducing it over time
                    float timeDelta = (float)(nextRenderingTime.TotalSeconds - lastRenderingTime.TotalSeconds);
                    float penalty = timeDelta * levelBar.PeakholdResetSpeed;
                    levelBar.MaxLevel -= penalty;
                    levelBar.MaxLevel = Math.Min(1, Math.Max(0, levelBar.MaxLevel)); // ensure value is between 0 and 1
                }
            }
        }
        #endregion
    }
}
