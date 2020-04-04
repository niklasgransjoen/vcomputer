namespace VComputer.Assembler.Binding
{
    internal enum BoundNodeKind
    {
        // Statements

        CommandStatement,
        LabelStatement,

        // Expressions

        LiteralExpression,
        LabelExpression,
        ErrorExpression,
    }
}