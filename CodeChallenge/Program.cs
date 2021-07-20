using System.Collections.Generic;
using System.Threading.Tasks;
using static System.Console;

namespace CodeChallenge
{
    class Program
    {
        /// <summary>
        /// 
        ///  Challenge: 
        ///  
        ///     In this example we are gathing information from sensors 
        ///     We need to collect all the data and verify specific data points 
        ///     for each sensor AND all the sensors combine AS the data streams in.  
        ///
        ///     You now own this code; uplift it to a reasonable quality standard and prepare it for future development.
        ///     
        ///     For some reason features of the code are missing.
        ///     
        ///     As the code runs it should: 
        ///     
        ///         Assert when a new Max value for 1 sensor is reached
        ///         Assert when a new Max value for all sensors is reached AND
        ///         Show the difference between old highest value and the new highest 
        ///         Assert the order of events
        ///         
        ///     Constraints
        ///         Timer with action delegate must be used to tap the sensor data.
        ///     
        ///     Notes
        ///         You can change almost everything as long as the core of the challenge is preserved.
        /// 
        /// </summary>

        static async Task Main(string[] args)
        {
            var sensorSetupList = new List<SensorConfig> {
                new SensorConfig { ThresholdLimit = 100, SensorId = "Titan-3330" },
                new SensorConfig { ThresholdLimit = 100, SensorId = "DTM-3650" } ,
                new SensorConfig { ThresholdLimit = 50, SensorId = "Rover-TM500"}
             };

            var sensorHub = new SensorHub();

            foreach (var setupObj in sensorSetupList)
            {
                ISensor sensor = new Sensor(WriteLine, setupObj);
                sensor.StartSensor();
                sensorHub.RegisterSensor(sensor);
            }
            await Task.Delay(20000); // Mimic registration of new sensor during runtime
            var newSensor = new Sensor(WriteLine, new SensorConfig { ThresholdLimit = 100, SensorId = "NewSensorRuntime" });
            newSensor.StartSensor();
            sensorHub.RegisterSensor(newSensor);

            // new Thread(() => ReadLine()).Start();

            ReadLine();
        }
    }

}
