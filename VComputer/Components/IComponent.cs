namespace VComputer.Components
{
    /// <summary>
    /// Represents a VComputer component.
    /// </summary>
    public interface IComponent
    {
        /// <summary>
        /// Connects a component to a clock.
        /// </summary>
        void Connect(ClockConnector clock);

        /// <summary>
        /// Connects a component to a bus.
        /// </summary>
        void Connect(Bus bus);
    }
}