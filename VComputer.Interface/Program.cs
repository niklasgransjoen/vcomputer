using System;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using VComputer.Assembler;
using VComputer.Assembler.Text;
using VComputer.Interface.Components;
using VComputer.Interface.Helpers;
using VComputer.Interface.Timers;

namespace VComputer.Interface
{
    internal static class Program
    {
        private const string ProgramPath = "program.asm";

        private const int ComputerBitCount = 8;
        private const int DefaultClockInterval = 100;

        private static bool _shutdownHasStarted = false;

        private static async Task Main()
        {
            InitializeApplication();
            var program = ReadProgram(ProgramPath);
            await RunComputer(program);
        }

        private static void InitializeApplication()
        {
            Console.CancelKeyPress += (_, args) =>
            {
                args.Cancel = true;
                _shutdownHasStarted = true;
            };
        }

        #region ReadProgram

        private static int[] ReadProgram(string path)
        {
            var assembler = new Assembler.Assembler(LanguageDefinition.AssemblyDefinition);
            if (!File.Exists(path))
            {
                ConsoleHelper.WriteLine($"Failed to locate program (file '{path}').", ConsoleColor.Red);
                return Array.Empty<int>();
            }

            string program = File.ReadAllText(path);
            try
            {
                return assembler.Assemble(program, ComputerBitCount);
            }
            catch (AssemblyDiagnosticException ex)
            {
                HandleDiagonstics(ex.Text, ex.Diagnostics);
                ConsoleHelper.Block();
                throw; // never reached.
            }
        }

        private static void HandleDiagonstics(SourceText text, ImmutableArray<Diagnostic> diagnostics)
        {
            for (int i = 0; i < diagnostics.Length; i++)
            {
                var diagnostic = diagnostics[i];

                int lineIndex = text.GetLineIndex(diagnostic.Span.Start);
                TextLine line = text.Lines.Span[lineIndex];
                int lineNumer = lineIndex + 1;
                int character = diagnostic.Span.Start - line.Start + 1;

                Console.WriteLine();
                ConsoleHelper.WriteLine($"({lineNumer}, {character}): {diagnostic}", ConsoleColor.DarkRed);

                TextSpan prefixSpan = TextSpan.FromBounds(line.Start, diagnostic.Span.Start);
                TextSpan suffixSpan = TextSpan.FromBounds(diagnostic.Span.End, line.End);

                string prefix = text.SubString(prefixSpan).ToString();
                string error = text.SubString(diagnostic.Span).ToString();
                string suffix = text.SubString(suffixSpan).ToString();

                Console.Write("    ");
                Console.Write(prefix);

                ConsoleHelper.Write(error, ConsoleColor.DarkRed);

                Console.Write(suffix);
                Console.WriteLine();

                // Arrow pointing at offending area.
                Console.Write("   " + new string(' ', character));
                ConsoleHelper.Write("^", ConsoleColor.DarkRed);
            }

            Console.WriteLine();
        }

        #endregion ReadProgram

        private static async Task RunComputer(int[] program)
        {
            var computerDefinition = new ComputerDefinition
            {
                Bits = ComputerBitCount,
                Inctructions = LanguageDefinition.InstructionDefinition,
                Timer = new MultimediaTimer(),
                RAMInitializer = new RAMInitializer(program),
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