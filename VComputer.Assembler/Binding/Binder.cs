using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using VComputer.Assembler.Syntax;

namespace VComputer.Assembler.Binding
{
    internal sealed class Binder
    {
        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();
        private readonly Dictionary<string, LabelSymbol> _labels;
        private readonly Dictionary<string, int> _instructions;

        private Binder(IEnumerable<LabelSymbol> labels, ImmutableArray<AssemblyInstruction> instructions)
        {
            _labels = labels.ToDictionary(label => label.Name);
            _instructions = instructions.ToDictionary(i => i.Command, i => i.OpCode);
        }

        public static BoundCompilationUnit Bind(CompilationUnitSyntax compilationUnit, ImmutableArray<AssemblyInstruction> instructions)
        {
            var labels = compilationUnit.Statements
                .Where(s => s.Kind == SyntaxKind.LabelStatement)
                .Cast<LabelStatement>()
                .Select(label => new LabelSymbol(label.LabelToken.Value?.ToString() ?? throw new ArgumentException("Missing label value")))
                .ToImmutableHashSet();

            var binder = new Binder(labels, instructions);

            var statements = binder.BindStatements(compilationUnit.Statements);
            var diagnostics = ImmutableArray.CreateRange(binder._diagnostics);

            return new BoundCompilationUnit(labels, statements, diagnostics);
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
            SyntaxKind.LabelStatement => BindLabelStatement((LabelStatement)statement),

            _ => throw new Exception($"Unexpected syntax '{statement.Kind}'."),
        };

        private BoundCommandStatement BindCommandStatement(CommandStatement statement)
        {
            string command = statement.OperatorStatement.CommandToken.Text.ToString();
            if (!_instructions.TryGetValue(command, out var opCode))
            {
                _diagnostics.ReportUndefinedCommand(statement.OperatorStatement.CommandToken.Span, command);
                _instructions[command] = 0;
            }

            BoundExpression? operand = null;
            if (statement.OperandExpression != null)
                operand = BindExpression(statement.OperandExpression);

            return new BoundCommandStatement(opCode, operand);
        }

        private BoundLabelStatement BindLabelStatement(LabelStatement statement)
        {
            string name = statement.LabelToken.Value?.ToString()!;
            return new BoundLabelStatement(_labels[name]);
        }

        #endregion Statement

        #region Expression

        private BoundExpression BindExpression(ExpressionSyntax expression)
        {
            return expression.Kind switch
            {
                SyntaxKind.LiteralExpression => BindLiteralExpression((LiteralExpression)expression),
                SyntaxKind.LabelExpression => BindLabelExpression((LabelExpression)expression),

                _ => throw new Exception($"Unexpected syntax '{expression.Kind}'."),
            };
        }

        private BoundExpression BindLiteralExpression(LiteralExpression expression)
        {
            return new BoundLiteralExpression(expression.Value);
        }

        private BoundExpression BindLabelExpression(LabelExpression expression)
        {
            string name = expression.LabelToken.Text.ToString();
            if (_labels.TryGetValue(name, out var label))
            {
                return new BoundLabelExpression(label);
            }

            _diagnostics.ReportUndefinedLabel(expression.LabelToken.Span, name);

            _labels[name] = new LabelSymbol(name);
            return BoundErrorExpression.Instance;
        }

        #endregion Expression
    }

    internal sealed class LabelSymbol
    {
        public LabelSymbol(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public override string ToString() => Name;
    }
}