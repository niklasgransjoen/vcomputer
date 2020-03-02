using System;
using VComputer.Components;

namespace VComputer
{
    internal static class Program
    {
        private const int ClockInterval = 1000;

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
            using var clock = new Clock(ClockInterval);

            while (!_shutdownHasStarted)
            {
                if (!Console.KeyAvailable)
                    continue;

                var key = Console.ReadKey(intercept: true);
                switch (key.Key)
                {
                    case ConsoleKey.T:
                        clock.IsEnabled ^= true;
                        break;

                    case ConsoleKey.S:
                        clock.Step();
                        break;

                    case ConsoleKey.LeftArrow:
                        clock.Interval /= ClockIntervalStep;
                        break;

                    case ConsoleKey.RightArrow:
                        clock.Interval *= ClockIntervalStep;
                        break;
                }
            }
        }
    }
}