using System;
using System.Threading.Tasks;
using System.Timers;

namespace CodeChallenge
{
    public class Sensor : ISensor
    {
        private Timer _timer;
        private bool _disposed;
        private readonly Action<Ping> _handler;
        private readonly int _maxSensorThresholdValue;
        private int _randomSeedValue => _maxSensorThresholdValue + 1;

        private static readonly object _object = new();
        protected static Random Random => new(Guid.NewGuid().GetHashCode());

        public event EventHandler<SensorEventArgs> SensorDataChanged;
        public event EventHandler<SensorEventArgs> SensorThresholdReached;
        public string SensorId { get; }

        public float CurrentMaximumReading { get; set; }

        public float PreviousMaximumReading { get; set; }

        public float Delta { get; set; }

        public Sensor(Action<Ping> doAThing, SensorConfig sensorSetup)
        {
            SensorId = sensorSetup.SensorId;
            _handler = doAThing;
            _maxSensorThresholdValue = (int)sensorSetup.ThresholdLimit - 1;
        }

        public void StartSensor()
        {
            _timer = new Timer(new Random(Guid.NewGuid().GetHashCode()).Next(999));

            _timer.Elapsed += Timer_Elapsed;
            _timer?.Start();
        }

        public void StopSensor()
        {
            _timer?.Stop();
            _timer.Elapsed -= Timer_Elapsed;
        }

        virtual public Ping Emit()
        {
            var packet = new Ping()
            {
                SourceThread = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString(),
                TimePeriod = DateTime.Now,
                SensorReading = Random.Next(_randomSeedValue),
                SensorId = SensorId
            };

            Task.Delay(Random.Next(100));

            return packet;
        }

        public void Dispose()
        {
            StopSensor();
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _timer?.Dispose();
            }

            _disposed = true;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var result = Emit();
            _handler(result);
            UpdateSensorState(result);
        }

        // Verifies the condtions upon which specific data point would be captured
        private void UpdateSensorState(Ping sensorData)
        {
            lock (_object)
            {
                //System.Threading.Thread.MemoryBarrier(); 
                if (sensorData.SensorReading > CurrentMaximumReading)
                {
                    PreviousMaximumReading = CurrentMaximumReading;
                    CurrentMaximumReading = sensorData.SensorReading;
                    Delta = CurrentMaximumReading - PreviousMaximumReading;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{nameof(sensorData.SensorId)}: {sensorData.SensorId}, {nameof(CurrentMaximumReading)}: {CurrentMaximumReading}, {nameof(PreviousMaximumReading)}: {PreviousMaximumReading}, {nameof(Delta)}: {Delta}, {nameof(sensorData.TimePeriod)}: {sensorData.TimePeriod}");
                    Console.ResetColor();
                    SensorDataChanged?.Invoke(this, new SensorEventArgs(new SensorData
                    {
                        PingData = sensorData,
                        MaximumSensorReading = CurrentMaximumReading,
                        PreviousMaximumSensorReading = PreviousMaximumReading,
                        Delta = Delta
                    }));
                }
                if (sensorData.SensorReading == _maxSensorThresholdValue)
                {
                    SensorThresholdReached?.Invoke(this, new SensorEventArgs(new SensorData
                    {
                        PingData = sensorData,
                        MaximumSensorReading = CurrentMaximumReading,
                        PreviousMaximumSensorReading = PreviousMaximumReading,
                        Delta = Delta
                    }));
                }
            }
        }
    }
}
