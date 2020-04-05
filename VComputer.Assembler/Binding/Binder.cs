using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using VComputer.Assembler.Symbols;
using VComputer.Assembler.Syntax;

namespace VComputer.Assembler.Binding
{
    internal sealed class Binder
    {
        #region Fields

        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();

        private readonly Dictionary<string, int> _instructions;
        private readonly SymbolDictionary<DirectiveSymbol> _directives = new SymbolDictionary<DirectiveSymbol>(DirectiveSymbol.Directives.ToDictionary(d => d.Name), DirectiveSymbol.NOOP);

        private readonly SymbolDictionary<MacroSymbol> _macros = new SymbolDictionary<MacroSymbol>(name => new MacroSymbol(name, 0));
        private readonly SymbolDictionary<ConstantSymbol> _constants = new SymbolDictionary<ConstantSymbol>(name => new ConstantSymbol(name));

        private readonly SymbolDictionary<LabelSymbol> _labels;
        private readonly ImmutableArray<BoundStatement> _statements;

        #endregion Fields

        #region Constructor

        private Binder(ImmutableArray<AssemblyInstruction> instructions, ImmutableArray<StatementSyntax> statements)
        {
            _instructions = instructions.ToDictionary(i => i.Command, i => i.OpCode);
            _labels = DetectLabelDeclarationStatements(statements);
            _statements = BindStatements(statements);
        }

        public static BoundCompilationUnit Bind(CompilationUnitSyntax compilationUnit, ImmutableArray<AssemblyInstruction> instructions)
        {
            var binder = new Binder(instructions, compilationUnit.Statements);

            var statements = binder._statements;
            var diagnostics = ImmutableArray.CreateRange(binder.GetDiagnostics());

            return new BoundCompilationUnit(statements, diagnostics);
        }

        #endregion Constructor

        private SymbolDictionary<LabelSymbol> DetectLabelDeclarationStatements(IEnumerable<StatementSyntax> statements)
        {
            var labels = new SymbolDictionary<LabelSymbol>(name => new LabelSymbol(name));
            foreach (LabelDeclarationStatement statement in statements.Where(s => s.Kind == SyntaxKind.LabelDeclarationStatement))
            {
                labels.TryDeclare(statement.Identifier, name => new LabelSymbol(name), out var label);
            }

            return labels;
        }

        private ImmutableArray<BoundStatement> BindStatements(IEnumerable<StatementSyntax> statements, IReadOnlySymbolDictionary<Symbol>? localSymbols = null)
        {
            var builder = ImmutableArray.CreateBuilder<BoundStatement>();
            foreach (var statement in statements)
            {
                var boundStatement = BindStatement(statement, localSymbols);
                builder.Add(boundStatement);
            }

            return builder.ToImmutable();
        }

        #region Statement

        private BoundStatement BindStatement(StatementSyntax statement, IReadOnlySymbolDictionary<Symbol>? localSymbols = null) => statement.Kind switch
        {
            SyntaxKind.CommandStatement => BindCommandStatement((CommandStatement)statement, localSymbols),
            SyntaxKind.MacroStatement => BindMacroStatement((MacroStatement)statement, localSymbols),
            SyntaxKind.DirectiveStatement => BindDirectiveStatement((DirectiveStatement)statement, localSymbols),
            SyntaxKind.MacroDefinitionStatement => BindMacroDefinitionStatement((MacroDefinitionStatement)statement),
            SyntaxKind.ConstantDeclarationStatement => BindConstantDeclarationStatement((ConstantDeclarationStatement)statement, localSymbols),
            SyntaxKind.LabelDeclarationStatement => BindLabelDeclarationStatement((LabelDeclarationStatement)statement),

            _ => throw new Exception($"Unexpected syntax '{statement.Kind}'."),
        };

