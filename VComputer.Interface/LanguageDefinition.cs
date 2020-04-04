using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using VComputer.Assembler;

namespace VComputer.Interface
{
    public static class LanguageDefinition
    {
        public static ImmutableArray<Instruction> InstructionDefinition { get; }
        public static ImmutableArray<AssemblyInstruction> AssemblyDefinition { get; }

        static LanguageDefinition()
        {
            InstructionDefinition = new[]
            {
                CreateNOP(),
                CreateLDA(),
                CreateSTA(),

                CreateADD(),

                CreateJMP(),

                CreateOUT(),
                CreateHLT(),
            }.ToImmutableArray();

            AssemblyDefinition = new[]
            {
                CreateAssemblyInstruction("NOP", OpCode.NOP),
                CreateAssemblyInstruction("LDA", OpCode.LDA),
                CreateAssemblyInstruction("STA", OpCode.STA),

                CreateAssemblyInstruction("ADD", OpCode.ADD),

                CreateAssemblyInstruction("JMP", OpCode.JMP),

                CreateAssemblyInstruction("OUT", OpCode.OUT),
                CreateAssemblyInstruction("HLT", OpCode.HLT),
            }.ToImmutableArray();
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

        private static AssemblyInstruction CreateAssemblyInstruction(string command, OpCode opCode)
        {
            return new AssemblyInstruction(command, (int)opCode);
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