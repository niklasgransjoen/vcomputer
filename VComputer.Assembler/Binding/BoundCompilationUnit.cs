using System.Collections.Immutable;

namespace VComputer.Assembler.Binding
{
    internal sealed class BoundCompilationUnit
    {
        public BoundCompilationUnit(
            ImmutableHashSet<LabelSymbol> labels,
            ImmutableHashSet<ConstantSymbol> constants,
            ImmutableArray<BoundStatement> statements,
            ImmutableArray<Diagnostic> diagnostics)
        {
            Labels = labels;
            Constants = constants;
            Statements = statements;
            Diagnostics = diagnostics;
        }

        public ImmutableHashSet<LabelSymbol> Labels { get; }
        public ImmutableHashSet<ConstantSymbol> Constants { get; }
        public ImmutableArray<BoundStatement> Statements { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
    }
}