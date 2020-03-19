using System;
using VComputer.Assembler.Text;

namespace VComputer.Assembler
{
    public sealed class AssemblyDiagnosticException : Exception
    {
        internal AssemblyDiagnosticException(SourceText text, ReadOnlyMemory<Diagnostic> diagnostics)
        {
            Text = text;
            Diagnostics = diagnostics;
        }

        public SourceText Text { get; }
        public ReadOnlyMemory<Diagnostic> Diagnostics { get; }
    }
}