using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using VComputer.Assembler.Symbols;

namespace VComputer.Assembler.Binding
{
    [DebuggerDisplay("{Macro} ({Operands})")]
    internal sealed class BoundMacroStatement : BoundStatement
    {
        public BoundMacroStatement(MacroSymbol macro, ImmutableArray<BoundExpression> operands)
        {
            Macro = macro;
            Operands = operands;
        }

        public override BoundNodeKind Kind => BoundNodeKind.MacroStatement;
        public MacroSymbol Macro { get; }
        public ImmutableArray<BoundExpression> Operands { get; }

        public override IEnumerable<BoundNode> GetChildren() => Operands;
    }
}