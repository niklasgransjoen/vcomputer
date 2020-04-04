namespace VComputer.Assembler.Binding
{
    internal enum BoundNodeKind
    {
        // Statements

        CommandStatement,
        ConstantDeclarationStatement,
        LabelDeclarationStatement,

        // Expressions

        LiteralExpression,
        ConstantExpression,
        LabelExpression,
        ErrorExpression,
    }
}