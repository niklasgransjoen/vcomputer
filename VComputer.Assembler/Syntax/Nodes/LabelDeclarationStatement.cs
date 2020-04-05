using System.Collections.Generic;

namespace VComputer.Assembler.Syntax
{
    internal sealed class LabelDeclarationStatement : StatementSyntax
    {
        public LabelDeclarationStatement(SyntaxToken identifier, SyntaxToken colonToken)
        {
            Identifier = identifier;
            ColonToken = colonToken;
        }

        public override SyntaxKind Kind => SyntaxKind.LabelDeclarationStatement;

        public SyntaxToken Identifier { get; }
        public SyntaxToken ColonToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Identifier;
            yield return ColonToken;
        }
    }
}