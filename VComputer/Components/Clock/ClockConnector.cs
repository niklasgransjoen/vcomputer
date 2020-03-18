using System.Collections.Generic;

namespace VComputer.Components
{
    /// <summary>
    /// Used for connecting components to the clock.
    /// </summary>
    public sealed class ClockConnector
    {
        private readonly List<ClockAction> _risingEdgeActions = new List<ClockAction>();
        private readonly List<ClockAction> _fallingEdgeActions = new List<ClockAction>();

        public ClockConnector()
        {
        }

        public IReadOnlyList<ClockAction> RisingEdgeActions => _risingEdgeActions;
        public IReadOnlyList<ClockAction> FallingEdgeActions => _fallingEdgeActions;

        /// <summary>
        /// Adds action that's executed on the rising edge.
        /// </summary>
        public void AddRisingEdgeAction(ClockAction action) => _risingEdgeActions.Add(action);

        /// <summary>
        /// Adds action that's executed on the falling edge.
        /// </summary>
        public void AddFallingEdgeAction(ClockAction action) => _fallingEdgeActions.Add(action);
    }
}