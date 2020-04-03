using VComputer.Assembler.Text;

namespace VComputer.Assembler.Syntax
{
    internal sealed class Lexer
    {
        private readonly SourceText _text;

        private int _position;
        private int _start;
        private SyntaxKind _kind;
        private object? _value;

        public Lexer(SourceText text)
        {
            _text = text;
        }

        #region Properties

        public DiagnosticBag Diagnostics { get; } = new DiagnosticBag();

        private char Current => Peek(0);

        private char LookAhead => Peek(1);

        #endregion Properties

        /// <summary>
        /// Lexes the next token in the source text.
        /// </summary>
        public SyntaxToken Lex()
        {
            _start = _position;
            _kind = SyntaxKind.BadToken;
            _value = null;

            if (TryLookupTokenKind(out SyntaxKind kind))
                _kind = kind;
            else
            {
                switch (Current)
                {
                    case '#':
                        ReadLineCommentToken();
                        break;

                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        ReadIntegerToken();
                        break;

                    case 'A':
                    case 'B':
                    case 'C':
                    case 'D':
                    case 'E':
                    case 'F':
                    case 'G':
                    case 'H':
                    case 'I':
                    case 'J':
                    case 'K':
                    case 'L':
                    case 'M':
                    case 'N':
                    case 'O':
                    case 'P':
                    case 'Q':
                    case 'R':
                    case 'S':
                    case 'T':
                    case 'U':
                    case 'V':
                    case 'W':
                    case 'X':
                    case 'Y':
                    case 'Z':
                        ReadCommandToken();
                        break;

                    case ' ':
                    case '\t':
                        ReadWhitespace();
                        break;

                    case '\r':
                    case '\n':
                        ReadNewLineToken();
                        break;

                    default:
                        Diagnostics.ReportBadCharacter(_position, Current);
                        Next();
                        break;
                }
            }

            string? text = SyntaxFacts.GetText(_kind);
            if (text is null)
            {
                int length = _position - _start;
                text = _text.ToString(_start, length);
            }

            return new SyntaxToken(_kind, _start, text, _value);
        }

        #region Lexing

        private bool TryLookupTokenKind(out SyntaxKind syntaxKind)
        {
            switch (Current)
            {
                case '\0':
                    syntaxKind = SyntaxKind.EndOfFileToken;
                    return true;

                default:
                    syntaxKind = default;
                    return false;
            }
        }

        private void ReadLineCommentToken()
        {
            do
            {
                Next();
            }
            while (Current != '\r' && Current != '\n' && Current != '\0');

            _kind = SyntaxKind.LineCommentToken;
        }

        private void ReadWhitespace()
        {
            while (Current == ' ' || Current == '\t')
                Next();

            _kind = SyntaxKind.WhitespaceToken;
        }

        private void ReadNewLineToken()
        {
            while (Current == '\n' || Current == '\r')
                Next();

            _kind = SyntaxKind.NewLineToken;
        }

        private void ReadIntegerToken()
        {
            // Keep reading digits as long as they're available,
            while (char.IsLetterOrDigit(Current))
            {
                Next();
            }

            _kind = SyntaxKind.IntegerToken;
        }

        private void ReadCommandToken()
        {
            while (char.IsUpper(Current))
            {
                Next();
            }

            _kind = SyntaxKind.CommandToken;
        }

        #endregion Lexing

        #region Helpers

        private char Peek(int offset)
        {
            int index = _position + offset;
            if (index >= _text.Length)
                return '\0';

            return _text[index];
        }

        private void Next()
        {
            _position++;
        }

        #endregion Helpers
    }
}