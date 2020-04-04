namespace VComputer.Assembler.Syntax
{
    public enum SyntaxKind
    {
        // Tokens

        BadToken,
        EndOfFileToken,
        IdentifierToken,
        IntegerToken,
        LabelToken,
        LabelDeclarationToken,
        WhitespaceToken,
        NewLineToken,
        LineCommentToken,

        // Compilation

        CompilationUnit,

        // Statements

        CommandStatement,
        LabelDeclarationStatement,
        ConstantDeclarationStatement,

        // Expressions

        LiteralExpression,
        NameExpression,
        LabelExpression,
        EqualsToken,
    }
}