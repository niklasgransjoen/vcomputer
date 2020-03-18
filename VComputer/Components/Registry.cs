using System;
using VComputer.Util;

namespace VComputer.Components
{
    public abstract class Registry : BaseComponent
    {
        private readonly int _bits;
        private readonly bool[] _values;

        protected Registry(int bits)
        {
            _values = new bool[bits];
            _bits = bits;
        }

        public bool Input { get; set; }
        public bool Output { get; set; }
        public ReadOnlyMemory<bool> Values => _values;

        public override void Connect(Bus bus)
        {
            ConfigurationUtil.AssertBitCount(_bits, bus.Bits);
            base.Connect(bus);
        }

        #region Callbacks

        protected override void Write()
        {
            if (!Output || Bus is null)
                return;

            _values.CopyTo(Bus.Lines, 0);
        }

        protected override void Read()
        {
            if (!Input || Bus is null)
                return;

            Bus.Lines.CopyTo(_values, 0);
        }

        #endregion Callbacks
    }

    public sealed class RegA : Registry
    {
        public RegA(int bits) : base(bits)
        {
        }
    }

    public sealed class RegB : Registry
    {
        public RegB(int bits) : base(bits)
        {
        }
    }
}