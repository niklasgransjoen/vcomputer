using System;
using VComputer.Components;
using VComputer.Util;

namespace VComputer.Interface.Components
{
    internal sealed class RAMInitializer : IRAMInitializer
    {
        private readonly ReadOnlyMemory<int> _memory;

        public RAMInitializer(ReadOnlyMemory<int> memory)
        {
            _memory = memory;
        }

        public void InitializeRAM(ReadOnlySpan<bool[]> memory)
        {
            ReadOnlySpan<int> memorySpan = _memory.Span;
            for (int i = 0; i < _memory.Length; i++)
            {
                MemoryUtil.FromInt(memorySpan[i], memory[i]);
            }
        }
    }
}