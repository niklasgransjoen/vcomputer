using System;
using System.Collections.Generic;
using System.Linq;
using VComputer.Assembler.Binding;

namespace VComputer.Assembler
{
    internal sealed class Evalutator
    {
        private readonly BoundCompilationUnit _compilationUnit;
        private readonly int _bits;

        private readonly Dictionary<LabelSymbol, int> _labelAddresses;
        private readonly Dictionary<ConstantSymbol, int> _constantValues;

        #region Constructor

        private Evalutator(BoundCompilationUnit compilationUnit, int bits)
        {
            _compilationUnit = compilationUnit;
            _bits = bits;

            _labelAddresses = CreateLabelAddressMappings(compilationUnit.Statements);
            _constantValues = InitializeConstantValues();
        }

        private static Dictionary<LabelSymbol, int> CreateLabelAddressMappings(IEnumerable<BoundStatement> statements)
        {
            var labelAddresses = new Dictionary<LabelSymbol, int>();

            int address = 0;
            foreach (var statement in statements)
            {
                if (statement.Kind == BoundNodeKind.LabelDeclarationStatement)
                {
                    var labelStatement = (BoundLabelDeclarationStatement)statement;
                    labelAddresses.Add(labelStatement.Label, address);
                }
                else if (statement.Kind == BoundNodeKind.CommandStatement)
                {
                    address++;
                }
            }

            return labelAddresses;
        }

        private Dictionary<ConstantSymbol, int> InitializeConstantValues()
        {
            var constantValues = new Dictionary<ConstantSymbol, int>();

            var constantDeclarationStatements = _compilationUnit.Statements.Where(s => s.Kind == BoundNodeKind.ConstantDeclarationStatement);
            foreach (BoundConstantDeclarationStatement constantDeclaration in constantDeclarationStatements)
            {
                int value = EvaluateExpression(constantDeclaration.Expression);
                constantValues.Add(constantDeclaration.Constant, value);
            }

            return constantValues;
        }

        #endregion Constructor

        public static int[] Evaluate(BoundCompilationUnit compilationUnit, int bits)
        {
            var evaluator = new Evalutator(compilationUnit, bits);
            return evaluator.Evaluate();
        }

        private int[] Evaluate()
        {
            int[] code = new int[_compilationUnit.Statements.Count(s => s.Kind == BoundNodeKind.CommandStatement)];

            int index = 0;
            foreach (BoundCommandStatement statement in _compilationUnit.Statements.Where(s => s.Kind == BoundNodeKind.CommandStatement))
            {
                code[index] = statement.OpCode << _bits / 2;
                if (statement.Operand != null)
                {
                    code[index] += EvaluateExpression(statement.Operand);
                }

                ++index;
            }

            return code;
        }

        #region Expression

        private int EvaluateExpression(BoundExpression expression)
        {
            return expression.Kind switch
            {
                BoundNodeKind.LiteralExpression => EvaluateLiteralExpression((BoundLiteralExpression)expression),
                BoundNodeKind.ConstantExpression => EvaluateConstantExpression((BoundConstantExpression)expression),
                BoundNodeKind.LabelExpression => EvaluateLabelExpression((BoundLabelExpression)expression),

                _ => throw new Exception($"Unexpected expression '{expression.Kind}'."),
            };
        }

        private int EvaluateLiteralExpression(BoundLiteralExpression expression) => expression.Value;

        private int EvaluateConstantExpression(BoundConstantExpression expression)
        {
            return _constantValues[expression.Constant];
        }

        private int EvaluateLabelExpression(BoundLabelExpression expression)
        {
            return _labelAddresses[expression.Label];
        }

        #endregion Expression
    }
}