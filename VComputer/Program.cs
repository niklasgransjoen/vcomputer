using System;

namespace VComputer
{
    internal static class Program
    {
        public const int ComputerBitCount = 8;

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
            using var computer = new Computer(ComputerBitCount);

            while (!_shutdownHasStarted)
            {
                if (!Console.KeyAvailable)
                    continue;

                var key = Console.ReadKey(intercept: true);
                computer.HandleInput(key);
            }
        }
    }
}