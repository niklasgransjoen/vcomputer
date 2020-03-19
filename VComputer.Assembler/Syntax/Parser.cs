using System;
using System.Collections.Generic;
using System.Globalization;
using VComputer.Assembler.Text;

namespace VComputer.Assembler.Syntax
{
    internal sealed class Parser
    {
        private readonly ReadOnlyMemory<SyntaxToken> _tokens;
        private int _position;

        public Parser(SourceText text)
        {
            List<SyntaxToken> tokens = new List<SyntaxToken>();

            Lexer lexer = new Lexer(text);
            SyntaxToken token;
            do
            {
                token = lexer.Lex();

                if (token.Kind != SyntaxKind.BadToken && token.Kind != SyntaxKind.WhitespaceToken)
                    tokens.Add(token);
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
            var commands = ParseCommands();
            var endOfFileToken = MatchToken(SyntaxKind.EndOfFileToken);

            return new CompilationUnitSyntax(commands, endOfFileToken);
        }

        private IEnumerable<CommandStatementSyntax> ParseCommands()
        {
            List<CommandStatementSyntax> _commands = new List<CommandStatementSyntax>();
            while (LookAhead.Kind != SyntaxKind.EndOfFileToken)
            {
                var operatorStatement = ParseOperator();
                var operandExpression = Current.Kind == SyntaxKind.IntegerToken ? ParseOperand() : null;
                var newLineToken = MatchToken(SyntaxKind.NewLineToken);

                var commandSyntax = new CommandStatementSyntax(operatorStatement, operandExpression, newLineToken);
                _commands.Add(commandSyntax);
            }

            return _commands;
        }

        private OperatorStatement ParseOperator()
        {
            var commandToken = MatchToken(SyntaxKind.CommandToken);
            return new OperatorStatement(commandToken, commandToken.Text);
        }

        private OperandExpression ParseOperand()
        {
            var operand = MatchToken(SyntaxKind.IntegerToken);
            if (!int.TryParse(operand.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int value))
                Diagnostics.ReportInvalidOperand(operand.Span, operand.Text);

            return new OperandExpression(operand, value);
        }

        #region Helper methods

        private SyntaxToken Peek(int offset)
        {
            int index = _position + offset;
            if (index >= _tokens.Length)
                return _tokens.Span[_tokens.Length - 1];

            return _tokens.Span[index];
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
            return new SyntaxToken(kind, Current.Position, string.Empty, null);
        }

        #endregion Helper methods
    }
}