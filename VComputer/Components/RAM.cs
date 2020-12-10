using System;
using VComputer.Util;

namespace VComputer.Components
{
    internal sealed class RAM : BaseComponent
    {
        private readonly int _bits;
        private readonly bool[][] _values;

        public RAM(int bits, IRAMInitializer? initializer)
        {
            _bits = bits;

            // Init ram.
            int ramSize = (int)Math.Pow(2, bits);
            _values = new bool[ramSize][];
            for (int i = 0; i < ramSize; i++)
            {
                _values[i] = new bool[bits];
            }

            initializer?.InitializeRAM(_values);
        }

        public bool Input { get; set; }
        public bool Output { get; set; }
        public int Address { get; set; }
        public ReadOnlyMemory<bool> Values => _values[Address];

        public override void Connect(Bus bus)
        {
            ConfigurationUtil.AssertBitCount(_bits, bus.BitCount);
            base.Connect(bus);
        }

        #region Callback

        protected override void Write()
        {
            if (!Output || Bus is null)
                return;

            _values[Address].CopyTo(Bus.Lines, 0);
        }

        protected override void Read()
        {
            if (!Input || Bus is null)
                return;

            Bus.Lines.CopyTo(_values[Address], 0);
        }

        #endregion Callback
    }
}