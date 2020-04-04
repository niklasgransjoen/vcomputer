using System;

namespace VComputer.Assembler.Syntax
{
    public static class SyntaxFacts
    {
        public static SyntaxKind GetKeywordKind(string word)
        {
            return word switch
            {
                _ => SyntaxKind.IdentifierToken,
            };
        }

        public static bool TryGetText(SyntaxKind kind, out ReadOnlyMemory<char> text)
        {
            text = kind switch
            {
                _ => default,
            };
            return !text.IsEmpty;
        }

        public static ReadOnlyMemory<char> GetDefaultTokenText(SyntaxKind kind) => (kind switch
        {
            SyntaxKind.EndOfFileToken => "\0",
            SyntaxKind.IdentifierToken => string.Empty,
            SyntaxKind.IntegerToken => "0",
            SyntaxKind.LabelToken => string.Empty,
            SyntaxKind.LabelDeclarationToken => ":",
            SyntaxKind.WhitespaceToken => " ",
            SyntaxKind.NewLineToken => "\n",
            SyntaxKind.LineCommentToken => "#",

            _ => default,
        }).AsMemory();

        public static object? GetDefaultTokenValue(SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.IntegerToken => "0",
                SyntaxKind.LabelToken => string.Empty,
                SyntaxKind.LabelDeclarationToken => ":",

                _ => null,
            };
        }
    }
}