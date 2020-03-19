using System;
using System.Collections.Generic;
using VComputer.Components;
using VComputer.Services;

namespace VComputer
{
    public sealed class ComputerDefinition
    {
        public ComputerDefinition()
        {
        }

        /// <summary>
        /// Gets or sets the number of bits for a computer. Must be a positive, even number.
        /// </summary>
        public int Bits { get; set; }

        /// <summary>
        /// Gets or sets the op-code definitions of this computer.
        /// </summary>
        public IReadOnlyCollection<Instruction> Inctructions { get; set; } = Array.Empty<Instruction>();

        /// <summary>
        /// Gets or sets an optional timer for the computer to use. If none is provided, an internal implementation will be used.
        /// </summary>
        /// <remarks>
        /// The timer will be disposed when the computer is disposed.
        /// </remarks>
        public ITimer? Timer { get; set; }

        /// <summary>
        /// Gets or sets an optional output registry to connect to the computer.
        /// </summary>
        public IOutputReg? OutputReg { get; set; }

        /// <summary>
        /// Gets or sets an optional debugger.
        /// </summary>
        public IDebugger? Debugger { get; set; }

        /// <summary>
        /// Gets or sets an optional RAM initializer.
        /// </summary>
        public IRAMInitializer? RAMInitializer { get; set; }
    }
}