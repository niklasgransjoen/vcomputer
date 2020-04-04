using System;
using System.Diagnostics;

namespace VComputer.Assembler.Text
{
    /// <summary>
    /// Describes the position and length of a section of text.
    /// </summary>
    [DebuggerDisplay("{Start}..{End}")]
    public readonly struct TextSpan : IEquatable<TextSpan>
    {
        #region Constructors

        public TextSpan(int start, int length)
        {
            Start = start;
            Length = length;
        }

        /// <summary>
        /// Creates a text span from the given start and end values.
        /// </summary>
        public static TextSpan FromBounds(int start, int end)
        {
            int length = end - start;
            return new TextSpan(start, length);
        }

        #endregion Constructors

        #region Properties

        public int Start { get; }
        public int Length { get; }
        public int End => Start + Length;

        #endregion Properties

        public override string ToString()
        {
            return $"{Start}..{End}";
        }

        #region Operators

        public override bool Equals(object? obj)
        {
            return obj is TextSpan span && Equals(span);
        }

        public bool Equals(TextSpan other)
        {
            return Start == other.Start &&
                   Length == other.Length;
        }

        public override int GetHashCode() => HashCode.Combine(Start, Length);

        public static bool operator ==(TextSpan left, TextSpan right) => left.Equals(right);

        public static bool operator !=(TextSpan left, TextSpan right) => !(left == right);

        #endregion Operators
    }
}