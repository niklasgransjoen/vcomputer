using System;
using System.Collections.Generic;

namespace VComputer.Assembler.Binding
{
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