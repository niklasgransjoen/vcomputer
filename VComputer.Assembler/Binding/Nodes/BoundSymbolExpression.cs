using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VComputer.Assembler.Symbols;

namespace VComputer.Assembler.Binding
{
    [DebuggerDisplay("{Symbol}")]
    internal sealed class BoundSymbolExpression : BoundExpression
    {
        public BoundSymbolExpression(Symbol label)
        {
            Symbol = label;
        }

        public override BoundNodeKind Kind => BoundNodeKind.SymbolExpression;
        public Symbol Symbol { get; }

        public override IEnumerable<BoundNode> GetChildren() => Enumerable.Empty<BoundNode>();
    }
}