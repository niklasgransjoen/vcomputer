using System;
using System.Collections.Generic;

namespace VComputer.Assembler.Text
{
    public sealed class SourceText
    {
        private readonly string _text;

        private SourceText(string text)
        {
            _text = text;
            Lines = ParseLines(this, text);
        }

        public static SourceText From(string text)
        {
            return new SourceText(text);
        }

        #region Properties

        public ReadOnlyMemory<TextLine> Lines { get; }

        public char this[int index] => _text[index];

        public int Length => _text.Length;

        #endregion Properties

        #region Methods

        public int GetLineIndex(int position)
        {
            int lower = 0;
            int upper = Lines.Length - 1;

            while (lower <= upper)
            {
                int index = lower + (upper - lower) / 2;
                int start = Lines.Span[index].Start;

                if (position == start)
                    return index;

                if (position < start)
                    upper = index - 1;
                else
                    lower = index + 1;
            }

            return lower - 1;
        }

        public override string ToString()
        {
            return _text;
        }

        public string ToString(int start, int length) => _text.Substring(start, length);

        public string ToString(TextSpan span)
        {
            int length = span.Length;
            if (length <= 0)
                return string.Empty;

            return _text.Substring(span.Start, length);
        }

        #endregion Methods

        #region Static methods

        private static ReadOnlyMemory<TextLine> ParseLines(SourceText sourceText, string text)
        {
            List<TextLine> result = new List<TextLine>();

            int position = 0;
            int lineStart = 0;

            while (position < text.Length)
            {
                int lineBreakWidth = GetLineBreakWidth(text, position);
                if (lineBreakWidth == 0)
                {
                    position++;
                }
                else
                {
                    AddLine(result, sourceText, position, lineStart, lineBreakWidth);

                    position += lineBreakWidth;
                    lineStart = position;
                }
            }

            if (position >= lineStart)
                AddLine(result, sourceText, position, lineStart, 0);

            return result.ToArray();
        }

        private static void AddLine(List<TextLine> result, SourceText sourceText, int position, int lineStart, int lineBreakWidth)
        {
            int lineLength = position - lineStart;
            int lineLengthIncludingLineBreak = lineLength + lineBreakWidth;
            TextLine line = new TextLine(sourceText, lineStart, lineLength, lineLengthIncludingLineBreak);

            result.Add(line);
        }

        private static int GetLineBreakWidth(string text, int position)
        {
            char currentChar = text[position];
            char nextChar = position + 1 >= text.Length ? '\0' : text[position + 1];

            if (currentChar == '\r' && nextChar == '\n')
                return 2;

            if (currentChar == '\r' || currentChar == '\n')
                return 1;

            return 0;
        }

        #endregion Static methods
    }
}