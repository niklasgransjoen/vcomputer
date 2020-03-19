using System;

namespace VComputer.Components
{
    /// <summary>
    /// Component used to initialize the RAM of the computer.
    /// </summary>
    public interface IRAMInitializer
    {
        void InitializeRAM(ReadOnlySpan<bool[]> memory);
    }
}