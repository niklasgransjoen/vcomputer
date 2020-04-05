using System.Collections.Generic;
using System.Diagnostics;

namespace VComputer.Assembler.Syntax
{
    [DebuggerDisplay("{CommandToken} {Operand}")]
    internal sealed class CommandStatement : StatementSyntax
    {
        public CommandStatement(SyntaxToken commandToken, ExpressionSyntax? operand, SyntaxToken? newLineToken)
        {
            CommandToken = commandToken;
            Operand = operand;
            NewLineToken = newLineToken;
        }

        public override SyntaxKind Kind => SyntaxKind.CommandStatement;

        public SyntaxToken CommandToken { get; }
        public ExpressionSyntax? Operand { get; }
        public SyntaxToken? NewLineToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return CommandToken;

            if (Operand != null)
            {
                yield return Operand;
            }

            if (NewLineToken != null)
            {
                yield return NewLineToken;
            }
        }
    }
}