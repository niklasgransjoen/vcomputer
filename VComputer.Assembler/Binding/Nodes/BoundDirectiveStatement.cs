using System.Collections.Generic;

namespace VComputer.Assembler.Binding
{
    internal sealed class BoundDirectiveStatement : BoundStatement
    {
        public BoundDirectiveStatement(Directive directive, BoundExpression? operand)
        {
            Directive = directive;
            Operand = operand;
        }

        public override BoundNodeKind Kind => BoundNodeKind.DirectiveStatement;

        public Directive Directive { get; }
        public BoundExpression? Operand { get; }

        public override IEnumerable<BoundNode> GetChildren()
        {
            if (Operand != null)
                yield return Operand;
        }
    }
}