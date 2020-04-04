using System.Collections.Generic;

namespace VComputer.Assembler.Syntax
{
    internal sealed class LabelStatement : StatementSyntax
    {
        public LabelStatement(SyntaxToken labelToken)
        {
            LabelToken = labelToken;
        }

        public override SyntaxKind Kind => SyntaxKind.LabelStatement;

        public SyntaxToken LabelToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return LabelToken;
        }
    }
}