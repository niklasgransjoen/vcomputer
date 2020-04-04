using System.Collections.Immutable;

namespace VComputer.Assembler.Binding
{
    internal sealed class BoundCompilationUnit
    {
        public BoundCompilationUnit(
            ImmutableHashSet<LabelSymbol> labels,
            ImmutableArray<BoundStatement> statements,
            ImmutableArray<Diagnostic> diagnostics)
        {
            Labels = labels;
            Statements = statements;
            Diagnostics = diagnostics;
        }

        public ImmutableHashSet<LabelSymbol> Labels { get; }
        public ImmutableArray<BoundStatement> Statements { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
    }
}