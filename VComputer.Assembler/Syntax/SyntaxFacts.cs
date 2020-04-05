using System;

namespace VComputer.Assembler.Syntax
{
    public static class SyntaxFacts
    {
        public static bool TryGetKeywordKind(string word, out SyntaxKind syntaxKind)
        {
            switch (word)
            {
                case "def":
                    syntaxKind = SyntaxKind.DefKeyword;
                    return true;

                default:
                    syntaxKind = default;
                    return false;
            }
        }

       
    }
}