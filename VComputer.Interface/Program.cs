using System;

namespace VComputer.Interface
{
    internal static class Program
    {
        private const int ComputerBitCount = 8;
        private const int DefaultClockInterval = 1000;
        private const double ClockIntervalStep = 1.1d;

        private static bool _shutdownHasStarted = false;

        private static void Main()
        {
            InitializeProgram();
            RunComputer();
        }

        private static void InitializeProgram()
        {
            Console.CancelKeyPress += (_, args) =>
            {
                args.Cancel = true;
                _shutdownHasStarted = true;
            };
        }

        private static void RunComputer()
        {
            var computerDefinition = new ComputerDefinition
            {
                Bits = ComputerBitCount,
            };

            using var computer = new Computer(computerDefinition);
            computer.Clock.Interval = DefaultClockInterval;

            while (!_shutdownHasStarted)
            {
                if (!Console.KeyAvailable)
                    continue;

                var key = Console.ReadKey(intercept: true);
                HandleInput(computer, key);
            }
        }

        private static void HandleInput(Computer computer, ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.T:
                    computer.Clock.IsEnabled ^= true;
                    break;

                case ConsoleKey.S:
                    computer.Clock.Step();
                    break;

                case ConsoleKey.LeftArrow:
                    computer.Clock.Interval /= ClockIntervalStep;
                    break;

                case ConsoleKey.RightArrow:
                    computer.Clock.Interval *= ClockIntervalStep;
                    break;
            }
        }
    }
}