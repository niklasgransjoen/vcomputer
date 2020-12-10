using System.Diagnostics.CodeAnalysis;

namespace VComputer.Assembler.Syntax
{
    internal enum SyntaxKind
    {
        // Tokens

        BadToken,
        EndOfFileToken,
        IdentifierToken,
        DirectiveToken,
        IntegerToken,
        EqualsToken,
        ColonToken,
        SlashSlashToken,
        WhitespaceToken,
        NewLineToken,
        LineCommentToken,

        // Keywords

        DefKeyword,

        // Compilation

        CompilationUnit,

        // Statements

        CommandStatement,
        MacroStatement,
        DirectiveStatement,
        MacroDefinitionStatement,
        LabelDeclarationStatement,
        ConstantDeclarationStatement,

        // Expressions

        LiteralExpression,
        NameExpression,
    }

    internal static class SyntaxKindExtensions
    {
        public static bool TryGetText(this SyntaxKind kind, [NotNullWhen(true)] out string? text)
        {
            text = kind switch
            {
                SyntaxKind.DefKeyword => "def",
                _ => null,
            };
            return text != null;
        }

        public static string GetDefaultTokenText(this SyntaxKind kind) => kind switch
        {
            SyntaxKind.EndOfFileToken => "\0",
            SyntaxKind.IdentifierToken => string.Empty,
            SyntaxKind.DirectiveToken => ".",
            SyntaxKind.IntegerToken => "0",
            SyntaxKind.EqualsToken => "=",
            SyntaxKind.ColonToken => ":",
            SyntaxKind.SlashSlashToken => "//",
            SyntaxKind.WhitespaceToken => " ",
            SyntaxKind.NewLineToken => "\n",
            SyntaxKind.LineCommentToken => "#",

            _ => string.Empty,
        };

        public static object? GetDefaultTokenValue(this SyntaxKind kind) => kind switch
        {
            SyntaxKind.IntegerToken => "0",
            _ => null,
        };

        public static bool IsExpression(this SyntaxKind kind) => kind switch
        {
            SyntaxKind.LiteralExpression => true,
            SyntaxKind.NameExpression => true,

            _ => false,
        };
    }
}