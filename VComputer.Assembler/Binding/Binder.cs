using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using VComputer.Assembler.Syntax;

namespace VComputer.Assembler.Binding
{
    internal sealed class Binder
    {
        #region Fields

        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();

        private readonly Dictionary<string, int> _instructions;
        private readonly Dictionary<string, LabelSymbol> _labels = new Dictionary<string, LabelSymbol>();
        private readonly Dictionary<string, ConstantSymbol> _constants = new Dictionary<string, ConstantSymbol>();
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

            var labels = binder._labels.Values.ToImmutableHashSet();
            var constants = binder._constants.Values.ToImmutableHashSet();
            var statements = binder._statements;
            var diagnostics = ImmutableArray.CreateRange(binder._diagnostics);

            return new BoundCompilationUnit(labels, constants, statements, diagnostics);
        }

        #endregion Constructor

        private Dictionary<string, LabelSymbol> DetectLabelDeclarationStatements(IEnumerable<StatementSyntax> statements)
        {
            var labels = new Dictionary<string, LabelSymbol>();
            foreach (LabelDeclarationStatement statement in statements.Where(s => s.Kind == SyntaxKind.LabelDeclarationStatement))
            {
                string name = statement.LabelToken.Value?.ToString() ?? throw new ArgumentException("Missing label value");
                if (!labels.ContainsKey(name))
                {
                    labels.Add(name, new LabelSymbol(name));
                }
                else
                {
                    _diagnostics.ReportLabelAlreadyDeclared(statement.LabelToken.Span, name);
                }
            }

            return labels;
        }

        private ImmutableArray<BoundStatement> BindStatements(ImmutableArray<StatementSyntax> statements)
        {
            var builder = ImmutableArray.CreateBuilder<BoundStatement>();
            foreach (var statement in statements)
            {
                builder.Add(BindStatement(statement));
            }

            return builder.ToImmutable();
        }

        #region Statement

        private BoundStatement BindStatement(StatementSyntax statement) => statement.Kind switch
        {
            SyntaxKind.CommandStatement => BindCommandStatement((CommandStatement)statement),
            SyntaxKind.ConstantDeclarationStatement => BindConstantDeclarationStatement((ConstantDeclarationStatement)statement),
            SyntaxKind.LabelDeclarationStatement => BindLabelStatement((LabelDeclarationStatement)statement),

            _ => throw new Exception($"Unexpected syntax '{statement.Kind}'."),
        };

        private BoundCommandStatement BindCommandStatement(CommandStatement statement)
        {
            string command = statement.CommandToken.Text.ToString();
            if (!_instructions.TryGetValue(command, out var opCode))
            {
                _diagnostics.ReportUndefinedCommand(statement.CommandToken.Span, command);
                _instructions[command] = 0;
            }

            BoundExpression? operand = null;
            if (statement.OperandExpression != null)
                operand = BindExpression(statement.OperandExpression);

            return new BoundCommandStatement(opCode, operand);
        }

        private BoundConstantDeclarationStatement BindConstantDeclarationStatement(ConstantDeclarationStatement statement)
        {
            string name = statement.Identifier.ToString();
            var expression = BindExpression(statement.Expression);

            if (!TryDeclareConstant(name, out var constant))
            {
                _diagnostics.ReportConstantAlreadyDeclared(statement.Identifier.Span, name);
            }

            return new BoundConstantDeclarationStatement(constant, expression);
        }

        private BoundLabelDeclarationStatement BindLabelStatement(LabelDeclarationStatement statement)
        {
            string name = statement.LabelToken.Value?.ToString()!;
            return new BoundLabelDeclarationStatement(_labels[name]);
        }

        #endregion Statement

        #region Expression

        private BoundExpression BindExpression(ExpressionSyntax expression)
        {
            return expression.Kind switch
            {
                SyntaxKind.LiteralExpression => BindLiteralExpression((LiteralExpression)expression),
                SyntaxKind.NameExpression => BindConstantExpression((ConstantExpression)expression),
                SyntaxKind.LabelExpression => BindLabelExpression((LabelExpression)expression),

                _ => throw new Exception($"Unexpected syntax '{expression.Kind}'."),
            };
        }

        private BoundExpression BindLiteralExpression(LiteralExpression expression)
        {
            return new BoundLiteralExpression(expression.Value);
        }

        private BoundExpression BindConstantExpression(ConstantExpression expression)
        {
            if (TryFindConstant(expression.Identifier, out var constant))
            {
                return new BoundConstantExpression(constant);
            }

            return BoundErrorExpression.Instance;
        }

        private BoundExpression BindLabelExpression(LabelExpression expression)
        {
            string name = expression.LabelToken.ToString();
            if (_labels.TryGetValue(name, out var label))
            {
                return new BoundLabelExpression(label);
            }

            _diagnostics.ReportUndefinedSymbol(expression.LabelToken.Span, name);

            _labels[name] = new LabelSymbol(name);
            return BoundErrorExpression.Instance;
        }

        #endregion Expression

        #region Helpers

        private bool TryDeclareConstant(string name, out ConstantSymbol constant)
        {
            if (_constants.TryGetValue(name, out constant!))
            {
                return false;
            }

            constant = new ConstantSymbol(name);
            _constants.Add(name, constant);
            return true;
        }

        private bool TryFindConstant(SyntaxToken identifier, [NotNullWhen(true)] out ConstantSymbol? constant)
        {
            string name = identifier.Text.ToString();
            if (_constants.TryGetValue(name, out constant))
            {
                return true;
            }

            _diagnostics.ReportUndefinedSymbol(identifier.Span, name);
            _constants.Add(name, new ConstantSymbol(name));
            return false;
        }

        #endregion Helpers
    }
}