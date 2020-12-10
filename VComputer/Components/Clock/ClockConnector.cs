using System.Collections.Generic;
using System.Collections.Immutable;

namespace VComputer.Components
{
    /// <summary>
    /// Used for connecting components to the clock.
    /// </summary>
    public sealed class ClockConnector
    {
        private readonly List<ClockAction> _clockActions = new List<ClockAction>();

        internal ClockConnector()
        {
        }

        internal ImmutableArray<ClockAction> ClockActions => _clockActions.ToImmutableArray();

        /// <summary>
        /// Adds action that's executed on the rising edge.
        /// </summary>
        public void AddAction(ClockAction action) => _clockActions.Add(action);
    }
}