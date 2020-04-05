﻿using System.Collections.Immutable;
using VComputer.Assembler.Symbols;

namespace VComputer.Assembler.Binding
{
    internal sealed class BoundCompilationUnit
    {
        public BoundCompilationUnit(
            ImmutableArray<BoundStatement> statements,
            ImmutableArray<Diagnostic> diagnostics)
        {
            Statements = statements;
            Diagnostics = diagnostics;
        }

        public ImmutableArray<BoundStatement> Statements { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
    }
}