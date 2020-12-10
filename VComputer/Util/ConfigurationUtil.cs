namespace VComputer.Util
{
    internal static class ConfigurationUtil
    {
        public static void AssertBitCount(int expectedBits, int actualBits)
        {
            if (expectedBits != actualBits)
            {
                throw new ConfigurationException("Connected bus has wrong bit count.");
            }
        }
    }
}