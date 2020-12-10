using VComputer.Util;

namespace VComputer.Components
{
    internal sealed class ProgramCounter : BaseComponent
    {
        internal ProgramCounter()
        {
        }

        public bool Enable { get; set; }
        public bool Input { get; set; }
        public bool Output { get; set; }
        public int Value { get; private set; }

        public override void Connect(ClockConnector clock)
        {
            base.Connect(clock);
            clock.AddAction(Increment, ClockPriorities.BeforeRead);
        }

        #region Callbacks

        protected override void Write()
        {
            if (!Output || Bus is null)
                return;

            MemoryUtil.FromInt(Value, Bus.Lines);
        }

        protected override void Read()
        {
            if (!Input || Bus is null)
                return;

            Value = MemoryUtil.ToInt(Bus.Lines);
        }

        private void Increment()
        {
            if (!Enable)
                return;

            Value++;
        }

        #endregion Callbacks
    }
}