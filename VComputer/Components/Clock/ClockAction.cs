using System;

namespace VComputer.Components
{
    /// <summary>
    /// Represents an action to be performed by the clock.
    /// </summary>
    public sealed class ClockAction
    {
        public ClockAction(Action callback, int priority)
        {
            Callback = callback ?? throw new ArgumentNullException(nameof(callback));
            Priority = priority;
        }

        /// <summary>
        /// The action to execute.
        /// </summary>
        public Action Callback { get; }

        /// <summary>
        /// The priority of the action. Actions are executed from highest to lowest priority.
        /// </summary>
        public int Priority { get; }
    }
}