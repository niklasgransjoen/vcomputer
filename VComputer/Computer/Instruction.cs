using System;
using System.Collections.Generic;
using System.Linq;

namespace VComputer
{
    /// <summary>
    /// Represents an instruction for the VComputer.
    /// </summary>
    public sealed class Instruction
    {
        public Instruction(int codeValue, IEnumerable<ComputerFlags> microInstructions)
        {
            CodeValue = codeValue;
            MicroInstructions = Array.AsReadOnly(microInstructions.ToArray());
        }

        public int CodeValue { get; }
        public IReadOnlyList<ComputerFlags> MicroInstructions { get; }
    }
}