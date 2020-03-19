# VComputer

Let's build a computer... with a computer.

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

### VComputer.Interface

This library serves two purposes:
1. It defines the instructions of the computer.
2. It provides a simple console interface.

The project targets win-x86 due to the implementation of ITimer, which utilizes a native Windows API called 'winmm.dll'. By changing this implementation of ITimer to be platform independent, the project can be retargeted for any platform.

The motivations for this design was the inaccuracy of the System.Threading.Timers.