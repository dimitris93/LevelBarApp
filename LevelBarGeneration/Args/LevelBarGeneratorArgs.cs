using System;

namespace LevelBarGeneration
{
    /// <summary>
    /// Arguments for creating a <see cref="LevelBarGenerator"/>
    /// </summary>
    public class LevelBarGeneratorArgs
    {
        /// <summary>
        /// The number of channels
        /// </summary>
        public int NumberOfChannels { get; set; } = 75;

        /// <summary>
        /// The frequency with which channel data are received
        /// </summary>
        public TimeSpan Interval { get; set; } = TimeSpan.FromMilliseconds(4);

        /// <summary>
        /// A delay that is imposed right before the to the data generation begins for the first time
        /// </summary>
        public TimeSpan StartTimeDelay { get; set; } = TimeSpan.FromSeconds(1);

        /// <summary>
        /// ChannelBlockSize
        /// </summary>
        public int ChannelBlockSize { get; set; } = 512;

        /// <summary>
        /// SamplingRate
        /// </summary>
        public int SamplingRate { get; set; } = 16384;

        /// <summary>
        /// SamplingTime
        /// </summary>
        public double SamplingTime { get; set; } = 1.0d;

    }
}
