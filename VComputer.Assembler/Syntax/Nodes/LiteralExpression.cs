using System;
using System.Collections.Generic;
using System.Diagnostics;
using VComputer.Assembler.Binding;

namespace VComputer.Assembler.Syntax
{
    [DebuggerDisplay("{Value}")]
    internal sealed class LiteralExpression : ExpressionSyntax
    {
        public LiteralExpression(SyntaxToken operandToken, int value)
        {
            OperandToken = operandToken;
            Value = value;
        }

        public override SyntaxKind Kind => SyntaxKind.LiteralExpression;

        public SyntaxToken OperandToken { get; }
        public int Value { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OperandToken;
        }
    }
}