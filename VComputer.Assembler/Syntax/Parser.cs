using System;
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
                SyntaxKind.DefKeyword => ParseMacroDefinitionStatement(),
                SyntaxKind.IdentifierToken when LookAhead.Kind == SyntaxKind.ColonToken => ParseLabelDeclarationStatement(),
                SyntaxKind.IdentifierToken when LookAhead.Kind == SyntaxKind.EqualsToken => ParseConstantDeclarationStatement(),
                SyntaxKind.DirectiveToken => ParseDirectiveStatement(),
                _ => ParseCommandOrMacroStatement(),
            };
        }

        private StatementSyntax ParseCommandOrMacroStatement(bool requireNewline = true)
        {
            if (Current.Text.IsEmpty || Current.Text.Span[0] == '_')
                return ParseMacroStatement(requireNewline);
            else
                return ParseCommandStatement(requireNewline);
        }

        private CommandStatement ParseCommandStatement(bool requireNewline = true)
        {
            var commandToken = MatchToken(SyntaxKind.IdentifierToken);
            var operand = CanParseExpression(Current.Kind) ? ParseExpression() : null;
            var newLineToken = requireNewline ? MatchToken(SyntaxKind.NewLineToken) : null;

            return new CommandStatement(commandToken, operand, newLineToken);
        }

        private MacroStatement ParseMacroStatement(bool requireNewline = true)
        {
            var commandToken = MatchToken(SyntaxKind.IdentifierToken);
            var operands = ImmutableArray.CreateBuilder<ExpressionSyntax>();
            while (CanParseExpression(Current.Kind))
            {
                var expression = ParseExpression();
                operands.Add(expression);
            }

            var newLineToken = requireNewline ? MatchToken(SyntaxKind.NewLineToken) : null;

            return new MacroStatement(commandToken, operands.ToImmutable(), newLineToken);
        }

        private DirectiveStatement ParseDirectiveStatement()
        {
            var directiveToken = MatchToken(SyntaxKind.DirectiveToken);
            var operandExpression = Current.Kind != SyntaxKind.NewLineToken ? ParseExpression() : null;
            var newLineToken = MatchToken(SyntaxKind.NewLineToken);

            return new DirectiveStatement(directiveToken, operandExpression, newLineToken);
        }

        #region MacroDefinitionStatement

        private MacroDefinitionStatement ParseMacroDefinitionStatement()
        {
            var defKeyword = MatchToken(SyntaxKind.DefKeyword);
            var identifier = MatchToken(SyntaxKind.IdentifierToken);
            if (identifier.Text.IsEmpty || identifier.Text.Span[0] != '_')
                Diagnostics.ReportBadMacroIdentifer(identifier.Span, identifier.Text.ToString());

            var parameters = ParseMacroParameters();
            var statements = ParseMacroDefinitionStatementBody();

            return new MacroDefinitionStatement(defKeyword, identifier, parameters, statements);
        }

        private ImmutableArray<SyntaxToken> ParseMacroParameters()
        {
            var builder = ImmutableArray.CreateBuilder<SyntaxToken>();
            while (Current.Kind == SyntaxKind.IdentifierToken)
            {
                builder.Add(Current);
                NextToken();
            }

            return builder.ToImmutable();
        }

        private SeparatedStatementCollection<StatementSyntax> ParseMacroDefinitionStatementBody()
        {
            // If the macro declaration is empty, escape early.
            if (Current.Kind != SyntaxKind.SlashSlashToken)
            {
                return SeparatedStatementCollection<StatementSyntax>.Empty;
            }

            var builder = ImmutableArray.CreateBuilder<SyntaxNode>();
            while (true)
            {
                var slashSlashToken = MatchToken(SyntaxKind.SlashSlashToken);
                var newlineToken = MatchToken(SyntaxKind.NewLineToken);

                builder.Add(slashSlashToken);
                builder.Add(newlineToken);

                if (Current.Kind != SyntaxKind.IdentifierToken)
                    return new SeparatedStatementCollection<StatementSyntax>(builder.ToImmutable(), SeparatorOptions.HasLeadingAndTrailingSeparator, separatorCount: 2);

                var statement = ParseCommandOrMacroStatement(requireNewline: false);
                builder.Add(statement);

                if (Current.Kind != SyntaxKind.SlashSlashToken)
                    return new SeparatedStatementCollection<StatementSyntax>(builder.ToImmutable(), SeparatorOptions.HasLeadingSeparator, separatorCount: 2);
            }
        }

        #endregion MacroDefinitionStatement

        private LabelDeclarationStatement ParseLabelDeclarationStatement()
        {
            var labelToken = MatchToken(SyntaxKind.IdentifierToken);
            if (labelToken.Text.IsEmpty || !char.IsLower(labelToken.Text.Span[0]))
                Diagnostics.ReportBadLabelIdentifier(labelToken.Span, labelToken.Text.ToString());

            var colonToken = MatchToken(SyntaxKind.ColonToken);

            return new LabelDeclarationStatement(labelToken, colonToken);
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

        private bool CanParseExpression(SyntaxKind kind) => kind switch
        {
            SyntaxKind.IntegerToken => true,
            SyntaxKind.IdentifierToken => true,
            _ => false,
        };

        private ExpressionSyntax ParseExpression()
        {
            return Current.Kind switch
            {
                SyntaxKind.IntegerToken => ParseLiteralExpression(),
                _ => ParseNameExpression(),
            };
        }

        private NameExpression ParseNameExpression()
        {
            var identifier = MatchToken(SyntaxKind.IdentifierToken);
            return new NameExpression(identifier);
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

            var defaultText = kind.GetDefaultTokenText().AsMemory();
            var defaultValue = kind.GetDefaultTokenValue();
            return new SyntaxToken(kind, Current.Position, defaultText, defaultValue);
        }

        #endregion Helper methods
    }
}