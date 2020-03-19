using System;
using System.Collections.Generic;
using VComputer.Components;

namespace VComputer
{
    /// <summary>
    /// Represents the computer.
    /// </summary>
    public sealed class Computer : IDisposable
    {
        private readonly IComponent[] _components;
        private readonly Bus _bus;

        #region Construct & Dispose

        public Computer(ComputerDefinition definition)
        {
            if (definition.Timer is null)
                throw new ArgumentException("A timer is required to create a computer.");

            Clock = new Clock(definition.Timer);
            _bus = new Bus(definition.Bits);

            _components = InitializeComponents(definition);
        }

        private IComponent[] InitializeComponents(ComputerDefinition definition)
        {
            int bits = definition.Bits;

            // Create components.

            var ram = new RAM(bits, definition.RAMInitializer);
            var ramController = new RAMController(ram);
            var instructionReg = new InstructionReg(bits);

            var regA = new RegA(bits);
            var regB = new RegB(bits);
            var alu = new ALU(regA, regB);

            var controlLogic = new ControlLogic(definition, instructionReg);
            var programCounter = new ProgramCounter();

            List<IComponent> components = new List<IComponent>
            {
                ram,
                ramController,
                instructionReg,

                regA,
                regB,
                alu,

                controlLogic,
                programCounter,
            };
            if (definition.OutputReg != null)
            {
                components.Add(definition.OutputReg);
            }

            // Initialize control logic.
            controlLogic.Clock = Clock;
            controlLogic.ProgramCounter = programCounter;
            controlLogic.RAM = ram;
            controlLogic.RAMController = ramController;
            controlLogic.RegA = regA;
            controlLogic.RegB = regB;
            controlLogic.ALU = alu;

            // Initialize components.

            ClockConnector clockConnector = new ClockConnector();
            foreach (var component in components)
            {
                component.Connect(_bus);
                component.Connect(clockConnector);
            }

            foreach (var action in clockConnector.ClockActions)
            {
                Clock.ClockActions.Add(action);
            }

            return components.ToArray();
        }

        public void Dispose()
        {
            Clock.Dispose();
            foreach (var component in _components)
            {
                if (component is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        #endregion Construct & Dispose

        public Clock Clock { get; }
    }
}