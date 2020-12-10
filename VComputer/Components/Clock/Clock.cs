using System;
using System.Collections.Generic;
using System.Linq;
using VComputer.Services;
using VComputer.Timers;

namespace VComputer.Components
{
    public sealed class Clock : IDisposable
    {
        internal const int MinClockInterval = 1;
        internal const int MaxClockInterval = 10_000;

        private readonly ITimer _timer;

        private double _interval;

        #region Construct & Dispose

        internal Clock(ITimer? timer)
        {
            _timer = timer ?? new DefaultTimer();
            _timer.Tick += OnTick;

            _timer.Start();
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
        public bool IsEnabled { get; set; } = false;

        /// <summary>
        /// Gets or sets the interval of the clock.
        /// </summary>
        public double Interval
        {
            get => _interval;
            set
            {
                _interval = Math.Min(MaxClockInterval, Math.Max(MinClockInterval, value));
                _timer.Interval = (int)_interval;
            }
        }

        internal bool Halt { get; set; }
        internal IList<ClockAction> ClockActions { get; } = new List<ClockAction>();

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

        private readonly object _timerLock = new object();
        private bool _timerClockBusy;

        private void OnTick()
        {
            if (IsEnabled && !_timerClockBusy)
            {
                lock (_timerLock)
                {
                    _timerClockBusy = true;
                    Tick();
                    _timerClockBusy = false;
                }
            }
        }

        private IEnumerable<Action>? _cachedActions;

        private void Tick()
        {
            if (Halt)
            {
                return;
            }

            if (_cachedActions is null)
            {
                _cachedActions = ClockActions.OrderByDescending(a => a.Priority).Select(a => a.Callback).ToArray();
            }

            foreach (var action in _cachedActions)
            {
                action();
            }
        }
    }
}