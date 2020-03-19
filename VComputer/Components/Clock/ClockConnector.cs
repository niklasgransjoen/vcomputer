using System.Collections.Generic;

namespace VComputer.Components
{
    /// <summary>
    /// Used for connecting components to the clock.
    /// </summary>
    public sealed class ClockConnector
    {
        private readonly List<ClockAction> _clockActions = new List<ClockAction>();

        public ClockConnector()
        {
        }

        public IReadOnlyList<ClockAction> ClockActions => _clockActions.AsReadOnly();

        /// <summary>
        /// Adds action that's executed on the rising edge.
        /// </summary>
        public void AddAction(ClockAction action) => _clockActions.Add(action);
    }
}