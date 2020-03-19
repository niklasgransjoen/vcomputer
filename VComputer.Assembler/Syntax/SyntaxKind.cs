namespace VComputer.Assembler.Syntax
{
    public enum SyntaxKind
    {
        // Tokens

        BadToken,
        EndOfFileToken,
        CommandToken,
        IntegerToken,
        WhitespaceToken,
        NewLineToken,
        //LineCommentToken,

        // Compilation

        CompilationUnit,

        // Statements

        OperatorStatement,

        // Expressions

        OperandExpression,
    }
}