using System;
using System.Collections.Generic;
using System.Linq;
using VComputer.Assembler.Binding;

namespace VComputer.Assembler
{
    internal sealed class Evalutator
    {
        private readonly BoundCompilationUnit _compilationUnit;
        private readonly Dictionary<LabelSymbol, int> _labelAddresses = new Dictionary<LabelSymbol, int>();

        #region Constructor

        private Evalutator(BoundCompilationUnit compilationUnit)
        {
            _compilationUnit = compilationUnit;
            InitializeLabelMappings();
        }

        private void InitializeLabelMappings()
        {
            int address = 0;
            foreach (var statement in _compilationUnit.Statements)
            {
                if (statement.Kind == BoundNodeKind.LabelStatement)
                {
                    var labelStatement = (BoundLabelStatement)statement;
                    _labelAddresses[labelStatement.LabelSymbol] = address;
                }
                else
                {
                    address++;
                }
            }
        }

        #endregion Constructor

        public static int[] Evaluate(BoundCompilationUnit compilationUnit, int bits)
        {
            var evaluator = new Evalutator(compilationUnit);
            return evaluator.Evaluate(bits);
        }

        private int[] Evaluate(int bits)
        {
            int[] code = new int[_compilationUnit.Statements.Count(s => s.Kind == BoundNodeKind.CommandStatement)];

            int index = 0;
            foreach (BoundCommandStatement statement in _compilationUnit.Statements.Where(s => s.Kind == BoundNodeKind.CommandStatement))
            {
                code[index] = statement.OpCode << bits / 2;
                if (statement.Operand != null)
                {
                    code[index] += EvaluateExpression(statement.Operand);
                }

                ++index;
            }

            return code;
        }

        private int EvaluateExpression(BoundExpression expression)
        {
            return expression.Kind switch
            {
                BoundNodeKind.LiteralExpression => EvaluateLiteralExpression((BoundLiteralExpression)expression),
                BoundNodeKind.LabelExpression => EvaluateLabelExpression((BoundLabelExpression)expression),

                _ => throw new Exception($"Unexpected expression '{expression.Kind}'."),
            };
        }

        private int EvaluateLiteralExpression(BoundLiteralExpression expression) => expression.Value;

        private int EvaluateLabelExpression(BoundLabelExpression expression)
        {
            return _labelAddresses[expression.LabelSymbol];
        }
    }
}