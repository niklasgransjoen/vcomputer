using System.Collections.Generic;
using System.Diagnostics;
using VComputer.Assembler.Symbols;

namespace VComputer.Assembler.Binding
{
    [DebuggerDisplay("{Directive} {Operand}")]
    internal sealed class BoundDirectiveStatement : BoundStatement
    {
        public BoundDirectiveStatement(DirectiveSymbol directive, BoundExpression? operand)
        {
            Directive = directive;
            Operand = operand;
        }

        public override BoundNodeKind Kind => BoundNodeKind.DirectiveStatement;

        public DirectiveSymbol Directive { get; }
        public BoundExpression? Operand { get; }

        public override IEnumerable<BoundNode> GetChildren()
        {
            if (Operand != null)
                yield return Operand;
        }
    }
}