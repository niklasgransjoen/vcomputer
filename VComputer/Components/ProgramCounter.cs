using VComputer.Util;

namespace VComputer.Components
{
    public sealed class ProgramCounter : BaseComponent
    {
        private int _value;

        public ProgramCounter()
        {
        }

        public bool Enable { get; set; }
        public bool Input { get; set; }
        public bool Output { get; set; }

        public override void Connect(ClockConnector clock)
        {
            base.Connect(clock);
            clock.AddFallingEdgeAction(Increment, ClockPriorities.BeforeRead);
        }

        #region Callbacks

        protected override void Write()
        {
            if (!Output || Bus is null)
                return;

            MemoryUtil.FromInt(_value, Bus.Lines);
        }

        protected override void Read()
        {
            if (!Input || Bus is null)
                return;

            _value = MemoryUtil.ToInt(Bus.Lines);
        }

        private void Increment()
        {
            if (!Enable)
                return;

            _value++;
        }

        #endregion Callbacks
    }
}