namespace VComputer.Components
{
    /// <summary>
    /// The computer bus.
    /// </summary>
    public sealed class Bus
    {
        public Bus(int bitCount)
        {
            BitCount = bitCount;
            Lines = new bool[bitCount];
        }

        public int BitCount { get; }
        public bool[] Lines { get; }
    }
}