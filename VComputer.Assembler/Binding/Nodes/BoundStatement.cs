using System.Collections.Generic;
using System.Linq;

namespace VComputer.Assembler.Binding
{
    internal abstract class BoundStatement : BoundNode
    {
    }

    internal sealed class BoundErrorExpression : BoundExpression
    {
        public static BoundErrorExpression Instance { get; } = new BoundErrorExpression();

        private BoundErrorExpression()
        {
        }

        public override BoundNodeKind Kind => BoundNodeKind.ErrorExpression;

        public override IEnumerable<BoundNode> GetChildren() => Enumerable.Empty<BoundNode>();
    }
}