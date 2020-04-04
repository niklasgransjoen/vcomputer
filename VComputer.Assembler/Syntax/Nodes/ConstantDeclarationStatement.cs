using System.Collections.Generic;

namespace VComputer.Assembler.Syntax
{
    internal sealed class ConstantDeclarationStatement : StatementSyntax
    {
        public ConstantDeclarationStatement(SyntaxToken identifier, SyntaxToken equalsToken, ExpressionSyntax expression)
        {
            Identifier = identifier;
            EqualsToken = equalsToken;
            Expression = expression;
        }

        public override SyntaxKind Kind => SyntaxKind.ConstantDeclarationStatement;

        public SyntaxToken Identifier { get; }
        public SyntaxToken EqualsToken { get; }
        public ExpressionSyntax Expression { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Identifier;
            yield return EqualsToken;
            yield return Expression;
        }
    }
}