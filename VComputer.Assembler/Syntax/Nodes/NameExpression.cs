using System.Collections.Generic;

namespace VComputer.Assembler.Syntax
{
    internal sealed class NameExpression : ExpressionSyntax
    {
        public NameExpression(SyntaxToken identifier)
        {
            Identifier = identifier;
        }

        public override SyntaxKind Kind => SyntaxKind.NameExpression;
        public SyntaxToken Identifier { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Identifier;
        }
    }
}