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

        public static string? GetText(SyntaxKind kind)
        {
            return kind switch
            {
                _ => null,
            };
        }
    }
}