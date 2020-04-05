using System.Collections.Generic;
using System.Diagnostics;

namespace VComputer.Assembler.Binding
{
    [DebuggerDisplay("{OpCode} {Operand}")]
    internal sealed class BoundCommandStatement : BoundStatement
    {
        public BoundCommandStatement(int opCode, BoundExpression? operand)
        {
            OpCode = opCode;
            Operand = operand;
        }

        public override BoundNodeKind Kind => BoundNodeKind.CommandStatement;
        public int OpCode { get; }
        public BoundExpression? Operand { get; }

        public override IEnumerable<BoundNode> GetChildren()
        {
            if (Operand != null)
                yield return Operand;
        }
    }
}