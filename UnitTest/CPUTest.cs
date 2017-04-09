using NesEmulator;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NesEmulator.Tests
{
    [TestClass()]
    public class CPUTestImmediateMode
    {
        private CPUHandler cpu = new CPUHandler();
        private MainMemory memory = new MainMemory();

        private const byte VALUE_MAX = 0xFF;  // 255
        private const byte VALUE_CARRY = 128; // 10000000

        [TestInitialize()]
        public void TestInitalize()
        {
            cpu = new CPUHandler();
            memory = new MainMemory();
            cpu.Init(memory);
        }

        [TestMethod()]
        public void EvalLDAXY()
        {
            const byte testValue = 0x56;    // 86
            Operation op = new Operation((byte)RicohCPU.OpCodeImmediate.LDA, testValue, 0, 0);
            cpu.EvalImmediate(op);
            Assert.AreEqual(testValue, cpu.GetRegA());

            op = new Operation((byte)RicohCPU.OpCodeImmediate.LDX, testValue, 0, 0);
            cpu.EvalImmediate(op);
            Assert.AreEqual(testValue, cpu.GetRegX());

            op = new Operation((byte)RicohCPU.OpCodeImmediate.LDY, testValue, 0, 0);
            cpu.EvalImmediate(op);
            Assert.AreEqual(testValue, cpu.GetRegY());
        }

        [TestMethod()]
        public void EvalADC()
        {
            const byte testValue = 5;
            Operation op = new Operation((byte)RicohCPU.OpCodeImmediate.ADC, testValue, 0, 0);

            Assert.AreEqual(0, cpu.GetRegA());
            cpu.EvalImmediate(op);
            Assert.AreEqual(testValue, cpu.GetRegA());
        }

        /*[TestMethod()] - TODO: not supported yet
        public void EvalADCMemory()
        {
            const byte testValue = 5;
            const byte testAddress = 0x0a;
            Operation op = new Operation((byte)RicohCPU.OpCodeImmediate.ADC, testAddress, 0, 0);

            Assert.AreEqual(0, cpu.GetRegA());
            memory.Write(testAddress, testValue);
            cpu.EvalImmediate(op);
            Assert.AreEqual(testValue, cpu.GetRegA());
        }*/

        [TestMethod()]
        public void EvalAND()
        {
            const byte testValue1 = 123;    // 01111011
            const byte testValue2 = 64;     // 01000000
            Operation op1 = new Operation((byte)RicohCPU.OpCodeImmediate.LDA, testValue1, 0, 0);
            Operation op2 = new Operation((byte)RicohCPU.OpCodeImmediate.AND, testValue2, 0, 0);

            Assert.AreEqual(0, cpu.GetRegA());
            cpu.EvalImmediate(op1);
            cpu.EvalImmediate(op2);
            Assert.AreEqual(testValue1 & testValue2, cpu.GetRegA());
        }

        [TestMethod()]
        public void EvalASL()
        {
            const byte testValue = 1;
            const byte testValueCarry = VALUE_CARRY;    // 10000000
            Operation op1 = new Operation((byte)RicohCPU.OpCodeImmediate.LDA, testValue, 0, 0);
            Operation op2 = new Operation((byte)RicohCPU.OpCodeImmediate.ASL, 0, 0, 0);
            Operation op3 = new Operation((byte)RicohCPU.OpCodeImmediate.LDA, testValueCarry, 0, 0);

            cpu.EvalImmediate(op3);
            cpu.EvalImmediate(op2);
            Assert.AreEqual(0, cpu.GetRegA());
            Assert.AreEqual((byte)RicohCPU.Flags.Carry, (byte)(cpu.GetFlagReg() & (byte)RicohCPU.Flags.Carry));

            Assert.AreEqual(0, cpu.GetRegA());
            cpu.EvalImmediate(op1);
            cpu.EvalImmediate(op2);
            Assert.AreEqual(testValue << 1, cpu.GetRegA());
            Assert.AreEqual(0, cpu.GetFlagReg() & (byte)RicohCPU.Flags.Carry);
        }

        [TestMethod()]
        public void EvalbitwiseOps()
        {
            const byte testValue1 = 123;    // 01111011
            const byte andValue = 87;       // 01010111
            const byte orValue = 131;       // 10000011
            const byte xorValue = 65;       // 01000001
            Operation op1 = new Operation((byte)RicohCPU.OpCodeImmediate.LDA, testValue1, 0, 0);
            Operation op2 = new Operation((byte)RicohCPU.OpCodeImmediate.AND, andValue, 0, 0);
            Operation op3 = new Operation((byte)RicohCPU.OpCodeImmediate.ORA, orValue, 0, 0);
            Operation op4 = new Operation((byte)RicohCPU.OpCodeImmediate.EOR, xorValue, 0, 0);

            cpu.EvalImmediate(op1);
            cpu.EvalImmediate(op2);
            Assert.AreEqual(testValue1 & andValue, cpu.GetRegA());

            cpu.EvalImmediate(op1);
            cpu.EvalImmediate(op3);
            Assert.AreEqual(testValue1 | orValue, cpu.GetRegA());

            cpu.EvalImmediate(op1);
            cpu.EvalImmediate(op4);
            Assert.AreEqual(testValue1 ^ xorValue, cpu.GetRegA());
        }

        [TestMethod()]
        public void EvalCMPXY()
        {
            CompareTest(RicohCPU.OpCodeImmediate.LDA, RicohCPU.OpCodeImmediate.CMP, cpu.GetRegA);
            CompareTest(RicohCPU.OpCodeImmediate.LDX, RicohCPU.OpCodeImmediate.CPX, cpu.GetRegX);
            CompareTest(RicohCPU.OpCodeImmediate.LDY, RicohCPU.OpCodeImmediate.CPY, cpu.GetRegY);
        }

        private void CompareTest(RicohCPU.OpCodeImmediate OpSetCode, RicohCPU.OpCodeImmediate OpCompareCode, CPUHandler.ReturnRegister comparitor )
        {
            const byte testValue = 1;
            const byte testValueMem = 1;
            const byte testValueMemGreater = 2;
            const byte testValueMemLower = 3;
            memory.Write(testValueMem, 1);
            memory.Write(testValueMemGreater, 2);
            memory.Write(testValueMemLower, 0);
            Operation op1 = new Operation((byte)OpSetCode, testValue, 0, 0);
            Operation op2 = new Operation((byte)OpCompareCode, testValueMem, 0, 0);
            Operation op3 = new Operation((byte)OpCompareCode, testValueMemGreater, 0, 0);
            Operation op4 = new Operation((byte)OpCompareCode, testValueMemLower, 0, 0);

            // zero
            cpu.EvalImmediate(op1);
            cpu.EvalImmediate(op2);
            Assert.AreEqual(testValue, comparitor());
            Assert.AreEqual((byte)RicohCPU.Flags.Zero, cpu.GetFlagReg() & (byte)RicohCPU.Flags.Zero);
            Assert.AreEqual(0, (byte)(cpu.GetFlagReg() & (byte)RicohCPU.Flags.Sign));

            // negative
            Assert.AreEqual(testValue, comparitor());
            cpu.EvalImmediate(op1);
            cpu.EvalImmediate(op3);
            Assert.AreEqual(0, cpu.GetFlagReg() & (byte)RicohCPU.Flags.Zero);
            Assert.AreEqual((byte)RicohCPU.Flags.Sign, cpu.GetFlagReg() & (byte)RicohCPU.Flags.Sign);

            // positive
            Assert.AreEqual(testValue, comparitor());
            cpu.EvalImmediate(op1);
            cpu.EvalImmediate(op4);
            Assert.AreEqual(0, cpu.GetFlagReg() & (byte)RicohCPU.Flags.Zero);
            Assert.AreEqual(0, cpu.GetFlagReg() & (byte)RicohCPU.Flags.Sign);
        }


    }
}