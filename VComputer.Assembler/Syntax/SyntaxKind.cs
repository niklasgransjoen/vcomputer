namespace VComputer.Assembler.Syntax
{
    public enum SyntaxKind
    {
        // Tokens

        BadToken,
        EndOfFileToken,
        IdentifierToken,
        DirectiveToken,
        LabelToken,
        LabelDeclarationToken,
        IntegerToken,
        EqualsToken,
        WhitespaceToken,
        NewLineToken,
        LineCommentToken,

        // Compilation

        CompilationUnit,

        // Statements

        CommandStatement,
        DirectiveStatement,
        LabelDeclarationStatement,
        ConstantDeclarationStatement,

        // Expressions

        LiteralExpression,
        NameExpression,
        LabelExpression,
    }
}