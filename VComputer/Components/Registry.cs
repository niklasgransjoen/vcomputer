using System;
using VComputer.Util;

namespace VComputer.Components
{
    internal interface IRegistry : IComponent
    {
        ReadOnlyMemory<bool> Values { get; }
    }

    internal abstract class Registry : BaseComponent, IRegistry
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
            ConfigurationUtil.AssertBitCount(_bits, bus.BitCount);
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

    internal sealed class RegA : Registry
    {
        public RegA(int bits) : base(bits)
        {
        }
    }

    internal sealed class RegB : Registry
    {
        public RegB(int bits) : base(bits)
        {
        }
    }

    internal sealed class InstructionReg : Registry
    {
        private readonly Memory<bool> _blanker;
        private readonly int _bits;

        public InstructionReg(int bits) : base(bits)
        {
            _blanker = new bool[bits / 2];
            _bits = bits;
        }

        protected override void Write()
        {
            if (!Output || Bus is null)
                return;

            Memory<bool> lines = Bus.Lines;
            _blanker.CopyTo(lines);
            Values.Slice(_bits / 2).CopyTo(lines.Slice(_bits / 2));
        }
    }
}