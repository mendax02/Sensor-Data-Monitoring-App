using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace CodeChallenge
{
    public class SensorHub : ISensorHub
    {
        // This timer will periodically check for the threshold value of all sensors and reset when they reach maximum
        private Timer _timer;
        private bool disposedValue;
        private static readonly object _object = new();

        public static readonly ConcurrentDictionary<string, SensorData> SensorDictionary = new();

        public static readonly ConcurrentQueue<SensorData> SensorDataMaxIntervalQueue = new();
        public IList<ISensor> ActiveSensors { get; set; }
        public bool ResetAllSensor { get; set; } = false;
        public SensorHub()
        {
            _timer = new Timer(new Random().Next(500));
            _timer.Interval = 500;
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
            ActiveSensors = new List<ISensor>();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            IsMaximumThresholdReachedForAllSensors();
        }

        public void RegisterSensor(ISensor sensor)
        {
            ActiveSensors.Add(sensor);
            sensor.SensorDataChanged += OnSensorDataChanged;
            sensor.SensorThresholdReached += OnSensorThresholdValueReached;
        }

        private void OnSensorThresholdValueReached(object sender, SensorEventArgs e)
        {
            // Adding data point in the dictionary whenever Threshold value is reached.
            // It can be replaced with some sexternal service or write logs to some file system or some other required action on the event
            lock (_object)
            {
                var sensorReport = e?.Data;
                // If sensor data is not present then add in the dictionary else update the value to the latest timestamp
                if (!SensorDictionary.TryGetValue(sensorReport.PingData?.SensorId, out SensorData storedSensorData))
                {
                    SensorDictionary.TryAdd(sensorReport.PingData?.SensorId, sensorReport);
                }
                else
                {
                    SensorDictionary.TryUpdate(sensorReport.PingData?.SensorId, sensorReport, storedSensorData);
                }
            }
        }

        // When the event is raised it inserts the datapoint in the queue, which can be used for further processing
        private void OnSensorDataChanged(object sender, SensorEventArgs e) => SensorDataMaxIntervalQueue.Enqueue(e.Data);

        // Checks if Threshold value for sensors is reached
        public bool IsMaximumThresholdReachedForAllSensors()
        {
            lock (_object)
            {
                var result = ActiveSensors.Count != 0 && SensorDictionary.Count == ActiveSensors.Count;
                if (result)
                {
                    Reset();
                }
                return result;
            }
        }

        public void Reset()
        {
            lock (_object)
            {
                if (SensorDictionary.Any())
                {
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------");
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("Alert: Maximum Threshold value reached for all sensors!!!");
                    Console.ResetColor();
                    var dict = SensorDictionary.OrderBy(x => x.Value.PingData.TimePeriod);
                    foreach (var item in dict)
                    {
                        //  Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(item.Value));
                        Console.WriteLine(item.Value);
                    }
                    Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------");
                    SensorDictionary.Clear();
                }

                ResetAllSensor = true;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Re-setting Sensor Data to default values!!!");
                Console.ResetColor();
                foreach (var sensor in ActiveSensors)
                {
                    sensor.CurrentMaximumReading = default;
                    sensor.Delta = default;
                    sensor.PreviousMaximumReading = default;
                }
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _timer.Dispose();
                    _timer = null;
                    foreach (var sensor in ActiveSensors)
                    {
                        sensor.SensorDataChanged -= OnSensorDataChanged;
                        sensor.SensorThresholdReached -= OnSensorThresholdValueReached;
                    }

                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SensorHub()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

    }
}
