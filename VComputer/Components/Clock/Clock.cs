using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace VComputer.Components
{
    public sealed class Clock : IDisposable
    {
        public const int MinClockInterval = 2;
        public const int MaxClockInterval = 20_000;

        private readonly Timer _timer;

        private double _interval;

        // Stores falling or rising edge.
        private bool _state = false;

        #region Construct & Dispose

        public Clock()
        {
            _timer = new Timer(OnTick, state: null, Timeout.Infinite, Timeout.Infinite);
        }

        public void Dispose()
        {
            _timer.Dispose();
        }

        #endregion Construct & Dispose

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

        public ICollection<ClockAction> RisingEdgeActions { get; } = new List<ClockAction>();
        public ICollection<ClockAction> FallingEdgeActions { get; } = new List<ClockAction>();

        #endregion Properties

        #region Methods

        public void Step()
        {
            if (!IsEnabled)
            {
                Tick();
            }
        }

        #endregion Methods

        private void OnTick(object? state)
        {
            if (IsEnabled)
            {
                Tick();
            }
        }

        private void Tick()
        {
            _state ^= true;
            IEnumerable<ClockAction> actions = _state ? RisingEdgeActions : FallingEdgeActions;
            foreach (var action in actions.OrderByDescending(a => a.Priority))
            {
                action.Callback();
            }
        }
    }
}