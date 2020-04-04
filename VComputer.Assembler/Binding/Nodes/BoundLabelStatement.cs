using System.Collections.Generic;
using System.Linq;

namespace VComputer.Assembler.Binding
{
    internal sealed class BoundLabelStatement : BoundStatement
    {
        public BoundLabelStatement(LabelSymbol labelSymbol)
        {
            LabelSymbol = labelSymbol;
        }

        public override BoundNodeKind Kind => BoundNodeKind.LabelStatement;
        public LabelSymbol LabelSymbol { get; }

        public override IEnumerable<BoundNode> GetChildren() => Enumerable.Empty<BoundNode>();
    }
}