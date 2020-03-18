using VComputer.Util;

namespace VComputer.Components
{
    public sealed class RAMController : BaseComponent
    {
        private readonly RAM _ram;

        public RAMController(RAM ram)
        {
            _ram = ram;
        }

        public bool Input { get; set; }

        protected override void Read()
        {
            if (!Input || Bus is null)
                return;

            _ram.Address = MemoryUtil.ToInt(Bus.Lines);
        }
    }
}