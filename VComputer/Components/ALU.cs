using System;
using VComputer.Util;

namespace VComputer.Components
{
    internal enum ALUMode
    {
        And = 0x1,
        Or = 0x2,
        XOr = 0x3,

        Add = 0x4,
        Subtract = 0x5,
    }

    internal sealed class ALU : BaseComponent
    {
        private readonly RegA _regA;
        private readonly RegB _regB;

        public ALU(RegA regA, RegB regB)
        {
            _regA = regA;
            _regB = regB;
        }

        public bool Output { get; set; }
        public bool Mode1 { get; set; }
        public bool Mode2 { get; set; }
        public bool Mode3 { get; set; }

        #region Callbacks

        protected override void Write()
        {
            if (!Output || Bus is null)
                return;

            ReadOnlySpan<bool> a = _regA.Values.Span;
            ReadOnlySpan<bool> b = _regB.Values.Span;
            Span<bool> result = Bus.Lines;

            ALUMode mode = GetMode();
            switch (mode)
            {
                case ALUMode.And:
                    And(in a, in b, in result);
                    break;

                case ALUMode.Or:
                    Or(in a, in b, in result);
                    break;

                case ALUMode.XOr:
                    XOr(in a, in b, in result);
                    break;

                case ALUMode.Add:
                    ClearSpan(in result);
                    Add(in a, in b, in result);
                    break;

                default:
                case ALUMode.Subtract:
                    ClearSpan(in result);
                    Sub(in a, in b, in result);
                    break;
            }
        }

        #endregion Callbacks

        #region ALU logic

        private static void And(in ReadOnlySpan<bool> a, in ReadOnlySpan<bool> b, in Span<bool> result)
        {
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = a[i] & b[i];
            }
        }

        private static void Or(in ReadOnlySpan<bool> a, in ReadOnlySpan<bool> b, in Span<bool> result)
        {
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = a[i] | b[i];
            }
        }

        private static void XOr(in ReadOnlySpan<bool> a, in ReadOnlySpan<bool> b, in Span<bool> result)
        {
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = a[i] ^ b[i];
            }
        }

        private static void Add(in ReadOnlySpan<bool> a, in ReadOnlySpan<bool> b, in Span<bool> result)
        {
            bool[] values = new bool[a.Length];

            Span<bool> x = stackalloc bool[result.Length];
            XOr(in a, in b, in x);

            for (int i = result.Length - 1; i >= 0; i--)
            {
                // Carry.
                if (i != 0)
                {
                    result[i - 1] = (a[i] & b[i]) | (x[i] & result[i]);
                }

                // Value.
                result[i] ^= x[i];
            }
        }

        private static void Sub(in ReadOnlySpan<bool> a, in ReadOnlySpan<bool> b, in Span<bool> result)
        {
            Span<bool> bInv = stackalloc bool[b.Length];
            for (int i = 0; i < b.Length; i++)
            {
                bInv[i] = !b[i];
            }

            // Set least significant bit in result because bInt should be incremented by one.
            result[^1] = true;

            ReadOnlySpan<bool> readOnlyBInv = bInv;
            Add(in a, in readOnlyBInv, in result);
        }

        #endregion ALU logic

        #region Utilities

        private ALUMode GetMode()
        {
            Span<bool> span = stackalloc bool[3];
            span[0] = Mode3;
            span[1] = Mode2;
            span[2] = Mode1;

            return (ALUMode)MemoryUtil.ToInt(span);
        }

        private static void ClearSpan(in Span<bool> span)
        {
            for (int i = 0; i < span.Length; i++)
            {
                span[i] = false;
            }
        }

        #endregion Utilities
    }
}