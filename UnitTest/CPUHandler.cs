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
        public delegate byte ReturnRegister();
        public byte GetPc() { return m_pc; }     // program counter
        public byte GetStack() { return m_stack; }  // stack register  
        public byte GetFlagReg() { return m_flagReg; }    // flag register
        public bool   GetFlagReg(RicohCPU.Flags flag) { return (((int)m_flagReg & (int)flag) != 0); }
        public byte GetRegA() { return m_regA; }       // accumulator
        public byte GetRegX() { return m_regX; }       // accumulator
        public byte GetRegY() { return m_regY; }       // accumulator
        //public bool GetInterruptDisable() { return m_InterruptDisable; }
    }
}
