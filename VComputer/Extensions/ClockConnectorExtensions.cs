using System;

namespace VComputer.Components
{
    public static class ClockConnectorExtensions
    {
        public static void AddAction(this ClockConnector connector, Action action, int priority)
        {
            if (connector is null) throw new ArgumentNullException(nameof(connector));
            if (action is null) throw new ArgumentNullException(nameof(action));

            connector.AddAction(new ClockAction(action, priority));
        }
    }
}