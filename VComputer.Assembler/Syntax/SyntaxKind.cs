namespace VComputer.Assembler.Syntax
{
    public enum SyntaxKind
    {
        // Tokens

        BadToken,
        EndOfFileToken,
        CommandToken,
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
        OperatorStatement,
        LabelStatement,

        // Expressions

        LiteralExpression,
        LabelExpression,
    }
}