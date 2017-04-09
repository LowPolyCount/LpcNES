using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NesEmulator
{
    /*
     * http://www.6502.org/tutorials/vflag.html
     * http://www.righto.com/2012/12/the-6502-overflow-flag-explained.html
     * http://nesdev.com/6502.txt
     * https://wiki.nesdev.com/w/index.php/Tools
     * http://neshla.sourceforge.net/
    */


    public class RicohCPU
    {
        public enum OpCodeImmediate : byte
        {
            ADC = 0x69,
            AND = 0x29,
            ASL = 0x0A, //@todo, not handled in Immediate mode
            LDA = 0xA9,
            LDX = 0xA2,
            LDY = 0xA0,
            CMP = 0xC9,
            CPX = 0xE0,
            CPY = 0xC0,
            EOR = 0x49,
            ORA = 0x09,
            SBC = 0xe9
            // instructions to be implemented
            /*BCC,
            BCS,
            BEQ,
            BIT,
            BMI,
            BNE,
            BPL,
            BRK,
            BVC,
            BVS,
            CLC,
            CLD,
            CLI,
            CLV,
            DEC,
            DEX,
            DEY,
            INC,
            INX,
            INY,
            JMP,
            JSR,
            LSR,
            NOP,
            PHA,
            PHP,
            PLA,
            PLP,
            ROL,
            RTI,
            RTS,
            SEC,
            SED,
            SEI,
            STA,
            STX,
            STY,
            TAX,
            TAY,
            TSX,
            TXA,
            TXS,
            TYX*/
        }

        public enum Flags : byte
        {
            Carry = 1 << 0,
            Zero = 1 << 1,
            InterruptDisable = 1 << 2,
            Decimal = 1 << 3,
            Break = 1 << 4,
            AlwaysSet = 1 << 5,
            Overflow = 1 << 6,
            Sign = 1 << 7
        }

        public enum AddressingMode
        {
            Immediate
        }

        protected byte m_pc = 0;         // program counter
        protected byte m_stack = 0;      // stack register  
        protected byte m_flagReg = 0;    // flag register - Also called P
        protected byte m_regA = 0;       // accumulator
        protected byte m_regX = 0;       // Index Register X
        protected byte m_regY = 0;       // Index Register Y

        private AddressingMode m_mode = AddressingMode.Immediate;   // addressing mode
        private MainMemory m_memory = null;

        public RicohCPU()
        {

        }

        public void Init(MainMemory InMemory)
        {
            m_memory = InMemory;
            m_flagReg = 0x34;    // irq disabled
            m_stack = 0xfd;
            m_regA = m_regX = m_regY = 0;
        }

        public void Eval(Operation op)
        {
            switch(m_mode)
            {
                case AddressingMode.Immediate:
                    EvalImmediate(op);
                    break;
                default:
                    throw new System.InvalidOperationException("Addressing Mode not supported:"+m_mode);
            }
        }

        // immediate mode 
        public void EvalImmediate(Operation op) 
        {
            switch(op.m_op)
            {
                case (byte)OpCodeImmediate.ADC:
                    AddAccumulator(op.m_arg1);
                    break;
                case (byte)OpCodeImmediate.LDA:
                    LoadRegister(ref m_regA, op.m_arg1);
                    break;
                case (byte)OpCodeImmediate.LDX:
                    LoadRegister(ref m_regX, op.m_arg1);
                    break;
                case (byte)OpCodeImmediate.LDY:
                    LoadRegister(ref m_regY, op.m_arg1);
                    break;
                case (byte)OpCodeImmediate.ASL:   //@todo: not supported in immediate mode
                    LeftShiftAccumulator();
                    break;
                case (byte)OpCodeImmediate.CMP:
                    CompareSource(m_regA, m_memory.Read(op.m_arg1));
                    break;
                case (byte)OpCodeImmediate.CPX:
                    CompareSource(m_regX, m_memory.Read(op.m_arg1));
                    break;
                case (byte)OpCodeImmediate.CPY:
                    CompareSource(m_regY, m_memory.Read(op.m_arg1));
                    break;
                case (byte)OpCodeImmediate.AND:
                    AndAccumulator(op.m_arg1);
                    break;
                case (byte)OpCodeImmediate.EOR:
                    XORAccumulator(op.m_arg1);
                    break;
                case (byte)OpCodeImmediate.ORA:
                    ORAccumulator(op.m_arg1);
                    break;
                case (byte)OpCodeImmediate.SBC:
                    SubtractWithCarry(op.m_arg1);
                    break;
                default:
                    throw new System.InvalidOperationException("In Mode "+m_mode+" OpCode not supported: " + op);
            }
        }

        private void SetFlag(Flags flag, bool value)
        {
            if(value)
            {
                m_flagReg = (byte)((int)m_flagReg | (int)flag);
            }
            else
            {
                m_flagReg = (byte)((int)m_flagReg & (int)~flag);
            }
        }

        private bool GetFlag(Flags flag)
        {
            return ((int)m_flagReg & (int)flag) == 1;
        }


        private void SetCarry(bool isCarry)
        {
            byte value = Convert.ToByte(isCarry);
            m_flagReg = (byte)(m_flagReg ^ (-value ^ m_flagReg) & (byte)Flags.Carry);
        }

        private void SetSign(bool value)
        {
            int intValue = Convert.ToInt32(value);
            m_flagReg = (byte)(m_flagReg ^ (-intValue ^ m_flagReg) & (byte)Flags.Sign);
        }

        private void SetZero(bool value)
        {
            int intValue = Convert.ToInt32(value);
            m_flagReg = (byte)(m_flagReg ^ (-intValue ^ m_flagReg) & (byte)Flags.Zero);
        }

        private void SetFlagsCompare(int comResult)
        {
            SetCarry(comResult > byte.MaxValue);
            SetSign(comResult < 0);
            SetZero(comResult == 0);
        }

        /**
         * op code implementations
         */

        // for CMP, CMX, CMY, etc. Send in the source, value is taken from memory
        private void CompareSource(byte source, byte value)
        {
            int comResult = Convert.ToInt32(source - value);
            SetFlagsCompare(comResult);
        }

        private void AddAccumulator(byte value)
        {
            int addValue = Convert.ToInt32(value + m_regA);
            m_regA += (byte)addValue;
            SetFlagsCompare(addValue);
        }

        private void LoadRegister(ref byte register, byte value)
        {
            register = value;
        }

        private void AndAccumulator(byte value)
        {
            // need to handle carry and overflow
            m_regA = (byte)(m_regA & value);
        }

        private void LeftShiftAccumulator()
        {
            int result = Convert.ToInt32(m_regA << 1);
            SetCarry(result > byte.MaxValue);
            m_regA = (byte)result;
        }

        private void XORAccumulator(byte value)
        {
            m_regA ^= value;
        }

        private void ORAccumulator(byte value)
        {
            m_regA |= value;
        }

        private void SubtractWithCarry(byte value)
        {
            SetCarry(true);
            int subValue = Convert.ToInt32(value - m_regA);
            m_regA -= value;
            // handle carry and overflow
            SetFlagsCompare(subValue);
        }
    }
}
