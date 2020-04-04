using System.Collections.Generic;
using System.Linq;

namespace VComputer.Assembler.Binding
{
    internal sealed class BoundLabelDeclarationStatement : BoundStatement
    {
        public BoundLabelDeclarationStatement(LabelSymbol label)
        {
            Label = label;
        }

        public override BoundNodeKind Kind => BoundNodeKind.LabelDeclarationStatement;
        public LabelSymbol Label { get; }

        public override IEnumerable<BoundNode> GetChildren() => Enumerable.Empty<BoundNode>();
    }
}