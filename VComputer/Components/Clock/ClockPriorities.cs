namespace VComputer.Components
{
    public static class ClockPriorities
    {
        public const int BeforeWrite = 10_000;
        public const int Write = BeforeWrite - 1;
        public const int AfterWrite = Write - 1;

        public const int BeforeRead = 1;
        public const int Read = BeforeRead - 1;
        public const int AfterRead = Read - 1;
    }
}