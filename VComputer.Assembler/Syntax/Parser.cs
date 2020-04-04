using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using VComputer.Assembler.Text;

namespace VComputer.Assembler.Syntax
{
    internal sealed class Parser
    {
        private static readonly HashSet<SyntaxKind> _skippableTokens = new HashSet<SyntaxKind>
        {
            SyntaxKind.BadToken,
            SyntaxKind.WhitespaceToken,
            SyntaxKind.LineCommentToken,
        };

        private readonly SyntaxToken[] _tokens;
        private int _position;

        public Parser(SourceText text)
        {
            List<SyntaxToken> tokens = new List<SyntaxToken>();

            Lexer lexer = new Lexer(text);
            SyntaxToken token;
            do
            {
                token = lexer.Lex();

                if (!_skippableTokens.Contains(token.Kind))
                {
                    tokens.Add(token);
                }
            } while (token.Kind != SyntaxKind.EndOfFileToken);

            _tokens = tokens.ToArray();
            Diagnostics.AddRange(lexer.Diagnostics);
        }

        #region Properties

        public DiagnosticBag Diagnostics { get; } = new DiagnosticBag();

        private SyntaxToken Current => Peek(0);
        private SyntaxToken LookAhead => Peek(1);

        #endregion Properties

        public CompilationUnitSyntax ParseCompilationUnit()
        {
            var statements = ParseStatements();
            var endOfFileToken = MatchToken(SyntaxKind.EndOfFileToken);

            return new CompilationUnitSyntax(statements, endOfFileToken);
        }

        #region Statement

        private ImmutableArray<StatementSyntax> ParseStatements()
        {
            var statements = ImmutableArray.CreateBuilder<StatementSyntax>();
            while (Current.Kind != SyntaxKind.EndOfFileToken)
            {
                if (Current.Kind == SyntaxKind.NewLineToken)
                {
                    NextToken();
                    continue;
                }

                var currentToken = Current;

                StatementSyntax statement = ParseStatement();
                statements.Add(statement);

                // If no tokens were consumed by the parse call,
                // we skip past the next newline. Parse errors will
                // have already been reported.
                if (currentToken == Current)
                {
                    while (Current.Kind != SyntaxKind.EndOfFileToken && Current.Kind != SyntaxKind.NewLineToken)
                    {
                        NextToken();
                    }
                }
            }

            return statements.ToImmutable();
        }

        private StatementSyntax ParseStatement()
        {
            return Current.Kind switch
            {
                SyntaxKind.LabelDeclarationToken => ParseLabelStatement(),
                SyntaxKind.IdentifierToken when LookAhead.Kind == SyntaxKind.EqualsToken => ParseConstantDeclarationStatement(),
                SyntaxKind.DirectiveToken => ParseDirectiveStatement(),
                _ => ParseCommandStatement(),
            };
        }

        private CommandStatement ParseCommandStatement()
        {
            var commandToken = MatchToken(SyntaxKind.IdentifierToken);
            var operandExpression = Current.Kind != SyntaxKind.NewLineToken ? ParseExpression() : null;
            var newLineToken = MatchToken(SyntaxKind.NewLineToken);

            return new CommandStatement(commandToken, operandExpression, newLineToken);
        }

        private DirectiveStatement ParseDirectiveStatement()
        {
            var directiveToken = MatchToken(SyntaxKind.DirectiveToken);
            var operandExpression = Current.Kind != SyntaxKind.NewLineToken ? ParseExpression() : null;
            var newLineToken = MatchToken(SyntaxKind.NewLineToken);

            return new DirectiveStatement(directiveToken, operandExpression, newLineToken);
        }

        private LabelDeclarationStatement ParseLabelStatement()
        {
            var labelToken = NextToken();
            return new LabelDeclarationStatement(labelToken);
        }

        private ConstantDeclarationStatement ParseConstantDeclarationStatement()
        {
            var identifer = MatchToken(SyntaxKind.IdentifierToken);
            var equalsToken = MatchToken(SyntaxKind.EqualsToken);
            var expression = ParseExpression();

            return new ConstantDeclarationStatement(identifer, equalsToken, expression);
        }

        #endregion Statement

        #region Expression

        private ExpressionSyntax ParseExpression()
        {
            return Current.Kind switch
            {
                SyntaxKind.IdentifierToken => ParseNameExpression(),
                SyntaxKind.LabelToken => ParseLabelExpression(),

                _ => ParseLiteralExpression(),
            };
        }

        private ConstantExpression ParseNameExpression()
        {
            var identifier = MatchToken(SyntaxKind.IdentifierToken);
            return new ConstantExpression(identifier);
        }

        private LabelExpression ParseLabelExpression()
        {
            var label = MatchToken(SyntaxKind.LabelToken);
            return new LabelExpression(label);
        }

        private LiteralExpression ParseLiteralExpression()
        {
            var operand = MatchToken(SyntaxKind.IntegerToken);
            if (!int.TryParse(operand.Text.Span, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int value))
                Diagnostics.ReportInvalidInteger(operand.Span, operand.Text.ToString());

            return new LiteralExpression(operand, value);
        }

        #endregion Expression

        #region Helper methods

        private SyntaxToken Peek(int offset)
        {
            int index = _position + offset;
            if (index >= _tokens.Length)
                return _tokens[^1];

            return _tokens[index];
        }

        private SyntaxToken NextToken()
        {
            SyntaxToken current = Current;
            _position++;

            return current;
        }

        private SyntaxToken MatchToken(SyntaxKind kind)
        {
            if (Current.Kind == kind)
                return NextToken();

            Diagnostics.ReportUnexpectedToken(Current.Span, Current.Kind, kind);

            var defaultText = SyntaxFacts.GetDefaultTokenText(kind);
            var defaultValue = SyntaxFacts.GetDefaultTokenValue(kind);
            return new SyntaxToken(kind, Current.Position, defaultText, defaultValue);
        }

        #endregion Helper methods
    }
}