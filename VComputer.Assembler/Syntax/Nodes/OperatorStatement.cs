using System.Collections.Generic;
using System.Diagnostics;

namespace VComputer.Assembler.Syntax
{
    [DebuggerDisplay("{Command}")]
    internal sealed class OperatorStatement : StatementSyntax
    {
        public OperatorStatement(SyntaxToken commandToken)
        {
            CommandToken = commandToken;
        }

        public override SyntaxKind Kind => SyntaxKind.OperatorStatement;
        public SyntaxToken CommandToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return CommandToken;
        }
    }
}