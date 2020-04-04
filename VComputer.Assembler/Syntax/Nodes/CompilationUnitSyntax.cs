using System.Collections.Generic;
using System.Collections.Immutable;

namespace VComputer.Assembler.Syntax
{
    internal sealed class CompilationUnitSyntax : SyntaxNode
    {
        public CompilationUnitSyntax(ImmutableArray<StatementSyntax> statements, SyntaxToken endOfFileToken)
        {
            Statements = statements;
            EndOfFileToken = endOfFileToken;
        }

        public ImmutableArray<StatementSyntax> Statements { get; }
        public SyntaxToken EndOfFileToken { get; }

        public override SyntaxKind Kind => SyntaxKind.CompilationUnit;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            for (int i = 0; i < Statements.Length; i++)
            {
                yield return Statements[i];
            }

            yield return EndOfFileToken;
        }
    }
}