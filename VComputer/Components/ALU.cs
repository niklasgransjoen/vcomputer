using System;

namespace VComputer.Components
{
    public enum ALUMode
    {
        And,
        Or,
        XOr,
        Add,
        Subtract,
    }

    public sealed class ALU : BaseComponent
    {
        private readonly RegA _regA;
        private readonly RegB _regB;

        public ALU(RegA regA, RegB regB)
        {
            _regA = regA;
            _regB = regB;
        }

        public bool Output { get; set; }
        public ALUMode Mode { get; set; }

        #region Callbacks

        protected override void Write()
        {
            if (!Output || Bus is null)
                return;

            ReadOnlySpan<bool> a = _regA.Values.Span;
            ReadOnlySpan<bool> b = _regB.Values.Span;
            Span<bool> result = Bus.Lines;

            switch (Mode)
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

        private static void ClearSpan(in Span<bool> span)
        {
            for (int i = 0; i < span.Length; i++)
            {
                span[i] = false;
            }
        }
    }
}