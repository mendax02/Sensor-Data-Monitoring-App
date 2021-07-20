using System.Timers;

namespace CodeChallenge
{
    public interface ITimer
    {
        #region Public Events

        event ElapsedEventHandler Elapsed;

        #endregion

        #region Public Properties

        double Interval { get; set; }

        #endregion

        #region Public Methods and Operators

        void Dispose();

        void Start();

        void Stop();

        #endregion
    }
}
