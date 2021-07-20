using System;
using System.Threading.Tasks;

namespace CodeChallenge
{
    public interface ISensorHub : IDisposable
    {
        /// <summary>
        /// Indicates that the Sensor data can be reset when true
        /// </summary>
        public bool ResetAllSensor { get; set; }
        /// <summary>
        /// Checks if Threshold value for sensors is reached
        /// </summary>
        /// <returns></returns>
        bool IsMaximumThresholdReachedForAllSensors();

        /// <summary>
        /// Register the sensor with the SensorHub and subscribes to events
        /// </summary>
        /// <param name="sensor"></param>
        void RegisterSensor(ISensor sensor);

        /// <summary>
        /// Reset the Maximum Sensor value Dictionary for all sensors. It is called when all sensors reach their maximum sensor limit.
        /// </summary>
        void Reset();
    }
}