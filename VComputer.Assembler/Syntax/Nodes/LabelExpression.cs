using System;
using System.Collections.Generic;
using VComputer.Assembler.Binding;

namespace VComputer.Assembler.Syntax
{
    internal sealed class LabelExpression : ExpressionSyntax
    {
        public LabelExpression(SyntaxToken labelToken)
        {
            LabelToken = labelToken;
        }

        public override SyntaxKind Kind => SyntaxKind.LabelExpression;
        public SyntaxToken LabelToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return LabelToken;
        }
    }
}