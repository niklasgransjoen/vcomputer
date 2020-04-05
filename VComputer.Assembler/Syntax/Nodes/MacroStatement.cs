using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace VComputer.Assembler.Syntax
{
    [DebuggerDisplay("{MacroToken} {Operands}")]
    internal sealed class MacroStatement : StatementSyntax
    {
        public MacroStatement(SyntaxToken macroToken, ImmutableArray<ExpressionSyntax> operands, SyntaxToken? newLineToken)
        {
            MacroToken = macroToken;
            Operands = operands;
            NewLineToken = newLineToken;
        }

        public override SyntaxKind Kind => SyntaxKind.MacroStatement;

        public SyntaxToken MacroToken { get; }
        public ImmutableArray<ExpressionSyntax> Operands { get; }
        public SyntaxToken? NewLineToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return MacroToken;

            foreach (var expression in Operands)
            {
                yield return expression;
            }

            if (NewLineToken != null)
            {
                yield return NewLineToken;
            }
        }
    }
}