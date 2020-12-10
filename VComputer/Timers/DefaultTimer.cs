using System;
using System.Threading;
using VComputer.Services;

namespace VComputer.Timers
{
    internal sealed class DefaultTimer : ITimer
    {
        private readonly Timer _timer;
        private int _interval;
        private bool _disposed;

        public DefaultTimer()
        {
            _timer = new Timer(OnTick, null, Timeout.Infinite, Timeout.Infinite);
        }

        public void Dispose()
        {
            _disposed = true;
            _timer.Dispose();
        }

        public int Interval
        {
            get => _interval;
            set
            {
                ThrowIfDisposed();

                _interval = value;
                if (IsRunning)
                {
                    Start();
                }
            }
        }

        public bool IsRunning { get; private set; }

        public event Action? Tick;

        public void Start()
        {
            ThrowIfDisposed();

            IsRunning = true;
            _timer.Change(_interval, _interval);
        }

        public void Stop()
        {
            ThrowIfDisposed();

            IsRunning = false;
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void OnTick(object? state)
        {
            Tick?.Invoke();
        }

        #region Helpers

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(DefaultTimer));
        }

        #endregion Helpers
    }
}