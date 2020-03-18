using System;
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
            Clock = new Clock();
            _bus = new Bus(definition.Bits);

            _components = InitializeComponents(definition.Bits);
        }

        private IComponent[] InitializeComponents(int bits)
        {
            var programCounter = new ProgramCounter();
            var ram = new RAM(bits);
            var ramController = new RAMController(ram);

            var regA = new RegA(bits);
            var regB = new RegB(bits);
            var alu = new ALU(regA, regB);

            IComponent[] components = new IComponent[]
            {
                programCounter,
                ram,
                ramController,

                regA,
                regB,
                alu,
            };

            ClockConnector clockConnector = new ClockConnector();
            foreach (var component in components)
            {
                component.Connect(_bus);
                component.Connect(clockConnector);
            }

            foreach (var action in clockConnector.RisingEdgeActions)
            {
                Clock.RisingEdgeActions.Add(action);
            }
            foreach (var action in clockConnector.FallingEdgeActions)
            {
                Clock.FallingEdgeActions.Add(action);
            }

            return components;
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