using System.Collections.Generic;
using System.Linq;

namespace VComputer.Assembler.Binding
{
    internal sealed class BoundLabelExpression : BoundExpression
    {
        public BoundLabelExpression(LabelSymbol labelSymbol)
        {
            LabelSymbol = labelSymbol;
        }

        public override BoundNodeKind Kind => BoundNodeKind.LabelExpression;
        public LabelSymbol LabelSymbol { get; }

        public override IEnumerable<BoundNode> GetChildren() => Enumerable.Empty<BoundNode>();
    }
}