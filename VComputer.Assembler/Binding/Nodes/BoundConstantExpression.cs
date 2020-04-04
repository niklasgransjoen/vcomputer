using System.Collections.Generic;
using System.Linq;

namespace VComputer.Assembler.Binding
{
    internal sealed class BoundConstantExpression : BoundExpression
    {
        public BoundConstantExpression(ConstantSymbol constant)
        {
            Constant = constant;
        }

        public override BoundNodeKind Kind => BoundNodeKind.ConstantExpression;
        public ConstantSymbol Constant { get; }

        public override IEnumerable<BoundNode> GetChildren() => Enumerable.Empty<BoundNode>();
    }
}