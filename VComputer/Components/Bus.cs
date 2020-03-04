namespace VComputer.Components
{
    public sealed class Bus
    {
        public Bus(int bits)
        {
            Lines = new bool[bits];
        }

        public bool[] Lines { get; }
    }
}