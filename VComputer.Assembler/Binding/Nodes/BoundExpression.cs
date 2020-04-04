using System.Collections.Generic;
using System.Linq;

namespace VComputer.Assembler.Binding
{
    internal abstract class BoundExpression : BoundNode
    {
    }

    internal sealed class BoundLiteralExpression : BoundExpression
    {
        public BoundLiteralExpression(int value)
        {
            Value = value;
        }

        public override BoundNodeKind Kind => BoundNodeKind.LiteralExpression;
        public int Value { get; }

        public override IEnumerable<BoundNode> GetChildren() => Enumerable.Empty<BoundNode>();
    }
}