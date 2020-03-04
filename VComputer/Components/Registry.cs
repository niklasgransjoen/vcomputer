using System.Collections.Generic;

namespace VComputer.Components
{
    public abstract class Registry : IBusComponent
    {
        private readonly Bus _bus;
        private readonly bool[] _values;

        protected Registry(Bus bus)
        {
            _bus = bus;
            _values = new bool[bus.Lines.Length];
        }

        public IReadOnlyList<bool> Values => _values;

        public void Read()
        {
            _bus.Lines.CopyTo(_values, 0);
        }

        public void Write()
        {
            _values.CopyTo(_bus.Lines, 0);
        }
    }

    public sealed class RegA : Registry
    {
        public RegA(Bus bus) : base(bus)
        {
        }
    }

    public sealed class RegB : Registry
    {
        public RegB(Bus bus) : base(bus)
        {
        }
    }
}