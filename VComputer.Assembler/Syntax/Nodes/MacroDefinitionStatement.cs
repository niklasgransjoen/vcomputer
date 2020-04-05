using System.Collections.Generic;
using System.Collections.Immutable;

namespace VComputer.Assembler.Syntax
{
    internal sealed class MacroDefinitionStatement : StatementSyntax
    {
        public MacroDefinitionStatement(
            SyntaxToken defKeyword,
            SyntaxToken identifier,
            ImmutableArray<SyntaxToken> parameters,
            SeparatedStatementCollection<StatementSyntax> statements)
        {
            DefKeyword = defKeyword;
            Identifier = identifier;
            Parameters = parameters;
            Statements = statements;
        }

        public override SyntaxKind Kind => SyntaxKind.MacroDefinitionStatement;

        public SyntaxToken DefKeyword { get; }
        public SyntaxToken Identifier { get; }
        public ImmutableArray<SyntaxToken> Parameters { get; }
        public SeparatedStatementCollection<StatementSyntax> Statements { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return DefKeyword;
            yield return Identifier;

            foreach (var parameter in Parameters)
            {
                yield return parameter;
            }

            foreach (var statement in Statements)
            {
                yield return statement;
            }
        }
    }
}