        private BoundCommandStatement BindCommandStatement(CommandStatement statement, IReadOnlySymbolDictionary<Symbol>? localSymbols = null)
        {
            string name = statement.CommandToken.ToString();
            if (_instructions.TryGetValue(name, out var opCode))
            {
                BoundExpression? operand = null;
                if (statement.Operand != null)
                    operand = BindExpression(statement.Operand, localSymbols);

                return new BoundCommandStatement(opCode, operand);
            }

            _diagnostics.ReportUndefinedCommandOrMacro(statement.CommandToken.Span, name);

            opCode = 0;
            _instructions.Add(name, opCode);
            return new BoundCommandStatement(opCode, null);
        }

        private BoundMacroStatement BindMacroStatement(MacroStatement statement, IReadOnlySymbolDictionary<Symbol>? localSymbols = null)
        {
            if (!_macros.TryFind(statement.MacroToken, out var macro))
            {
                return new BoundMacroStatement(macro, ImmutableArray<BoundExpression>.Empty);
            }

            var operands = ImmutableArray.CreateBuilder<BoundExpression>();
            foreach (var operand in statement.Operands)
            {
                var boundOperands = BindExpression(operand, localSymbols);
                operands.Add(boundOperands);
            }

            return new BoundMacroStatement(macro, operands.ToImmutable());
        }

        private BoundDirectiveStatement BindDirectiveStatement(DirectiveStatement statement, IReadOnlySymbolDictionary<Symbol>? localSymbols = null)
        {
            _directives.TryFind(statement.DirectiveToken, out var directive);

            BoundExpression? operand = null;
            if (directive.HasOperand)
            {
                if (statement.OperandExpression != null)
                    operand = BindExpression(statement.OperandExpression, localSymbols);
                else
                    _diagnostics.ReportMissingOperand(statement.Span);
            }

            return new BoundDirectiveStatement(directive, operand);
        }

        private BoundMacroDeclarationStatement BindMacroDefinitionStatement(MacroDefinitionStatement statement)
        {
            _macros.TryDeclare(statement.Identifier, name => new MacroSymbol(name, statement.Parameters.Length), out var macro);
            var parameters = BindMacroParameters(statement.Parameters);
            var statements = BindStatements(statement.Statements, parameters);

            return new BoundMacroDeclarationStatement(macro, parameters.Values.ToImmutableArray(), statements);
        }

        private SymbolDictionary<MacroParameterSymbol> BindMacroParameters(ImmutableArray<SyntaxToken> parameters)
        {
            SymbolDictionary<MacroParameterSymbol> parameterSymbols = new SymbolDictionary<MacroParameterSymbol>(name => new MacroParameterSymbol(name));
            foreach (var parameter in parameters)
            {
                parameterSymbols.TryDeclare(parameter, name => new MacroParameterSymbol(name), out var parameterSymbol);
            }
            return parameterSymbols;
        }

        private BoundConstantDeclarationStatement BindConstantDeclarationStatement(ConstantDeclarationStatement statement, IReadOnlySymbolDictionary<Symbol>? localSymbols = null)
        {
            _constants.TryDeclare(statement.Identifier, name => new ConstantSymbol(name), out var constant);
            var expression = BindExpression(statement.Expression, localSymbols);

            return new BoundConstantDeclarationStatement(constant, expression);
        }

        private BoundLabelDeclarationStatement BindLabelDeclarationStatement(LabelDeclarationStatement statement)
        {
            string name = statement.Identifier.ToString();
            return new BoundLabelDeclarationStatement(_labels[name]);
        }

        #endregion Statement

        #region Expression

        private BoundExpression BindExpression(ExpressionSyntax expression, IReadOnlySymbolDictionary<Symbol>? localSymbols = null)
        {
            return expression.Kind switch
            {
                SyntaxKind.LiteralExpression => BindLiteralExpression((LiteralExpression)expression),
                SyntaxKind.NameExpression => BindNameExpression((NameExpression)expression, localSymbols),

                _ => throw new Exception($"Unexpected syntax '{expression.Kind}'."),
            };
        }

        private BoundExpression BindLiteralExpression(LiteralExpression expression)
        {
            return new BoundLiteralExpression(expression.Value);
        }

