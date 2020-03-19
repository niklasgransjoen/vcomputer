using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VComputer.Util;

namespace VComputer.Components
{
    /// <summary>
    /// Represents the control logic of the computer.
    /// </summary>
    public sealed class ControlLogic : IComponent
    {
        private readonly int _bits;
        private readonly Dictionary<int, IReadOnlyList<ComputerFlags>> _instructions;
        private int _microInstructionPointer = 0;
        private readonly StringBuilder _debuggerInfoBuilder = null!;

        public ControlLogic(ComputerDefinition definition, InstructionReg instructionReg)
        {
            _bits = definition.Bits;
            _instructions = definition.Inctructions.ToDictionary(i => i.CodeValue, i => i.MicroInstructions);

            InstructionReg = instructionReg;
            OutputReg = definition.OutputReg;
            if ((Debugger = definition.Debugger) != null)
            {
                _debuggerInfoBuilder = new StringBuilder();
            }
        }

        #region Components

        public Clock? Clock { get; set; }
        public ProgramCounter? ProgramCounter { get; set; }
        public RAM? RAM { get; set; }
        public RAMController? RAMController { get; set; }
        public InstructionReg InstructionReg { get; }
        public RegA? RegA { get; set; }
        public RegB? RegB { get; set; }
        public ALU? ALU { get; set; }
        public IOutputReg? OutputReg { get; }
        public IDebugger? Debugger { get; }

        #endregion Components

        public void Connect(ClockConnector clock)
        {
            clock.AddAction(OnTick, ClockPriorities.ControlLogic);
        }

        public void Connect(Bus bus)
        {
            // The control logic is not connected to the bus.
        }

        #region Callbacks

        private void OnTick()
        {
            if (_microInstructionPointer == 0)
            {
                LookUpInstruction();
            }
            else if (_microInstructionPointer == 1)
            {
                FetchInstruction();
            }
            else
            {
                // Execute fetched instruction.

                // Attempt to retrieve the instruction.
                var instructionValue = InstructionReg.Values.Slice(0, _bits / 2);
                int instruction = MemoryUtil.ToInt(instructionValue);
                if (!_instructions.TryGetValue(instruction, out var microInstructions) ||
                    microInstructions.Count == 0)
                {
                    // Instruction does not exist, or has zero micro instructions.

                    // To avoid wasting this clock cycle, perform lookup and skip straight to step fetch on the next tick.
                    LookUpInstruction();
                    _microInstructionPointer = 1;
                    return;
                }

                // Set the correct flags.
                int currentMicroInstructionIndex = _microInstructionPointer - 2; // subtract the two fetch instructions.
                var currentInstruction = microInstructions[currentMicroInstructionIndex];
                SetFlags(currentInstruction);

                // On the last instruction
                if (currentMicroInstructionIndex == microInstructions.Count - 1)
                {
                    ResetInstructionPointer();
                    return;
                }
            }

            ++_microInstructionPointer;
        }

        private void FetchInstruction()
        {
            SetFlags(ComputerFlags.RO | ComputerFlags.II | ComputerFlags.CE);
        }

        private void LookUpInstruction()
        {
            SetFlags(ComputerFlags.CO | ComputerFlags.MI);
        }

        private void ResetInstructionPointer()
        {
            _microInstructionPointer = 0;
        }

        #endregion Callbacks

        private void SetFlags(ComputerFlags flags)
        {
            TrySet(flags, ComputerFlags.AI, RegA, (regA, value) => regA.Input = value);
            TrySet(flags, ComputerFlags.AO, RegA, (regA, value) => regA.Output = value);
            TrySet(flags, ComputerFlags.BI, RegB, (regB, value) => regB.Input = value);
            TrySet(flags, ComputerFlags.BO, RegB, (regB, value) => regB.Output = value);
            TrySet(flags, ComputerFlags.II, InstructionReg, (instructionReg, value) => instructionReg.Input = value);
            TrySet(flags, ComputerFlags.IO, InstructionReg, (instructionReg, value) => instructionReg.Output = value);
            TrySet(flags, ComputerFlags.RI, RAM, (ram, value) => ram.Input = value);
            TrySet(flags, ComputerFlags.RO, RAM, (ram, value) => ram.Output = value);
            TrySet(flags, ComputerFlags.MI, RAMController, (ramController, value) => ramController.Input = value);
            TrySet(flags, ComputerFlags.LO, ALU, (alu, value) => alu.Output = value);
            TrySet(flags, ComputerFlags.LM1, ALU, (alu, value) => alu.Mode1 = value);
            TrySet(flags, ComputerFlags.LM2, ALU, (alu, value) => alu.Mode2 = value);
            TrySet(flags, ComputerFlags.LM3, ALU, (alu, value) => alu.Mode3 = value);
            TrySet(flags, ComputerFlags.CE, ProgramCounter, (counter, value) => counter.Enable = value);
            TrySet(flags, ComputerFlags.CI, ProgramCounter, (counter, value) => counter.Input = value);
            TrySet(flags, ComputerFlags.CO, ProgramCounter, (counter, value) => counter.Output = value);
            TrySet(flags, ComputerFlags.OI, OutputReg, (outputReg, value) => outputReg.Input = value);
            TrySet(flags, ComputerFlags.HLT, Clock, (clock, value) => clock.Halt = value);

            if (Debugger != null)
            {
                _debuggerInfoBuilder.AppendLine($"Instructions:   {flags}");
                _debuggerInfoBuilder.AppendLine($"RAMAddress:     0x{RAM?.Address:X2}");
                _debuggerInfoBuilder.AppendLine($"RAM:            {MemoryUtil.ToString(RAM?.Values ?? Array.Empty<bool>())}");
                _debuggerInfoBuilder.AppendLine($"RegA:           {MemoryUtil.ToString(RegA?.Values ?? Array.Empty<bool>())}");
                _debuggerInfoBuilder.AppendLine($"RegB:           {MemoryUtil.ToString(RegB?.Values ?? Array.Empty<bool>())}");
                _debuggerInfoBuilder.AppendLine($"InstructionReg: {MemoryUtil.ToString(InstructionReg?.Values ?? Array.Empty<bool>())}");
                _debuggerInfoBuilder.AppendLine($"ProgramCounter: 0x{ProgramCounter?.Value:X2}");

                Debugger.Info = _debuggerInfoBuilder.ToString();
                _debuggerInfoBuilder.Clear();
            }
        }

        private static void TrySet<TComponent>(ComputerFlags flags, ComputerFlags check, TComponent? component, Action<TComponent, bool> setter)
            where TComponent : class
        {
            if (component != null)
            {
                bool value = flags.HasFlag(check);
                setter(component, value);
            }
        }
    }
}