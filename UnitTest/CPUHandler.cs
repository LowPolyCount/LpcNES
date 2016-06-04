using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NesEmulator;

namespace NesEmulator.Tests
{
    class CPUHandler : RicohCPU
    {
        public ushort GetPc() { return m_pc; }     // program counter
        public ushort GetStack() { return m_stack; }  // stack register  
        public ushort GetFlagReg() { return m_flagReg; }    // flag register
        public ushort GetRegA() { return m_regA; }       // accumulator
        public ushort GetRegX() { return m_regX; }       // accumulator
        public ushort GetRegY() { return m_regY; }       // accumulator
        //public bool GetInterruptDisable() { return m_InterruptDisable; }
    }
}
