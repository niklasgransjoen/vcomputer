using System.Collections;
using System.Collections.Generic;
using VComputer.Assembler.Syntax;
using VComputer.Assembler.Text;

namespace VComputer.Assembler
{
    internal sealed class DiagnosticBag : IEnumerable<Diagnostic>
    {
        private readonly List<Diagnostic> _diagnostics = new List<Diagnostic>();

        public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void AddRange(DiagnosticBag diagnostics)
        {
            _diagnostics.AddRange(diagnostics);
        }

        private void Report(TextSpan span, string message)
        {
            Diagnostic diagnostic = new Diagnostic(span, message);
            _diagnostics.Add(diagnostic);
        }

        #region Report

        public void ReportBadCharacter(int position, char character)
        {
            TextSpan span = new TextSpan(position, 1);
            string message = $"Bad character input '{character}'.";
            Report(span, message);
        }

        public void ReportUnexpectedToken(TextSpan span, SyntaxKind actualKind, SyntaxKind expectedKind)
        {
            string message = $"Unexpected token <{actualKind}>, expected <{expectedKind}>";
            Report(span, message);
        }

        public void ReportInvalidInteger(TextSpan span, string? text)
        {
            string message = $"The value '{text}' is not a valid integer.";
            Report(span, message);
        }

        public void ReportUndefinedSymbol(TextSpan span, string name)
        {
            string message = $"The symbol '{name}' is not defined.";
            Report(span, message);
        }

        public void ReportLabelAlreadyDeclared(TextSpan span, string name)
        {
            string message = $"A label with the name '{name}' already exists.";
            Report(span, message);
        }

        public void ReportConstantAlreadyDeclared(TextSpan span, string name)
        {
            string message = $"A constant with the name '{name}' already exists.";
            Report(span, message);
        }

        public void ReportUndefinedCommand(TextSpan span, string command)
        {
            string message = $"The command '{command}' does not exist.";
            Report(span, message);
        }

        public void ReportBadDirective(TextSpan span, string name)
        {
            string message = $"Directive .'{name}' does not exist.";
            Report(span, message);
        }

        public void ReportMissingOperand(TextSpan span)
        {
            string message = $"Missing required operand on command or directive.";
            Report(span, message);
        }

        #endregion Report
    }
}