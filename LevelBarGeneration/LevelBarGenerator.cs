// <copyright file="LevelBarGenerator.cs" company="VIBES.technology">
// Copyright (c) VIBES.technology. All rights reserved.
// </copyright>

namespace LevelBarGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MathNet.Numerics.Distributions;
    using MathNet.Numerics;
    using System.Threading;

    /// <summary>
    /// LevelBarGenerator
    /// </summary>
    public class LevelBarGenerator
    {
        #region Private fields
        private readonly LevelBarGeneratorArgs args;
        private GeneratorState state = GeneratorState.Stopped;
        private Timer timer;
        private float[][] levels;
        private float minLevel;
        private float maxLevel;
        private int[] channelIds;
        private int jobCounter;
        #endregion


        #region Events
        /// <summary>
        /// Occurs when [channel added].
        /// </summary>
        public event EventHandler<ChannelChangedEventArgs> ChannelAdded;

        /// <summary>
        /// Occurs when [channel removed].
        /// </summary>
        public event EventHandler<ChannelChangedEventArgs> ChannelRemoved;

        /// <summary>
        /// Occurs when [channel data received].
        /// </summary>
        public event EventHandler<ChannelDataEventArgs> ChannelLevelDataReceived;

        /// <summary>
        /// Occurs when [state changed].
        /// </summary>
        public event EventHandler<GeneratorStateChangedEventArgs> GeneratorStateChanged;
        #endregion


        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="LevelBarGenerator"/> class.
        /// </summary>
        public LevelBarGenerator(LevelBarGeneratorArgs args)
        {
            this.args = args;
        }
        #endregion


        #region Public methods
        /// <summary>
        /// Add the channels and start the <see cref="LevelBarGenerator"/>
        /// </summary>
        public void Connect()
        {
            if (state == GeneratorState.Running)
            {
                Console.WriteLine("Generator is already connected");
                return;
            }

            RegisterChannels();
            GenerateHelperData();
            Start();
            GeneratorStateChanged?.Invoke(this, new GeneratorStateChangedEventArgs { State = GeneratorState.Running });
        }

        /// <summary>
        /// Remove the channels and stop the <see cref="LevelBarGenerator"/>
        /// </summary>
        public void Disconnect()
        {
            if (state == GeneratorState.Stopped)
            {
                Console.WriteLine("Generator is already stopped");
                return;
            }

            Stop();
            DeregisterChannels();
            GeneratorStateChanged?.Invoke(this, new GeneratorStateChangedEventArgs { State = GeneratorState.Stopped });
        }

        /// <summary>
        /// Transforms a Level value to a scale that is more appropriate for visualization.
        /// </summary>
        public float TransformLevelValue(double level)
        {
            level = Math.Log10(level); // transform 
            var min = Math.Log10(minLevel);
            var max = Math.Log10(maxLevel);

            level = (level - min) / (max - min); // normalize
            level = Math.Max(0, Math.Min(1, level)); // ensure value is between 0 and 1
            return (float)level;
        }
        #endregion


        #region Private methods
        private void Start()
        {
            timer = new Timer(ExecuteScheduledJob, null, args.StartTimeDelay, args.Interval);
            state = GeneratorState.Running;
        }

        private void Stop()
        {
            timer?.Dispose();
            state = GeneratorState.Stopped;
        }

        private void RegisterChannels()
        {
            for (int i = 0; i < args.NumberOfChannels; ++i)
            {
                ChannelAdded?.Invoke(this, new ChannelChangedEventArgs { ChannelId = i });
            }
        }

        private void DeregisterChannels()
        {
            for (int i = args.NumberOfChannels - 1; i >= 0; --i)
            {
                ChannelRemoved?.Invoke(this, new ChannelChangedEventArgs { ChannelId = i });
            }
        }

        private void ExecuteScheduledJob(object state)
        {
            // Use lock to ensure thread safety
            lock (this)
            {
                // Execute scheduled job logic, and wait for completion
                Execute().Wait();
            }
        }

        private Task Execute()
        {
            // Don't run when there is no data present
            if (levels == null)
            {
                return Task.CompletedTask;
            }

            jobCounter += 1;
            if (jobCounter >= levels.Length)
            {
                jobCounter = 0;
            }

            ChannelLevelDataReceived?.Invoke(this, new ChannelDataEventArgs { ChannelIds = channelIds, Levels = levels[jobCounter] });

            return Task.CompletedTask;
        }

        private byte[][] GenerateHelperData()
        {
            // random
            Random randomLevel = new();

            // Generate meta data
            channelIds = Enumerable.Range(0, args.NumberOfChannels).ToArray();

            // Helpers
            int numberOfDataPointsPerChannel = 5 * (int)(args.SamplingTime * args.SamplingRate);
            int numberOfBlocks = numberOfDataPointsPerChannel / (args.ChannelBlockSize / 8);
            int numberOfTriggerBlocks = (int)(args.SamplingTime * args.SamplingRate) / (args.ChannelBlockSize / 8);

            // Generate the real data
            int triggerChannel = 36;
            byte[][] rawData = new byte[numberOfBlocks][];
            levels = new float[numberOfBlocks][];

            for (int b = 0; b < numberOfBlocks; b++)
            {
                List<byte> blockData = new();
                List<float> blockLevels = new();

                for (int i = 0; i < args.NumberOfChannels; i++)
                {
                    if (i != triggerChannel)
                    {
                        // Response Data
                        double[] data = new double[args.ChannelBlockSize / 8];
                        Normal normal = new(0, 0.01);
                        normal.Samples(data);

                        // Trigger Data
                        if (b < numberOfTriggerBlocks)
                        {
                            float factor = (numberOfTriggerBlocks - b) / (float)numberOfTriggerBlocks;
                            factor = (float)Math.Exp(1 - (1 / (factor * factor)));
                            double[] sineData = Generate.Sinusoidal(args.ChannelBlockSize / 8, args.SamplingRate, randomLevel.NextDouble() * 1000, factor);

                            for (int k = 0; k < data.Length; k++)
                            {
                                data[k] += sineData[k];
                            }
                        }

                        blockData.AddRange(GetBytes(data));
                        blockLevels.Add((float)(data.Select(c => Math.Abs(c)).Max() / 10d));
                    }
                    else
                    {
                        // Trigger Data
                        double[] data = new double[args.ChannelBlockSize / 8];
                        Normal normal = new(0, 0.01);
                        normal.Samples(data);

                        if (b == 0)
                        {
                            double[] triggerData = Generate.Impulse(args.ChannelBlockSize / 8, 1, 50);
                            triggerData[49] = 0.9;
                            triggerData[48] = 0.7;
                            triggerData[47] = 0.2;
                            triggerData[46] = 0.1;
                            triggerData[51] = 0.9;
                            triggerData[52] = 0.7;
                            triggerData[53] = 0.2;
                            triggerData[54] = 0.1;

                            for (int k = 0; k < data.Length; k++)
                            {
                                data[k] += triggerData[k];
                            }
                        }

                        blockData.AddRange(GetBytes(data));
                        blockLevels.Add((float)(data.Select(c => Math.Abs(c)).Max() / 10d));
                    }
                }

                // Set the value
                rawData[b] = blockData.ToArray();
                levels[b] = blockLevels.ToArray();
            }

            minLevel = levels.SelectMany(levelArray => levelArray).Min();
            maxLevel = levels.SelectMany(levelArray => levelArray).Max();

            return rawData;
        }

        private static byte[] GetBytes(double[] values)
        {
            var result = new byte[values.Length * sizeof(double)];
            Buffer.BlockCopy(values, 0, result, 0, result.Length);
            return result;
        }        
        #endregion
    }
}
