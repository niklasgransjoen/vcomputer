using System;
using System.Collections.Generic;

namespace VComputer.Interface
{
    internal sealed class InputHandler
    {
        private const double ClockIntervalStep = 1.1d;

        private readonly Computer _computer;
        private readonly Dictionary<ConsoleKey, ConsoleAction> _inputMappings = new Dictionary<ConsoleKey, ConsoleAction>();

        #region Constructor

        public InputHandler(Computer computer)
        {
            _computer = computer;

            InitializeInputMappings();
        }

        private void InitializeInputMappings()
        {
            _inputMappings[ConsoleKey.T] = new ConsoleAction(ToggleClock, "Toggle clock");
            _inputMappings[ConsoleKey.S] = new ConsoleAction(ClockStep, "Single-step clock");
            _inputMappings[ConsoleKey.LeftArrow] = new ConsoleAction(DecrementClockInterval, "Decrease clock speed");
            _inputMappings[ConsoleKey.RightArrow] = new ConsoleAction(IncrementClockInterval, "Increase clock speed");
        }

        #endregion Constructor

        public void PrintInfo()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Console input interface");

            foreach (var inputMapping in _inputMappings)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("Key " + inputMapping.Key + " ");

                Console.ResetColor();
                Console.WriteLine(inputMapping.Value.Description);
            }
            Console.WriteLine();
        }

        public void HandleInput(ConsoleKeyInfo keyInfo)
        {
            if (_inputMappings.TryGetValue(keyInfo.Key, out var action))
            {
                action.Action();
            }
        }

        #region Actions

        private void ToggleClock()
        {
            _computer.Clock.IsEnabled ^= true;
        }

        private void ClockStep()
        {
            _computer.Clock.Step();
        }

        private void DecrementClockInterval()
        {
            _computer.Clock.Interval /= ClockIntervalStep;
        }

        private void IncrementClockInterval()
        {
            _computer.Clock.Interval *= ClockIntervalStep;
        }

        #endregion Actions
    }
}