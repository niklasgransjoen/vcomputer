using System.Collections.Generic;
using System.Diagnostics;

namespace VComputer.Assembler.Syntax
{
    [DebuggerDisplay("{OperatorStatement} {OperandExpression}")]
    internal sealed class CommandStatementSyntax : SyntaxNode
    {
        public CommandStatementSyntax(OperatorStatement operatorStatement, OperandExpression? operandExpression, SyntaxToken newLineToken)
        {
            OperatorStatement = operatorStatement;
            OperandExpression = operandExpression;
            NewLineToken = newLineToken;
        }

        public override SyntaxKind Kind => SyntaxKind.CommandToken;

        public OperatorStatement OperatorStatement { get; }
        public OperandExpression? OperandExpression { get; }
        public SyntaxToken NewLineToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OperatorStatement;

            if (OperandExpression != null)
            {
                yield return OperandExpression;
            }

            yield return NewLineToken;
        }
    }

    [DebuggerDisplay("{Command}")]
    internal sealed class OperatorStatement : StatementSyntax
    {
        public OperatorStatement(SyntaxToken commandToken, string command)
        {
            CommandToken = commandToken;
            Command = command;
        }

        public override SyntaxKind Kind => SyntaxKind.OperatorStatement;

        public SyntaxToken CommandToken { get; }
        public string Command { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return CommandToken;
        }
    }

    [DebuggerDisplay("{Value}")]
    internal sealed class OperandExpression : ExpressionSyntax
    {
        public OperandExpression(SyntaxToken operandToken, int value)
        {
            OperandToken = operandToken;
            Value = value;
        }

        public override SyntaxKind Kind => SyntaxKind.OperandExpression;

        public SyntaxToken OperandToken { get; }
        public int Value { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OperandToken;
        }
    }
}