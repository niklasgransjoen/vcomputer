using System;
using System.Collections.Generic;

namespace VComputer.Util
{
    /// <summary>
    /// Collection of primitive math utilities.
    /// </summary>
    internal static class MathUtil
    {
        private static readonly IDictionary<int, int> _powerTable = new Dictionary<int, int>();

        /// <summary>
        /// Returns two to the given power.
        /// </summary>
        public static int TwoPow(int value)
        {
            if (!_powerTable.TryGetValue(value, out int result))
            {
                result = (int)Math.Pow(2, value);
                _powerTable.Add(value, result);
            }

            return result;
        }
    }
}