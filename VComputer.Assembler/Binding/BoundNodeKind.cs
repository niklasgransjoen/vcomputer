namespace VComputer.Assembler.Binding
{
    internal enum BoundNodeKind
    {
        // Statements

        CommandStatement,
        DirectiveStatement,
        ConstantDeclarationStatement,
        LabelDeclarationStatement,

        // Expressions

        LiteralExpression,
        ConstantExpression,
        LabelExpression,
        ErrorExpression,
    }
}