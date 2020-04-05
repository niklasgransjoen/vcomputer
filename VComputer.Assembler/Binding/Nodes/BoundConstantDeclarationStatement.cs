using System.Collections.Generic;
using System.Diagnostics;
using VComputer.Assembler.Symbols;

namespace VComputer.Assembler.Binding
{
    [DebuggerDisplay("{Constant} {Expression}")]
    internal sealed class BoundConstantDeclarationStatement : BoundStatement
    {
        public BoundConstantDeclarationStatement(ConstantSymbol constant, BoundExpression expression)
        {
            Constant = constant;
            Expression = expression;
        }

        public override BoundNodeKind Kind => BoundNodeKind.ConstantDeclarationStatement;

        public ConstantSymbol Constant { get; }
        public BoundExpression Expression { get; }

        public override IEnumerable<BoundNode> GetChildren()
        {
            yield return Expression;
        }
    }
}