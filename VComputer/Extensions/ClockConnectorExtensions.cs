using System;

namespace VComputer.Components
{
    public static class ClockConnectorExtensions
    {
        public static void AddRisingEdgeAction(this ClockConnector connector, Action action, int priority)
        {
            if (connector is null) throw new ArgumentNullException(nameof(connector));
            if (action is null) throw new ArgumentNullException(nameof(action));

            connector.AddRisingEdgeAction(new ClockAction(action, priority));
        }

        public static void AddFallingEdgeAction(this ClockConnector connector, Action action, int priority)
        {
            if (connector is null) throw new ArgumentNullException(nameof(connector));
            if (action is null) throw new ArgumentNullException(nameof(action));

            connector.AddFallingEdgeAction(new ClockAction(action, priority));
        }
    }
}