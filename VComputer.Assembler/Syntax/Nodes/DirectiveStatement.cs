using System.Collections.Generic;

namespace VComputer.Assembler.Syntax
{
    internal sealed class DirectiveStatement : StatementSyntax
    {
        public DirectiveStatement(SyntaxToken directiveToken, ExpressionSyntax? operandExpression, SyntaxToken newLineToken)
        {
            DirectiveToken = directiveToken;
            OperandExpression = operandExpression;
            NewLineToken = newLineToken;
        }

        public override SyntaxKind Kind => SyntaxKind.DirectiveStatement;

        public SyntaxToken DirectiveToken { get; }
        public ExpressionSyntax? OperandExpression { get; }
        public SyntaxToken NewLineToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return DirectiveToken;
            if (OperandExpression != null)
                yield return OperandExpression;

            yield return NewLineToken;
        }
    }
}