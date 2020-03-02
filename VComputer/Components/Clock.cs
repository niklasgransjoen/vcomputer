using System;
using System.Threading;

namespace VComputer.Components
{
    internal sealed class Clock : IDisposable
    {
        public const int MinClockInterval = 2;
        public const int MaxClockInterval = 20_000;

        private readonly Timer _timer;
        private double _interval;

        // Stores falling or rising edge.
        private bool _state = false;

        #region Construct & Dispose

        public Clock(double interval)
        {
            _timer = new Timer(OnTick, state: null, Timeout.Infinite, Timeout.Infinite);

            Interval = interval;
        }

        public void Dispose()
        {
            _timer.Dispose();
        }

        #endregion Construct & Dispose

        #region Events

        public event Action? FallingEdge;

        public event Action? RisingEdge;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating if the clock is currently running.
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the interval of the clock.
        /// </summary>
        public double Interval
        {
            get => _interval;
            set
            {
                _interval = Math.Min(MaxClockInterval, Math.Max(MinClockInterval, value));
                _timer.Change((int)_interval / 2, (int)_interval / 2);
            }
        }

        #endregion Properties

        #region Properties

        public void Step()
        {
            if (!IsEnabled)
            {
                Tick();
            }
        }

        #endregion Properties

        private void OnTick(object? state)
        {
            if (IsEnabled)
            {
                Tick();
            }
        }

        private void Tick()
        {
            if (_state ^= true)
                RisingEdge?.Invoke();
            else
                FallingEdge?.Invoke();
        }
    }
}