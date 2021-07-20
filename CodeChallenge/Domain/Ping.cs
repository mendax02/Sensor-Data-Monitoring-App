using System;
using System.Globalization;

namespace CodeChallenge
{
    /// <summary>
    /// Sensor Data for the sensor 
    /// </summary>
    public record Ping
    {
        public string SourceThread { get; init; }

        public DateTime TimePeriod; // Occurence of the event
        public float SensorReading { get; init; } // The sensor value
        public string SensorId { get; init; } // This will recognise the sensor from which data is coming

        public override string ToString() => $"SourceThread: {SourceThread}, TimePeriod: {TimePeriod.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)}, SensorReading: {SensorReading}, SensorId: {SensorId}";
    }

    public class SensorEventArgs : EventArgs
    {
        public SensorData Data { get; }

        public SensorEventArgs(SensorData data)
        {
            Data = data;
        }
    }
}
