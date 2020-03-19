using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using VComputer.Services;

namespace VComputer.Interface.Timers
{
    /// <summary>
    /// A timer based on the multimedia timer API with 1ms precision.
    /// </summary>
    public sealed class MultimediaTimer : ITimer
    {
        private const int EventTypeSingle = 0;
        private const int EventTypePeriodic = 1;

        private bool _disposed = false;
        private int _interval, _resolution;
        private volatile uint _timerId;

        // Hold the timer callback to prevent garbage collection.
        private readonly MultimediaTimerCallback _callback;

        #region Construct & Dispose

        public MultimediaTimer()
        {
            _callback = new MultimediaTimerCallback(TimerCallbackMethod);
            Resolution = 5;
            Interval = 10;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            if (IsRunning)
            {
                StopInternal();
            }
        }

        #endregion Construct & Dispose

        public static Task Delay(int millisecondsDelay, CancellationToken token = default)
        {
            if (millisecondsDelay < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(millisecondsDelay), millisecondsDelay, "The value cannot be less than 0.");
            }

            if (millisecondsDelay == 0)
            {
                return Task.CompletedTask;
            }

            token.ThrowIfCancellationRequested();

            // allocate an object to hold the callback in the async state.
            MultimediaTimerCallback[] state = new MultimediaTimerCallback[1];
            var completionSource = new TaskCompletionSource<object?>(state);
            state[0] = (uint id, uint msg, ref uint uCtx, uint rsv1, uint rsv2) =>
            {
                // Note we don't need to kill the timer for one-off events.
                completionSource.TrySetResult(null);
            };

            uint userCtx = 0;
            var timerId = TimeSetEvent((uint)millisecondsDelay, 0, state[0], ref userCtx, EventTypeSingle);

            return completionSource.Task;
        }

        #region Events

        public event Action? Tick;

        #endregion Events

        /// <summary>
        /// The period of the timer in milliseconds.
        /// </summary>
        public int Interval
        {
            get => _interval;
            set
            {
                ThrowIfDisposed();

                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(Interval));

                _interval = value;
                if (Resolution > Interval)
                    Resolution = value;

                if (IsRunning)
                {
                    StopInternal();
                    StartInternal();
                }
            }
        }

        /// <summary>
        /// The resolution of the timer in milliseconds. The minimum resolution is 0, meaning highest possible resolution.
        /// </summary>
        public int Resolution
        {
            get => _resolution;
            set
            {
                ThrowIfDisposed();

                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(Resolution));

                _resolution = value;
            }
        }

        /// <summary>
        /// Gets whether the timer has been started yet.
        /// </summary>
        public bool IsRunning => _timerId != 0;

        public void Start()
        {
            ThrowIfDisposed();
            if (IsRunning)
                return;

            StartInternal();
        }

        public void Stop()
        {
            ThrowIfDisposed();
            if (!IsRunning)
                return;

            StopInternal();
        }

        private void StartInternal()
        {
            uint userCtx = 0;
            _timerId = TimeSetEvent((uint)Interval, (uint)Resolution, _callback, ref userCtx, EventTypePeriodic);
        }

        private void StopInternal()
        {
            NativeMethods.TimeKillEvent(_timerId);
            _timerId = 0;
        }

        private void TimerCallbackMethod(uint id, uint msg, ref uint userCtx, uint rsv1, uint rsv2)
        {
            Tick?.Invoke();
        }

        #region Helpers

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(MultimediaTimer));
        }

        #endregion Helpers

        #region Utilities

        private static uint TimeSetEvent(uint msDelay, uint msResolution, MultimediaTimerCallback callback, ref uint userCtx, uint eventType)
        {
            uint timerId = NativeMethods.TimeSetEvent(msDelay, msResolution, callback, ref userCtx, eventType);
            if (timerId == 0)
            {
                int error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error);
            }

            return timerId;
        }

        #endregion Utilities
    }

    internal delegate void MultimediaTimerCallback(uint id, uint msg, ref uint userCtx, uint rsv1, uint rsv2);

    internal static class NativeMethods
    {
        private const string DLL = "winmm.dll";

        [DllImport(DLL, SetLastError = true, EntryPoint = "timeSetEvent")]
        internal static extern uint TimeSetEvent(uint msDelay, uint msResolution, MultimediaTimerCallback callback, ref uint userCtx, uint eventType);

        [DllImport(DLL, SetLastError = true, EntryPoint = "timeKillEvent")]
        internal static extern void TimeKillEvent(uint uTimerId);
    }
}