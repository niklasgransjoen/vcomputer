using System.Collections.Immutable;
using VComputer.Assembler.Binding;
using VComputer.Assembler.Syntax;

namespace VComputer.Assembler
{
    public sealed class Assembler
    {
        private readonly ImmutableArray<AssemblyInstruction> _definition;

        public Assembler(ImmutableArray<AssemblyInstruction> definition)
        {
            _definition = definition;
        }

        public int[] Assemble(string program, int bits)
        {
            var syntaxTree = SyntaxTree.Parse(program);
            if (syntaxTree.Diagnostics.Length != 0)
            {
                throw new AssemblyDiagnosticException(syntaxTree.Text, syntaxTree.Diagnostics);
            }

            var boundCompilation = Binder.Bind(syntaxTree.Root, _definition);
            if (boundCompilation.Diagnostics.Length != 0)
            {
                throw new AssemblyDiagnosticException(syntaxTree.Text, boundCompilation.Diagnostics);
            }

            return Evalutator.Evaluate(boundCompilation, bits);
        }
    }
}