using System;

namespace VComputer.Assembler.Syntax
{
    public static class SyntaxFacts
    {
        public static SyntaxKind GetKeywordKind(string word)
        {
            return word switch
            {
                _ => SyntaxKind.CommandToken,
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
    }
}