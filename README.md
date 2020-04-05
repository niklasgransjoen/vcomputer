# VComputer

Let's build a computer... with a computer.

Inspired by Ben Eater's series on building an 8-bit computer on breadboards, but without any of the commitment of actually buying breadboards and components. [Youtube playlist of his series](https://www.youtube.com/playlist?list=PLowKtXNTBypGqImE405J2565dvjafglHU).

The whole project is written in C#, and includes the computer logic itself, as well as an assembler, and a simple console interface. The computer is written in a way that attempts to simulate Eater's 8-bit computer as close as possible. This includes not using higher-level concepts where avoidable.

## Projects

### VComputer

The core computer definition.

#### Computer flags

This section contains details concerning the different computer control flags. Be aware that the value of the different flags are not part of the definition, and might change. It's therefore recommended to access the different flags by name, which will be constant.

**AI**

Registry A in.

**AO**

Registry A out.

**BI**

Registry B in.

**BO**

Registry B out.

**II**

Instruction registry in.

**IO**

Instruction registry out.

**RI**

RAM in.

**RO**

RAM out.

**MI**

RAM Controller (memory) in.

**LO**

ALU out.

**LM1**

ALU Mode bit 1.

**LM2**

ALU Mode bit 2.

**LM3**

ALU Mode bit 3.

**CE**

Counter enable.

**CI**

Counter in.

**CO**

Counter out.

**OI**

Output registry in.

### VComputer.Assembler

An general-purpose asssembler.

The assembler exposes only two classes:
- VComputer.Assembler.Assembler
- VComputer.Assembler.AssemblyInstruction

The Assembler class accepts a collection of assembly instructions in its constructor, meaning the assembler can be repurposed for any language definition.

The assembler currently provides support for the following functionality:
- Commands
- Labels
- Constants
- Assembly directives
- Macros

Supported assembly directives:
- .org: Set the location pointer of the assembler.
- .word: Write a literal to the current location, and advance the location pointer by one.

### VComputer.Interface

This library serves two purposes:
1. It defines the instructions of the computer.
2. It provides a simple console interface.

The project targets win-x86 due to the implementation of ITimer, which utilizes a native Windows API called 'winmm.dll'. By changing this implementation of ITimer to be platform independent, the project can be retargeted for any platform.

The motivations for this design was the inaccuracy of the System.Threading.Timers.
