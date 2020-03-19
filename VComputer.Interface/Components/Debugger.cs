using System;
using VComputer.Components;
using VComputer.Interface.Helpers;

namespace VComputer.Interface.Components
{
    public sealed class Debugger : IDebugger
    {
        public Debugger()
        {
        }

        private string _info = string.Empty;

        public string Info
        {
            get => _info;
            set
            {
                _info = value;
                PrintInfo();
            }
        }

        private void PrintInfo()
        {
            int cursorPos = Console.CursorTop;

            ++Console.CursorTop;

            ConsoleHelper.ClearLines(10);
            ConsoleHelper.Write(_info, ConsoleColor.Red);

            Console.CursorTop = cursorPos;
        }
    }
}