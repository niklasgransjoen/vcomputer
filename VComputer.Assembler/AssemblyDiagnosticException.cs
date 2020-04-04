using System;
using System.Collections.Immutable;
using VComputer.Assembler.Text;

namespace VComputer.Assembler
{
    public sealed class AssemblyDiagnosticException : Exception
    {
        internal AssemblyDiagnosticException(SourceText text, ImmutableArray<Diagnostic> diagnostics)
        {
            Text = text;
            Diagnostics = diagnostics;
        }

        public SourceText Text { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
    }
}