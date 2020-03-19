using System;

namespace VComputer.Services
{
    /// <summary>
    /// Represents a timer.
    /// </summary>
    public interface ITimer : IDisposable
    {
        /// <summary>
        /// The timer event.
        /// </summary>
        event Action Tick;

        /// <summary>
        /// Gets or sets the interval this timer executes at. Setting this while the timer is running will reset the timer.
        /// </summary>
        int Interval { get; set; }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the timer.
        /// </summary>
        void Stop();
    }
}