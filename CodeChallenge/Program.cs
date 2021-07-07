using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
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
        ///             Show the difference between old highest value and the new highest 
        ///         Assert the order of events
        ///         
        ///     Constraints
        ///         Timer with action delegate must be used to tap the sensor data.
        ///     
        ///     Notes
        ///         You can change almost everything as long as the core of the challenge is preserved.
        /// 
        /// </summary>

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "<Pending>")]
        static async Task Main(string[] args)
        {
            new Sensor(WriteLine, 1);
            new Sensor(WriteLine, 10);
            new Thread(
                new ThreadStart(() => 
                    new Sensor(WriteLine, 200)));         

            ReadLine();
        }
    }

    class Sensor
    {
        public System.Timers.Timer timer;
        public dynamic handler;

        public Sensor(Action<Ping> doAThing, int seed)
        {
            handler = doAThing;
            var random = new Random();
            this.random = new Random(seed);

            timer = new System.Timers.Timer(random.Next(999));
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e) => handler(emit());

        protected Random random;
        virtual public Ping emit() {
            var packet = new Ping() { Now = DateTime.Parse(DateTime.Now.ToLongDateString()), Value = random.Next(100) };
            Thread.Sleep(random.Next(100)); 
            return packet; }
    }

    record Ping
    {     
        public string Source => Thread.CurrentThread.ManagedThreadId.ToString();
        public DateTime Now; 
        public float Value { get; init; }
    }
}
