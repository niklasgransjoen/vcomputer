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
            _value = null;

            if (!TryLookupTokenKind(out _kind))
            {
                switch (Current)
                {
                    case '#':
                        ReadLineComment();
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
                        ReadInteger();
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
                        ReadIdentifier();
                        break;

                    case 'a':
                    case 'b':
                    case 'c':
                    case 'd':
                    case 'e':
                    case 'f':
                    case 'g':
                    case 'h':
                    case 'i':
                    case 'j':
                    case 'k':
                    case 'l':
                    case 'm':
                    case 'n':
                    case 'o':
                    case 'p':
                    case 'q':
                    case 'r':
                    case 's':
                    case 't':
                    case 'u':
                    case 'v':
                    case 'w':
                    case 'x':
                    case 'y':
                    case 'z':
                        ReadLabel();
                        break;

                    case ' ':
                    case '\t':
                        ReadWhitespace();
                        break;

                    case '\r':
                    case '\n':
                        ReadNewLine();
                        break;

                    default:
                        if (char.IsWhiteSpace(Current))
                            ReadWhitespace();
                        else
                        {
                            Diagnostics.ReportBadCharacter(_position, Current);
                            Next();
                        }
                        break;
                }
            }

            if (!SyntaxFacts.TryGetText(_kind, out var text))
            {
                int length = _position - _start;
                text = _text.SubString(_start, length);
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

                case '=':
                    Next();
                    syntaxKind = SyntaxKind.EqualsToken;
                    return true;

                default:
                    syntaxKind = SyntaxKind.BadToken;
                    return false;
            }
        }

        private void ReadLineComment()
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

        private void ReadNewLine()
        {
            while (Current == '\n' || Current == '\r')
                Next();

            _kind = SyntaxKind.NewLineToken;
        }

        private void ReadInteger()
        {
            // Keep reading digits as long as they're available,
            while (char.IsLetterOrDigit(Current))
            {
                Next();
            }

            _kind = SyntaxKind.IntegerToken;
            _value = _text.SubString(_start, _position - _start);
        }

        private void ReadIdentifier()
        {
            do
            {
                Next();
            }
            while (char.IsUpper(Current) || Current == '_');

            _kind = SyntaxKind.IdentifierToken;
        }

        private void ReadLabel()
        {
            do
            {
                Next();
            }
            while (char.IsLetter(Current));
            _value = _text.SubString(_start, _position - _start);

            if (Match(':'))
            {
                _kind = SyntaxKind.LabelDeclarationToken;
            }
            else
            {
                _kind = SyntaxKind.LabelToken;
            }
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

        private bool Match(char expected)
        {
            if (Current == expected)
            {
                Next();
                return true;
            }

            return false;
        }

        #endregion Helpers
    }
}