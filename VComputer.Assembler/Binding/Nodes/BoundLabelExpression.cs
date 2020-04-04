using System.Collections.Generic;
using System.Linq;

namespace VComputer.Assembler.Binding
{
    internal sealed class BoundLabelExpression : BoundExpression
    {
        public BoundLabelExpression(LabelSymbol label)
        {
            Label = label;
        }

        public override BoundNodeKind Kind => BoundNodeKind.LabelExpression;
        public LabelSymbol Label { get; }

        public override IEnumerable<BoundNode> GetChildren() => Enumerable.Empty<BoundNode>();
    }
}