#nullable disable

using LevelBarGeneration;

namespace LevelBarGeneratorTests
{
    [TestClass]
    public class LevelBarGenerationTest
    {
        #region Constants
        private readonly TimeSpan maxWaitingDuration = TimeSpan.FromSeconds(100);
        private readonly TimeSpan interval = TimeSpan.FromMilliseconds(10);
        #endregion


        #region Private fields
        private LevelBarGenerator generator;
        private LevelBarGeneratorArgs generatorArgs;
        #endregion


        #region Init
        [TestInitialize]
        public void TestInitialize()
        {
            generatorArgs = new LevelBarGeneratorArgs
            {
                NumberOfChannels = 50,
                StartTimeDelay = TimeSpan.FromSeconds(0),
                Interval = interval
            };
            generator = new LevelBarGenerator(generatorArgs);
        }
        #endregion 


        #region Test methods
        [TestMethod]
        public void StoppedGenerator_Connects_ChannelsAreAdded()
        {
            // Arrange
            var channelIds = new List<int>();
            generator.ChannelAdded += (sender, e) => { channelIds.Add(e.ChannelId); };

            // Act
            generator.Connect();

            // Assert
            Assert.AreEqual(
                expected: generatorArgs.NumberOfChannels, 
                actual: channelIds.Count, 
                message: $"The number of channels added was different than expected.");
            Assert.AreEqual(
                expected: channelIds.Distinct().Count(), 
                actual: channelIds.Count,
                message: $"One or more channel IDs were not unique.");
        }

        [TestMethod]
        public void RunningGenerator_Disconnects_ChannelAreRemoved()
        {
            // Arrange
            var addedChannelIds = new List<int>();
            var removedChannelIds = new List<int>();
            generator.ChannelAdded += (sender, e) => { addedChannelIds.Add(e.ChannelId); };
            generator.ChannelRemoved += (sender, e) => { removedChannelIds.Add(e.ChannelId); };
            generator.Connect();

            // Act
            generator.Disconnect();

            // Assert
            int numberOfChannelsAdded = addedChannelIds.Count;
            int numberOfChannelsRemoved = removedChannelIds.Count;

            Assert.AreEqual(numberOfChannelsAdded, numberOfChannelsRemoved, 
                $"The number of channels removed was different than the number of channels added.");
        }

        [TestMethod]
        public void StoppedGenerator_Connects_StateChangesToRunning()
        {
            // Arrange
            GeneratorState? state = null; 
            generator.GeneratorStateChanged += (sender, e) => { state = e.State; };

            // Act
            generator.Connect();

            // Assert
            Assert.AreEqual(
                expected: GeneratorState.Running,
                actual: state,
                message: $"The generator state did not change to 'running'.");
        }

        [TestMethod]
        public void RunningGenerator_Disconnects_StateChangesToStopped()
        {
            // Arrange
            GeneratorState? state = null;
            generator.GeneratorStateChanged += (sender, e) => { state = e.State; };
            generator.Connect();

            // Act
            generator.Disconnect();

            // Assert
            Assert.AreEqual(
                expected: GeneratorState.Stopped,
                actual: state,
                message: $"The generator state did not change to 'stopped'.");
        }

        [TestMethod]
        public async Task ConnectedGenerator_WaitsSomeTime_ValidDataIsReceived()
        {
            // Arrange
            int[] channelIds = null;
            float[] levels = null;
            var dataReceivedSignal = new TaskCompletionSource<bool>();
            generator.ChannelLevelDataReceived += (sender, e) =>
            {
                if(!dataReceivedSignal.Task.IsCompleted)
                {
                    channelIds = e.ChannelIds;
                    levels = e.Levels;
                    dataReceivedSignal.SetResult(true);
                }
            };
            generator.Connect();

            // Act
            await Task.WhenAny(dataReceivedSignal.Task, Task.Delay(maxWaitingDuration));

            // Assert
            Assert.IsTrue(dataReceivedSignal.Task.IsCompleted, "ChannelLevelDataReceived event did not fire.");

            Assert.AreEqual(
                expected: generatorArgs.NumberOfChannels,
                actual: channelIds?.Length,
                message: $"The number of channel IDs received was different than expected.");

            Assert.AreEqual(
                expected: generatorArgs.NumberOfChannels,
                actual: levels?.Length,
                message: $"The number of level values received was different than expected.");

            Assert.IsTrue(levels.All(level => level >= 0 && level <= 1),
                "Some of the level values received were not in the range of [0, 1].");
        }
        #endregion
    }
}