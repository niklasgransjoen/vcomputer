using System;
using System.Linq;

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

    public sealed class ALU : IBusComponent
    {
        private readonly RegA _regA;
        private readonly RegB _regB;
        private readonly Bus _bus;

        public ALU(RegA regA, RegB regB, Bus bus)
        {
            _regA = regA;
            _regB = regB;
            _bus = bus;
        }

        public ALUMode Mode { get; set; }

        public void Read()
        {
            throw new InvalidOperationException("The ALU cannot read from the BUS.");
        }

        public void Write()
        {
            bool[] a = _regA.Values.ToArray();
            bool[] b = _regB.Values.ToArray();

            var result = Mode switch
            {
                ALUMode.And => And(a, b),
                ALUMode.Or => Or(a, b),
                ALUMode.XOr => XOr(a, b),
                ALUMode.Add => Add(a, b),
                ALUMode.Subtract => Sub(a, b),

                _ => throw new Exception($"Unexpected ALU Mode '{Mode}'."),
            };
        }

        private bool[] And(bool[] a, bool[] b)
        {
            bool[] values = new bool[a.Length];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = a[i] & b[i];
            }

            return values;
        }

        private bool[] Or(bool[] a, bool[] b)
        {
            bool[] values = new bool[a.Length];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = a[i] | b[i];
            }

            return values;
        }

        private bool[] XOr(bool[] a, bool[] b)
        {
            bool[] values = new bool[a.Length];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = a[i] ^ b[i];
            }

            return values;
        }

        private bool[] Add(bool[] a, bool[] b, bool carry = false)
        {
            bool[] values = new bool[a.Length];
            values[a.Length - 1] = carry;

            bool[] x = XOr(a, b);

            for (int i = values.Length - 1; i >= 0; i--)
            {
                // Carry.
                if (i != 0)
                {
                    values[i - 1] = (a[i] & b[i]) | (x[i] & values[i]);
                }

                // Value.
                values[i] ^= x[i];
            }

            return values;
        }

        private bool[] Sub(bool[] a, bool[] b)
        {
            for (int i = 0; i < b.Length; i++)
            {
                b[i] ^= true;
            }

            return Add(a, b, carry: true);
        }
    }
}