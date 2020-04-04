using System.Collections.Generic;
using System.Diagnostics;

namespace VComputer.Assembler.Syntax
{
    [DebuggerDisplay("{OperatorStatement} {OperandExpression}")]
    internal sealed class CommandStatement : StatementSyntax
    {
        public CommandStatement(OperatorStatement operatorStatement, ExpressionSyntax? operandExpression, SyntaxToken newLineToken)
        {
            OperatorStatement = operatorStatement;
            OperandExpression = operandExpression;
            NewLineToken = newLineToken;
        }

        public override SyntaxKind Kind => SyntaxKind.CommandStatement;

        public OperatorStatement OperatorStatement { get; }
        public ExpressionSyntax? OperandExpression { get; }
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
}