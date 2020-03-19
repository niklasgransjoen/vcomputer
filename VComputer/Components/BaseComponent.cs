namespace VComputer.Components
{
    /// <summary>
    /// Base implementation of <see cref="IComponent"/>.
    /// </summary>
    public abstract class BaseComponent : IComponent
    {
        protected Bus? Bus { get; set; }

        public BaseComponent()
        {
        }

        public virtual void Connect(ClockConnector clock)
        {
            clock.AddAction(Write, ClockPriorities.Write);
            clock.AddAction(Read, ClockPriorities.Read);
        }

        public virtual void Connect(Bus bus)
        {
            Bus = bus;
        }

        #region Callbacks

        protected virtual void Write()
        {
        }

        protected virtual void Read()
        {
        }

        #endregion Callbacks
    }
}