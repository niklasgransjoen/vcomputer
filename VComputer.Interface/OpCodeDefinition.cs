using System;
using System.Collections.Generic;

namespace VComputer.Interface
{
    public static class OpCodeDefinition
    {
        public static IReadOnlyCollection<Instruction> Definition { get; }

        static OpCodeDefinition()
        {
            var definitions = new[]
            {
                CreateNOP(),
                CreateLDA(),
                CreateSTA(),

                CreateADD(),

                CreateJMP(),

                CreateOUT(),
                CreateHLT(),
            };
            Definition = Array.AsReadOnly(definitions);
        }

        #region Create instructions

        private static Instruction CreateNOP()
        {
            var microInstructions = Array.Empty<ComputerFlags>();
            return CreateInstruction(OpCode.NOP, microInstructions);
        }

        private static Instruction CreateLDA()
        {
            var microInstructions = new ComputerFlags[]
            {
                ComputerFlags.IO | ComputerFlags.MI,
                ComputerFlags.RO | ComputerFlags.AI,
            };

            return CreateInstruction(OpCode.LDA, microInstructions);
        }

        private static Instruction CreateSTA()
        {
            var microInstructions = new ComputerFlags[]
            {
                ComputerFlags.IO | ComputerFlags.MI,
                ComputerFlags.AO | ComputerFlags.RI,
            };

            return CreateInstruction(OpCode.STA, microInstructions);
        }

        private static Instruction CreateADD()
        {
            var microInstructions = new ComputerFlags[]
            {
                ComputerFlags.IO | ComputerFlags.MI,
                ComputerFlags.RO | ComputerFlags.BI,
                ComputerFlags.LM3 | ComputerFlags.LO | ComputerFlags.AI,
            };

            return CreateInstruction(OpCode.ADD, microInstructions);
        }

        private static Instruction CreateJMP()
        {
            var microInstructions = new ComputerFlags[]
            {
                ComputerFlags.IO | ComputerFlags.CI,
            };

            return CreateInstruction(OpCode.JMP, microInstructions);
        }

        private static Instruction CreateOUT()
        {
            var microInstructions = new ComputerFlags[]
            {
                ComputerFlags.AO | ComputerFlags.OI,
            };

            return CreateInstruction(OpCode.OUT, microInstructions);
        }

        private static Instruction CreateHLT()
        {
            var microInstructions = new ComputerFlags[]
            {
                ComputerFlags.HLT,
            };

            return CreateInstruction(OpCode.HLT, microInstructions);
        }

        #endregion Create instructions

        private static Instruction CreateInstruction(OpCode opCode, IEnumerable<ComputerFlags> microInstructions)
        {
            return new Instruction((int)opCode, microInstructions);
        }
    }

    public enum OpCode
    {
        NOP = 0x0,
        LDA = 0x1,
        STA = 0x2,

        ADD = 0x6,

        JMP = 0x9,

        OUT = 0xE,
        HLT = 0xF,
    }
}