namespace VComputer.Assembler.Symbols
{
    internal sealed class MacroSymbol : Symbol
    {
        public MacroSymbol(string name, int parameterCount) : base(name)
        {
            ParameterCount = parameterCount;
        }

        public int ParameterCount { get; }
    }
}