namespace VComputer.Assembler.Binding
{
    internal enum BoundNodeKind
    {
        // Statements

        CommandStatement,
        MacroStatement,
        DirectiveStatement,
        MacroDeclarationStatement,
        ConstantDeclarationStatement,
        LabelDeclarationStatement,

        // Expressions

        LiteralExpression,
        SymbolExpression,
        ErrorExpression,
    }
}