        private BoundExpression BindNameExpression(NameExpression expression, IReadOnlySymbolDictionary<Symbol>? localSymbols = null)
        {
            var localSymbol = localSymbols?.TryFind(expression.Identifier, handleError: false);
            if (localSymbol != null)
            {
                return new BoundSymbolExpression(localSymbol);
            }

            if (_labels.TryFind(expression.Identifier, out var label, handleError: false))
            {
                return new BoundSymbolExpression(label);
            }

            if (_constants.TryFind(expression.Identifier, out var constant, handleError: true))
            {
                return new BoundSymbolExpression(constant);
            }

            return BoundErrorExpression.Instance;
        }

        #endregion Expression

        private IEnumerable<Diagnostic> GetDiagnostics()
        {
            return _diagnostics
                .Concat(_directives.Diagnostics)
                .Concat(_macros.Diagnostics)
                .Concat(_constants.Diagnostics)
                .Concat(_labels.Diagnostics);
        }

        #region Utility classes

        private interface IReadOnlySymbolDictionary<out TSymbol>
            where TSymbol : Symbol
        {
            TSymbol this[string key] { get; }

            TSymbol? TryFind(SyntaxToken identifier, bool handleError = true);
        }

        private sealed class SymbolDictionary<TSymbol> : Dictionary<string, TSymbol>, IReadOnlySymbolDictionary<TSymbol>
            where TSymbol : Symbol
        {
            private readonly TSymbol _fallback = null!;
            private readonly Func<string, TSymbol> _fallbackFactory = null!;

            public SymbolDictionary(TSymbol fallback)
            {
                _fallback = fallback;
            }

            public SymbolDictionary(Func<string, TSymbol> fallbackFactory)
            {
                _fallbackFactory = fallbackFactory;
            }

            public SymbolDictionary(IEnumerable<KeyValuePair<string, TSymbol>> keyValuePairs, TSymbol fallback)
                : base(keyValuePairs)
            {
                _fallback = fallback;
            }

            public SymbolDictionary(IEnumerable<KeyValuePair<string, TSymbol>> keyValuePairs, Func<string, TSymbol> fallbackFactory)
                : base(keyValuePairs)
            {
                _fallbackFactory = fallbackFactory;
            }

            public DiagnosticBag Diagnostics { get; } = new DiagnosticBag();

            public bool TryFind(
                SyntaxToken identifier,
                out TSymbol symbol,
                bool handleError = true)
            {
                string name = identifier.ToString();
                if (TryGetValue(name, out symbol!))
                {
                    return true;
                }

                symbol = _fallback ?? _fallbackFactory(name);
                if (handleError)
                {
                    Diagnostics.ReportUndefinedSymbol(identifier.Span, name);
                    Add(name, symbol);
                }

                return false;
            }

            public TSymbol? TryFind(SyntaxToken identifier, bool handleError = true)
            {
                string name = identifier.ToString();
                if (TryGetValue(name, out var symbol))
                {
                    return symbol;
                }

                if (handleError)
                {
                    Diagnostics.ReportUndefinedSymbol(identifier.Span, name);

                    symbol = _fallback ?? _fallbackFactory(name);
                    Add(name, symbol);
                }

                return null;
            }

            public bool TryDeclare(SyntaxToken identifier, Func<string, TSymbol> factory, out TSymbol symbol, bool handleError = true)
            {
                string name = identifier.ToString();
                if (TryGetValue(name, out symbol!))
                {
                    if (handleError)
                    {
                        switch (symbol)
                        {
                            case MacroSymbol _:
                                Diagnostics.ReportMacroAlreadyDeclared(identifier.Span, name);
                                break;

                            case MacroParameterSymbol _:
                                Diagnostics.ReportDuplicateMacroParameter(identifier.Span, name);
                                break;

                            case ConstantSymbol _:
                                Diagnostics.ReportConstantAlreadyDeclared(identifier.Span, name);
                                break;

                            case LabelSymbol _:
                                Diagnostics.ReportLabelAlreadyDeclared(identifier.Span, name);
                                break;

                            default:
                                throw new Exception($"Unexpected symbol type '{typeof(TSymbol)}'.");
                        }
                    }

                    return false;
                }

                symbol = factory(name);
                Add(name, symbol);
                return true;
            }
        }

        #endregion Utility classes
    }
}