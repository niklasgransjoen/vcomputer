using System;

namespace VComputer.Interface
{
    internal sealed class ConsoleAction
    {
        public ConsoleAction(Action action, string description)
        {
            Action = action ?? throw new ArgumentNullException(nameof(action));
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }

        public Action Action { get; }

        public string Description { get; }
    }
}