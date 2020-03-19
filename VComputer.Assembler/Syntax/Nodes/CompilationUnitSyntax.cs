using System;
using System.Collections.Generic;
using System.Linq;

namespace VComputer.Assembler.Syntax
{
    internal sealed class CompilationUnitSyntax : SyntaxNode
    {
        public CompilationUnitSyntax(IEnumerable<CommandStatementSyntax> commands, SyntaxToken endOfFileToken)
        {
            Commands = commands.ToArray();
            EndOfFileToken = endOfFileToken;
        }

        public ReadOnlyMemory<CommandStatementSyntax> Commands { get; }
        public SyntaxToken EndOfFileToken { get; }

        public override SyntaxKind Kind => SyntaxKind.CompilationUnit;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            for (int i = 0; i < Commands.Length; i++)
            {
                yield return Commands.Span[i];
            }

            yield return EndOfFileToken;
        }
    }
}