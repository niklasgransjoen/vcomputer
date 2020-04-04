using System.Collections.Generic;

namespace VComputer.Assembler.Syntax
{
    internal sealed class LabelDeclarationStatement : StatementSyntax
    {
        public LabelDeclarationStatement(SyntaxToken labelToken)
        {
            LabelToken = labelToken;
        }

        public override SyntaxKind Kind => SyntaxKind.LabelDeclarationStatement;

        public SyntaxToken LabelToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return LabelToken;
        }
    }
}