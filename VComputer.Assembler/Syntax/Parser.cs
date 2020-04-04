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

                StatementSyntax statement = Current.Kind switch
                {
                    SyntaxKind.LabelDeclarationToken => ParseLabelStatement(),
                    _ => ParseCommandStatement(),
                };
                statements.Add(statement);
            }

            return statements.ToImmutable();
        }

        private CommandStatement ParseCommandStatement()
        {
            var operatorStatement = ParseOperatorStatement();
            var operandExpression = Current.Kind != SyntaxKind.NewLineToken ? ParseExpression() : null;
            var newLineToken = MatchToken(SyntaxKind.NewLineToken);

            return new CommandStatement(operatorStatement, operandExpression, newLineToken);
        }

        private OperatorStatement ParseOperatorStatement()
        {
            var commandToken = MatchToken(SyntaxKind.CommandToken);
            return new OperatorStatement(commandToken);
        }

        private LabelStatement ParseLabelStatement()
        {
            var labelToken = NextToken();
            return new LabelStatement(labelToken);
        }

        #endregion Statement

        #region Expression

        private ExpressionSyntax ParseExpression()
        {
            if (Current.Kind == SyntaxKind.LabelToken)
                return ParseLabelExpression();
            else
                return ParseLiteralExpression();
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
                Diagnostics.ReportInvalidOperand(operand.Span, operand.Text.ToString());

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
            return new SyntaxToken(kind, Current.Position, default, null);
        }

        #endregion Helper methods
    }
}