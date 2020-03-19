using System;
using System.Threading.Tasks;
using VComputer.Interface.Components;
using VComputer.Interface.Timers;

namespace VComputer.Interface
{
    internal static class Program
    {
        private const int ComputerBitCount = 8;
        private const int DefaultClockInterval = 100;

        private static bool _shutdownHasStarted = false;

        private static async Task Main()
        {
            InitializeProgram();
            await RunComputer();
        }

        private static void InitializeProgram()
        {
            Console.CancelKeyPress += (_, args) =>
            {
                args.Cancel = true;
                _shutdownHasStarted = true;
            };
        }

        private static async Task RunComputer()
        {
            var computerDefinition = new ComputerDefinition
            {
                Bits = ComputerBitCount,
                Inctructions = OpCodeDefinition.Definition,
                Timer = new MultimediaTimer(),
                RAMInitializer = new RAMInitializer(),
                OutputReg = new Display(),
                Debugger = new Debugger(),
            };

            using var computer = new Computer(computerDefinition);
            InputHandler inputHandler = new InputHandler(computer);

            inputHandler.PrintInfo();
            computer.Clock.Interval = DefaultClockInterval;

            while (!_shutdownHasStarted)
            {
                await Task.Delay(100);
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(intercept: true);
                    inputHandler.HandleInput(key);
                }
            }
        }
    }
}