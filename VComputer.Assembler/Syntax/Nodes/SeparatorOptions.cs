using System;

namespace VComputer.Assembler.Syntax
{
    [Flags]
    internal enum SeparatorOptions
    {
        None = 0,
        HasLeadingSeparator = 1,
        HasTrailingSeparator = 2,
        HasLeadingAndTrailingSeparator = HasLeadingSeparator | HasTrailingSeparator
    }
}