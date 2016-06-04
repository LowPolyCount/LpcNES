using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NesEmulator;

namespace UnitTest
{
    class TestCPU : RicohCPU
    {
        public Int16 GetPc() { return m_pc; }     // program counter
        public Int16 GetStack() {return m_stack; }  // stack register  
        public Int16 GetFlagReg() {return m_flagReg; }    // flag register
        public Int16 GetRegA() { return m_regA; }       // accumulator
        public Int16 GetRegX() { return m_regX; }       // accumulator
        public Int16 GetRegY() { return m_regY; }       // accumulator
        public bool GetInterruptDisable() { return m_InterruptDisable; }
    }
}
