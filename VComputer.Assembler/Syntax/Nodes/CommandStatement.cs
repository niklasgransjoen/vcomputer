using System.Collections.Generic;
using System.Diagnostics;

namespace VComputer.Assembler.Syntax
{
    [DebuggerDisplay("{OperatorStatement} {OperandExpression}")]
    internal sealed class CommandStatement : StatementSyntax
    {
        public CommandStatement(SyntaxToken commandToken, ExpressionSyntax? operandExpression, SyntaxToken newLineToken)
        {
            CommandToken = commandToken;
            OperandExpression = operandExpression;
            NewLineToken = newLineToken;
        }

        public override SyntaxKind Kind => SyntaxKind.CommandStatement;

        public SyntaxToken CommandToken { get; }
        public ExpressionSyntax? OperandExpression { get; }
        public SyntaxToken NewLineToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return CommandToken;

            if (OperandExpression != null)
            {
                yield return OperandExpression;
            }

            yield return NewLineToken;
        }
    }
}