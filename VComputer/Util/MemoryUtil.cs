using System;
using System.Linq;

namespace VComputer.Util
{
    /// <summary>
    /// Collection of utilities for working with VComputer's memory.
    /// </summary>
    public static class MemoryUtil
    {
        #region ToInt

        /// <summary>
        /// Converts the given memory segment to an integer.
        /// </summary>
        public static int ToInt(bool[] memory)
        {
            if (memory is null)
                throw new ArgumentNullException(nameof(memory));

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
        /// Converts the given memory segment to an integer.
        /// </summary>
        public static int ToInt(in ReadOnlyMemory<bool> memory) => ToInt(memory.Span);

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

        #endregion ToInt

        /// <summary>
        /// Converts an integer into a memory segment.
        /// </summary>
        public static void FromInt(int value, in Span<bool> memory)
        {
            for (int i = memory.Length - 1; i >= 0; i--)
            {
                memory[i] = value % 2 == 1;
                value /= 2;
            }
        }

        /// <summary>
        /// Converts the given memory segment to a string.
        /// </summary>
        public static string ToString(bool[] memory)
        {
            if (memory is null)
                throw new ArgumentNullException(nameof(memory));

            if (memory.Length == 0)
                return string.Empty;

            var expression = memory.Select(b => b ? "1" : "0");
            return string.Join("", expression);
        }

        /// <summary>
        /// Converts the given memory segment to a string.
        /// </summary>
        public static string ToString(in ReadOnlyMemory<bool> memory) => ToString(memory.Span);

        /// <summary>
        /// Converts the given memory segment to a string.
        /// </summary>
        public static string ToString(in ReadOnlySpan<bool> memory)
        {
            if (memory.Length == 0)
                return string.Empty;

            Span<char> span = stackalloc char[memory.Length];
            for (int i = 0; i < memory.Length; i++)
            {
                span[i] = memory[i] ? '1' : '0';
            }

            return span.ToString();
        }
    }
}