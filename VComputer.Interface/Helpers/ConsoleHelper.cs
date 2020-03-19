using System;

namespace VComputer.Interface.Helpers
{
    public static class ConsoleHelper
    {
        public static void ClearLine()
        {
            Console.CursorLeft = 0;
            Console.Write(new string(' ', Console.WindowWidth - 1));
            Console.CursorLeft = 0;
        }

        public static void ClearLines(int lineCount)
        {
            int cursorTop = Console.CursorTop;
            for (int i = 0; i < lineCount; i++)
            {
                ClearLine();
                ++Console.CursorTop;
            }
            Console.CursorTop = cursorTop;
        }

        public static void Write(string value, ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;

            Console.Write(value);

            Console.ResetColor();
        }

        public static void WriteLine(string value, ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
        {
            Write(value, foreground, background);
            Console.WriteLine();
        }

        public static void Block()
        {
            while (true)
            {
            }
        }
    }
}