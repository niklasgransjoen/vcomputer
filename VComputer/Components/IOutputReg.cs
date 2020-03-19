namespace VComputer.Components
{
    /// <summary>
    /// Represents an output registry.
    /// </summary>
    public interface IOutputReg : IComponent
    {
        bool Input { get; set; }
    }

    public interface IDebugger
    {
        string Info { get; set; }
    }
}