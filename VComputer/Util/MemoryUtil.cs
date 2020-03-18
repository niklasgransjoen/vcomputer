using System;

namespace VComputer.Util
{
    /// <summary>
    /// Collection of utilities for working with VComputer's memory.
    /// </summary>
    public static class MemoryUtil
    {
        /// <summary>
        /// Converts the given memory segment to an integer.
        /// </summary>
        public static int ToInt(in ReadOnlySpan<bool> memory)
        {
            int result = 0;

            int maxVal = memory.Length - 1;
            for (int i = maxVal; i >= 0; i--)
            {
                if (memory[i])
                {
                    result += MathUtil.TwoPow(maxVal - i);
                }
            }

            return result;
        }

        /// <summary>
        /// Converts an integer into a memory segment.
        /// </summary>
        public static void FromInt(int value, in Span<bool> memory)
        {
            for (int i = 0; i < memory.Length; i++)
            {
                memory[i] = value % 2 == 1;
                value /= 2;
            }
        }
    }
}