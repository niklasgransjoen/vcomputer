using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VComputer.Assembler.Symbols;

namespace VComputer.Assembler.Binding
{
    [DebuggerDisplay("{Label}")]
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