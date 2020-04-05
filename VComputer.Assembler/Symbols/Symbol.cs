namespace VComputer.Assembler.Symbols
{
    internal abstract class Symbol
    {
        protected Symbol(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public override string ToString() => Name;
    }
}