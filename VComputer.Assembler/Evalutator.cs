using System;
using System.Collections.Generic;
using System.Linq;
using VComputer.Assembler.Binding;
using VComputer.Assembler.Extensions;
using VComputer.Assembler.Symbols;

namespace VComputer.Assembler
{
    internal sealed class Evalutator
    {
        private readonly BoundCompilationUnit _compilationUnit;
        private readonly int _bits;

        private readonly Dictionary<MacroSymbol, BoundMacroDeclarationStatement> _macros = new Dictionary<MacroSymbol, BoundMacroDeclarationStatement>();
        private readonly Dictionary<Symbol, int> _globalSymbols = new Dictionary<Symbol, int>();

        private readonly int[] _code;
        private int _locationPointer = 0;

        #region Constructor

        private Evalutator(BoundCompilationUnit compilationUnit, int bits)
        {
            _compilationUnit = compilationUnit;
            _bits = bits;

            InitializeSymbols();
            _code = new int[CalculateProgramSize()];
        }

        private void InitializeSymbols()
        {
            var symbols = new Dictionary<Symbol, int>();

            // Macros
            var macroDeclarations = _compilationUnit.Statements
                .Where(s => s.Kind == BoundNodeKind.MacroDeclarationStatement)
                .Cast<BoundMacroDeclarationStatement>();
            _macros.AddRange(macroDeclarations.ToDictionary(m => m.Macro));

            // Constants
            var constantDeclarations = _compilationUnit.Statements
                .Where(s => s.Kind == BoundNodeKind.ConstantDeclarationStatement)
                .Cast<BoundConstantDeclarationStatement>();
            foreach (var constantDeclaration in constantDeclarations)
            {
                int value = EvaluateExpression(constantDeclaration.Expression);
                _globalSymbols.Add(constantDeclaration.Constant, value);
            }

            // Labels
            int labelAddress = 0;
            foreach (var statement in _compilationUnit.Statements)
            {
                if (statement.Kind == BoundNodeKind.LabelDeclarationStatement)
                {
                    var labelStatement = (BoundLabelDeclarationStatement)statement;
                    _globalSymbols.Add(labelStatement.Label, labelAddress);
                }
                else
                {
                    labelAddress += CalculateSize(statement);
                }
            }
        }

        #endregion Constructor

        public static int[] Evaluate(BoundCompilationUnit compilationUnit, int bits)
        {
            var evaluator = new Evalutator(compilationUnit, bits);
            evaluator.EvaluateStatements(compilationUnit.Statements);

            return evaluator._code;
        }

        #region Statement

        private void EvaluateStatements(IEnumerable<BoundStatement> statements, IReadOnlyDictionary<Symbol, int>? localSymbols = null)
        {
            foreach (var statement in statements)
            {
                EvaluateStatement(statement, localSymbols);
            }
        }

        private void EvaluateStatement(BoundStatement statement, IReadOnlyDictionary<Symbol, int>? localSymbols = null)
        {
            switch (statement.Kind)
            {
                case BoundNodeKind.CommandStatement:
                    EvaluateCommandStatement((BoundCommandStatement)statement, localSymbols);
                    break;

                case BoundNodeKind.MacroStatement:
                    EvaluateMacroStatement((BoundMacroStatement)statement, localSymbols);
                    break;

                case BoundNodeKind.DirectiveStatement:
                    EvaluateDirectiveStatement((BoundDirectiveStatement)statement, localSymbols);
                    break;

                case BoundNodeKind.MacroDeclarationStatement:
                case BoundNodeKind.ConstantDeclarationStatement:
                case BoundNodeKind.LabelDeclarationStatement:
                    break;

                default:
                    throw new Exception($"Unexpected syntax '{statement.Kind}'.");
            }
        }

