using System;
using VComputer.Components;
using VComputer.Interface.Helpers;
using VComputer.Util;

namespace VComputer.Interface.Components
{
    internal sealed class Display : BaseComponent, IOutputReg
    {
        public Display()
        {
            Console.CursorVisible = false;
        }

        public bool Input { get; set; }

        public override void Connect(ClockConnector clock)
        {
            clock.AddAction(Print, ClockPriorities.Read);
        }

        #region Callbacks

        private void Print()
        {
            if (!Input || Bus is null)
                return;

            int value = MemoryUtil.ToInt(Bus.Lines);

            ConsoleHelper.ClearLine();
            ConsoleHelper.Write($"Output: {value}", ConsoleColor.Green);
        }

        #endregion Callbacks
    }
}