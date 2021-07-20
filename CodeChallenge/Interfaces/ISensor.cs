using System;

namespace CodeChallenge
{
    public interface ISensor: IDisposable
    {
        public event EventHandler<SensorEventArgs> SensorDataChanged;
        public event EventHandler<SensorEventArgs> SensorThresholdReached;
        string SensorId { get; }

        float CurrentMaximumReading { get; set; }

        float PreviousMaximumReading { get; set; }

        float Delta { get; set; }

        void StartSensor();

        void StopSensor();
    }

}
