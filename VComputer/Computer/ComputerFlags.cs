using System;

namespace VComputer
{
    [Flags]
    public enum ComputerFlags : ulong
    {
        // Registry

        AI = 0x01,
        AO = 0x02,
        BI = 0x04,
        BO = 0x08,
        II = 0x10,
        IO = 0x20,

        // RAM

        RI = 0x00_40,
        RO = 0x00_80,
        MI = 0x01_00,

        // ALU

        LO = 0x02_00,
        LM1 = 0x04_00,
        LM2 = 0x08_00,
        LM3 = 0x10_00,

        // Program counter

        CE = 0x20_00,
        CI = 0x40_00,
        CO = 0x80_00,

        // Other

        OI = 0x10_00_00,
        HLT = 0x20_00_00,
    }
}