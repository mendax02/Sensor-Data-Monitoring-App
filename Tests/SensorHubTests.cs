using CodeChallenge;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class SensorHubTests
    {
        private readonly Mock<ISensor> _sensorMock1;

        public SensorHubTests()
        {
            _sensorMock1 = new Mock<ISensor>();
        }

        [Fact]
        public void When_Register_Sensor_Is_Called_Registers_Sensor_In_SensorHub()
        {
            var sensorHub = new SensorHub();
            sensorHub.RegisterSensor(_sensorMock1.Object);
            _sensorMock1.SetupAdd(x => x.SensorDataChanged += (sender, args) => { });
            _sensorMock1.SetupAdd(x => x.SensorThresholdReached += (sender, args) => { });
            _sensorMock1.VerifyAdd(m => m.SensorDataChanged += It.IsAny<EventHandler<SensorEventArgs>>(), Times.Exactly(1));
            _sensorMock1.VerifyAdd(m => m.SensorThresholdReached += It.IsAny<EventHandler<SensorEventArgs>>(), Times.Exactly(1));
        }

        [Fact]
        public void When_OnSensorDataChanged_Raised_Add_SensorData_In_Dictionary()
        {
            InitialDataSetup(out SensorEventArgs data1, out _);

            var sensorHub = new SensorHub();
            sensorHub.RegisterSensor(_sensorMock1.Object);

            _sensorMock1.Raise(t => t.SensorDataChanged += null, data1);

            var mockSensorHub = new Mock<ISensorHub>();
            Assert.True(!SensorHub.SensorDataMaxIntervalQueue.IsEmpty);
        }

        [Fact]
        public void When_SensorThresholdReached_Raised_Add_SensorData_In_Dictionary()
        {
            InitialDataSetup(out SensorEventArgs data1, out _);

            var sensorMock2 = new Mock<ISensor>();
            _sensorMock1.Setup(x => x.SensorId).Returns("Sensor-1");
            sensorMock2.Setup(x => x.SensorId).Returns("Sensor-2");

            var sensorHub = new SensorHub();
            sensorHub.RegisterSensor(_sensorMock1.Object);
            sensorHub.RegisterSensor(sensorMock2.Object);

            _sensorMock1.Raise(t => t.SensorThresholdReached += null, data1);

            Assert.True(!SensorHub.SensorDictionary.IsEmpty);

            var updatedData = new SensorEventArgs(new SensorData
            {
                PingData = new Ping
                {
                    SensorId = "Sensor-1",
                    SensorReading = 98,
                    SourceThread = "1",
                    TimePeriod = DateTime.Now
                },
                MaximumSensorReading = 98,
                PreviousMaximumSensorReading = 88,
                Delta = 1
            });

            _sensorMock1.Raise(t => t.SensorThresholdReached += null, updatedData);

            Assert.True(!SensorHub.SensorDictionary.IsEmpty);
        }

        [Fact]
        public async Task When_SensorThresholdReached_ForAllSensors_SensorDictionary_Is_Reset()
        {
            InitialDataSetup(out SensorEventArgs data1, out SensorEventArgs data2);
            var sensorMock2 = new Mock<ISensor>();
            _sensorMock1.Setup(x => x.SensorId).Returns("Sensor-1");
            sensorMock2.Setup(x => x.SensorId).Returns("Sensor-2");
            var sensorHub = new SensorHub();
            sensorHub.RegisterSensor(_sensorMock1.Object);
            sensorHub.RegisterSensor(sensorMock2.Object);
            _sensorMock1.Raise(t => t.SensorThresholdReached += null, data1);
            _sensorMock1.Raise(t => t.SensorThresholdReached += null, data2);
            await Task.Delay(550);

            Assert.True(SensorHub.SensorDictionary.IsEmpty);
        }

        private void InitialDataSetup(out SensorEventArgs data1, out SensorEventArgs data2)
        {
            data1 = new SensorEventArgs(new SensorData
            {
                PingData = new Ping
                {
                    SensorId = "Sensor-1",
                    SensorReading = 98,
                    SourceThread = "1",
                    TimePeriod = DateTime.Now
                },
                MaximumSensorReading = 89,
                PreviousMaximumSensorReading = 80,
                Delta = 9
            });

            data2 = new SensorEventArgs(new SensorData
            {
                PingData = new Ping
                {
                    SensorId = "Sensor-2",
                    SensorReading = 98,
                    SourceThread = "4",
                    TimePeriod = DateTime.Now
                },
                MaximumSensorReading = 89,
                PreviousMaximumSensorReading = 88,
                Delta = 1
            });
        }
    }

}
