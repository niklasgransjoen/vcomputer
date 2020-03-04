namespace VComputer.Components
{
    /// <summary>
    /// Represents a component able to read and write from the bus.
    /// </summary>
    public interface IBusComponent
    {
        void Read();

        void Write();
    }
}