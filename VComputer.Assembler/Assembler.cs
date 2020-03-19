using System;
using System.Collections.Generic;
using System.Linq;
using VComputer.Assembler.Syntax;

namespace VComputer.Assembler
{
    public sealed class Assembler
    {
        private readonly Dictionary<string, int> _opCodes;

        public Assembler(IEnumerable<AssemblyInstruction> definition)
        {
            if (definition is null)
                throw new ArgumentNullException(nameof(definition));

            _opCodes = definition.ToDictionary(d => d.Command, d => d.OpCode);
        }

        public int[] Assemble(string program, int bits)
        {
            var syntaxTree = SyntaxTree.Parse(program);
            if (syntaxTree.Diagnostics.Length != 0)
            {
                throw new AssemblyDiagnosticException(syntaxTree.Text, syntaxTree.Diagnostics);
            }

            return Assemble(syntaxTree.Root.Commands.Span, bits);
        }

        private int[] Assemble(ReadOnlySpan<CommandStatementSyntax> commands, int bits)
        {
            int[] code = new int[commands.Length];
            for (int i = 0; i < commands.Length; i++)
            {
                var command = commands[i];
                code[i] = _opCodes[command.OperatorStatement.Command] << bits / 2;
                if (command.OperandExpression != null)
                {
                    code[i] += command.OperandExpression.Value;
                }
            }

            return code;
        }
    }
}