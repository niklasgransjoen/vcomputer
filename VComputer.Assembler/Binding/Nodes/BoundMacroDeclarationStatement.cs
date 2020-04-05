using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using VComputer.Assembler.Symbols;

namespace VComputer.Assembler.Binding
{
    [DebuggerDisplay("{Macro} ({Parameters}, {Statements})")]
    internal sealed class BoundMacroDeclarationStatement : BoundStatement
    {
        public BoundMacroDeclarationStatement(
            MacroSymbol macro,
            ImmutableArray<MacroParameterSymbol> parameters,
            ImmutableArray<BoundStatement> statements)
        {
            Macro = macro;
            Parameters = parameters;
            Statements = statements;
        }

        public override BoundNodeKind Kind => BoundNodeKind.MacroDeclarationStatement;
        public MacroSymbol Macro { get; }
        public ImmutableArray<MacroParameterSymbol> Parameters { get; }
        public ImmutableArray<BoundStatement> Statements { get; }

        public override IEnumerable<BoundNode> GetChildren() => Statements;
    }
}