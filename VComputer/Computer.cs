using System;
using VComputer.Components;

namespace VComputer
{
    /// <summary>
    /// Represents the computer.
    /// </summary>
    public sealed class Computer : IDisposable
    {
        private const int ClockInterval = 1000;

        private const double ClockIntervalStep = 1.1d;

        private readonly Clock _clock;
        private readonly Bus _bus;
        private readonly RegA _regA;
        private readonly RegB _regB;
        private readonly ALU _alu;

        #region Construct & Dispose

        public Computer(int bits)
        {
            _clock = new Clock(ClockInterval);
            _bus = new Bus(bits);

            _regA = new RegA(_bus);
            _regB = new RegB(_bus);
            _alu = new ALU(_regA, _regB, _bus);

            SubscribeToEvents();
        }

        public void Dispose()
        {
            _clock.Dispose();

            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
        }

        private void UnsubscribeFromEvents()
        {
        }

        #endregion Construct & Dispose

        public void HandleInput(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.T:
                    _clock.IsEnabled ^= true;
                    break;

                case ConsoleKey.S:
                    _clock.Step();
                    break;

                case ConsoleKey.LeftArrow:
                    _clock.Interval /= ClockIntervalStep;
                    break;

                case ConsoleKey.RightArrow:
                    _clock.Interval *= ClockIntervalStep;
                    break;
            }
        }
    }
}