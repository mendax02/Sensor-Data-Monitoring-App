using CodeChallenge;
using System.Diagnostics;
using System.Threading;
using Xunit;

namespace Tests
{
    public class SensorTests
    {
        [Fact]
        public void When_SensorData_Changes_SensorDataChanged_Event_Is_Raised()
        {
            var autoResetEvent = new AutoResetEvent(false);
            var actual = string.Empty;
            SensorData sensorData = default;

            var sensor = new Sensor(DoNothing, new SensorConfig { ThresholdLimit = 100, SensorId = "Titan-3330" });
            sensor.StartSensor();
            sensor.SensorDataChanged += (_, args) =>
            {
                sensorData = args?.Data;
                autoResetEvent.Set();
            };
            Assert.True(autoResetEvent.WaitOne());
            Assert.Equal("Titan-3330", sensorData.PingData.SensorId);
        }

        [Fact]
        public void When_SensorData_Reaches_Threshold_SensorThresholdReached_Event_Is_Raised()
        {
            var autoResetEvent = new AutoResetEvent(false);
            var actual = string.Empty;
            SensorData sensorData = default;

            var sensor = new Sensor(DoNothing, new SensorConfig { ThresholdLimit = 4, SensorId = "Titan-3330" });
            sensor.StartSensor();
            sensor.SensorThresholdReached += (_, args) =>
            {
                sensorData = args?.Data;
                autoResetEvent.Set();
            };

            Assert.True(autoResetEvent.WaitOne());
            Assert.Equal("Titan-3330", sensorData.PingData.SensorId);
            Assert.Equal(3, sensorData.MaximumSensorReading);
        }
        private static void DoNothing(Ping value)
        {
            // nothing 
        }
    }
}
