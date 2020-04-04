namespace VComputer.Assembler.Binding
{
    internal sealed class ConstantSymbol
    {
        public ConstantSymbol(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public override string ToString() => Name;
    }
}