namespace VComputer.Components
{
    /// <summary>
    /// The computer bus.
    /// </summary>
    public sealed class Bus
    {
        public Bus(int bits)
        {
            Bits = bits;
            Lines = new bool[bits];
        }

        public int Bits { get; }
        public bool[] Lines { get; }
    }
}