        private void EvaluateCommandStatement(BoundCommandStatement statement, IReadOnlyDictionary<Symbol, int>? localSymbols = null)
        {
            var value = statement.OpCode << _bits / 2;
            if (statement.Operand != null)
            {
                value += EvaluateExpression(statement.Operand, localSymbols);
            }

            PushValue(value);
        }

        private void EvaluateMacroStatement(BoundMacroStatement statement, IReadOnlyDictionary<Symbol, int>? localSymbols = null)
        {
            var macro = _macros[statement.Macro];
            var parameters = evaluateParameters();

            EvaluateStatements(macro.Statements, parameters);

            Dictionary<Symbol, int> evaluateParameters()
            {
                var parameters = new Dictionary<Symbol, int>();
                for (int i = 0; i < macro.Parameters.Length; i++)
                {
                    var parameterSymbol = macro.Parameters[i];
                    var parameterValue = EvaluateExpression(statement.Operands[i], localSymbols);
                    parameters.Add(parameterSymbol, parameterValue);
                }

                return parameters;
            }
        }

        private void EvaluateDirectiveStatement(BoundDirectiveStatement statement, IReadOnlyDictionary<Symbol, int>? localSymbols = null)
        {
            if (statement.Directive == DirectiveSymbol.ORG)
            {
                _locationPointer = EvaluateExpression(statement.Operand!, localSymbols);
            }
            else if (statement.Directive == DirectiveSymbol.WORD)
            {
                var value = EvaluateExpression(statement.Operand!, localSymbols);
                PushValue(value);
            }
        }

        #endregion Statement

        #region Expression

        private int EvaluateExpression(BoundExpression expression, IReadOnlyDictionary<Symbol, int>? localSymbols = null)
        {
            return expression.Kind switch
            {
                BoundNodeKind.LiteralExpression => EvaluateLiteralExpression((BoundLiteralExpression)expression),
                BoundNodeKind.SymbolExpression => EvaluateLabelExpression((BoundSymbolExpression)expression, localSymbols),

                _ => throw new Exception($"Unexpected syntax '{expression.Kind}'."),
            };
        }

        private int EvaluateLiteralExpression(BoundLiteralExpression expression) => expression.Value;

        private int EvaluateLabelExpression(BoundSymbolExpression expression, IReadOnlyDictionary<Symbol, int>? localSymbols = null)
        {
            if (localSymbols != null && localSymbols.TryGetValue(expression.Symbol, out var value))
                return value;

            return _globalSymbols[expression.Symbol];
        }

        #endregion Expression

        #region Helpers

        private int CalculateProgramSize()
        {
            int programSize = 0;
            int maxProgramSize = 0;

            foreach (var statement in _compilationUnit.Statements)
            {
                programSize += CalculateSize(statement);
                if (statement.Kind == BoundNodeKind.DirectiveStatement)
                {
                    var directiveStatement = (BoundDirectiveStatement)statement;
                    if (directiveStatement.Directive == DirectiveSymbol.ORG)
                    {
                        maxProgramSize = Math.Max(programSize, maxProgramSize);
                        programSize = EvaluateExpression(directiveStatement.Operand!);
                    }
                }
            }

            return Math.Max(programSize, maxProgramSize);
        }

        private int CalculateSize(BoundStatement statement)
        {
            switch (statement.Kind)
            {
                case BoundNodeKind.CommandStatement:
                    return 1;

                case BoundNodeKind.MacroStatement:
                    var macroStatement = (BoundMacroStatement)statement;
                    return _macros[macroStatement.Macro].Statements.Sum(statement => CalculateSize(statement));

                case BoundNodeKind.DirectiveStatement when
                    statement is BoundDirectiveStatement directiveStatement &&
                    directiveStatement.Directive == DirectiveSymbol.WORD:
                    {
                        return 1;
                    }

                default:
                    return 0;
            }
        }

        private void PushValue(int value)
        {
            _code[_locationPointer] = value;
            _locationPointer++;
        }

        #endregion Helpers
    }
}