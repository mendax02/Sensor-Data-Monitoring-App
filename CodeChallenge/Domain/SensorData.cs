using System.Globalization;

namespace CodeChallenge
{
    public record SensorData
    {
        public Ping PingData;
        public float MaximumSensorReading;
        public float PreviousMaximumSensorReading;
        public float Delta;

        public override string ToString()
        {
            return $" SensorId: {PingData.SensorId}, SourceThread: {PingData.SourceThread}, TimePeriod: {PingData.TimePeriod.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)}, " +
                $"{nameof(MaximumSensorReading)}: {MaximumSensorReading}, {nameof(PreviousMaximumSensorReading)}: {PreviousMaximumSensorReading}, {nameof(Delta)}: {Delta}";
        }
    }

    /// <summary>
    /// Initial Values required to init and register the sensor
    /// </summary>
    public record SensorConfig
    {
        public float ThresholdLimit;
        public string SensorId;
    }
}
