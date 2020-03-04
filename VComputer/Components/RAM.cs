using System;

namespace VComputer.Components
{
    public sealed class RAM : IBusComponent
    {
        private readonly Bus _bus;
        private readonly bool[][] _values;

        public RAM(Bus bus)
        {
            _bus = bus;

            // Init ram.
            int ramSize = (int)Math.Pow(2, bus.Lines.Length);
            _values = new bool[ramSize][];
            for (int i = 0; i < ramSize; i++)
            {
                _values[i] = new bool[bus.Lines.Length];
            }
        }

        public int Address { get; set; }

        public void Read()
        {
            _bus.Lines.CopyTo(_values[Address], 0);
        }

        public void Write()
        {
            _values[Address].CopyTo(_bus.Lines, 0);
        }
    }

    public sealed class RAMController : IBusComponent
    {
        private readonly RAM _ram;
        private readonly Bus _bus;

        public RAMController(RAM ram, Bus bus)
        {
            _ram = ram;
            _bus = bus;
        }

        public void Read()
        {
        }

        public void Write()
        {
            throw new InvalidOperationException("");
        }
    }
}