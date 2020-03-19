namespace VComputer.Assembler
{
    public sealed class AssemblyInstruction
    {
        public AssemblyInstruction(string command, int opCode)
        {
            Command = command;
            OpCode = opCode;
        }

        public string Command { get; }
        public int OpCode { get; }
    }
}