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

        private int _locationPointer = 0;

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
            int[] code = new int[CalculateProgramSize(_compilationUnit.Statements)];

            foreach (var statement in _compilationUnit.Statements)
            {
                if (EvaluateStatement(statement, out int result))
                {
                    code[_locationPointer] = result;
                    _locationPointer++;
                }
            }

            return code;
        }

        #region Statement

        private bool EvaluateStatement(BoundStatement statement, out int result)
        {
            return statement.Kind switch
            {
                BoundNodeKind.CommandStatement => EvaluateCommandStatement((BoundCommandStatement)statement, out result),
                BoundNodeKind.DirectiveStatement => EvaluateDirectiveStatement((BoundDirectiveStatement)statement, out result),
                BoundNodeKind.ConstantDeclarationStatement => EvaluateNoOpStatement(out result),
                BoundNodeKind.LabelDeclarationStatement => EvaluateNoOpStatement(out result),

                _ => throw new Exception($"Unexpected syntax '{statement.Kind}'."),
            };
        }

        private bool EvaluateCommandStatement(BoundCommandStatement statement, out int result)
        {
            result = statement.OpCode << _bits / 2;
            if (statement.Operand != null)
            {
                result += EvaluateExpression(statement.Operand);
            }

            return true;
        }

        private bool EvaluateDirectiveStatement(BoundDirectiveStatement statement, out int result)
        {
            if (statement.Directive == Directive.ORG)
            {
                _locationPointer = EvaluateExpression(statement.Operand!);
            }
            else if (statement.Directive == Directive.WORD)
            {
                result = EvaluateExpression(statement.Operand!);
                return true;
            }

            result = 0;
            return false;
        }

        private bool EvaluateNoOpStatement(out int result)
        {
            result = 0;
            return false;
        }

        #endregion Statement

        #region Expression

        private int EvaluateExpression(BoundExpression expression)
        {
            return expression.Kind switch
            {
                BoundNodeKind.LiteralExpression => EvaluateLiteralExpression((BoundLiteralExpression)expression),
                BoundNodeKind.ConstantExpression => EvaluateConstantExpression((BoundConstantExpression)expression),
                BoundNodeKind.LabelExpression => EvaluateLabelExpression((BoundLabelExpression)expression),

                _ => throw new Exception($"Unexpected syntax '{expression.Kind}'."),
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

        #region Utilities

        private int CalculateProgramSize(IEnumerable<BoundStatement> statements)
        {
            int programSize = 0;
            int maxProgramSize = 0;

            foreach (var statement in statements)
            {
                if (statement.Kind == BoundNodeKind.CommandStatement)
                    programSize++;
                else if (statement.Kind == BoundNodeKind.DirectiveStatement)
                {
                    var directiveStatement = (BoundDirectiveStatement)statement;
                    if (directiveStatement.Directive == Directive.ORG)
                    {
                        maxProgramSize = Math.Max(programSize, maxProgramSize);
                        programSize = EvaluateExpression(directiveStatement.Operand!);
                    }
                    else if (directiveStatement.Directive == Directive.WORD)
                    {
                        programSize++;
                    }
                }
            }

            return Math.Max(programSize, maxProgramSize);
        }

        #endregion Utilities
    }
}