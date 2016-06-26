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

        [TestInitialize()]
        public void TestInitalize()
        {
            cpu = new CPUHandler();
            memory = new MainMemory();
            cpu.Init(memory);
        }

        [TestMethod()]
        public void EvalLDA()
        {
            const byte testValue = 0x56;
            Operation op = new Operation((byte)RicohCPU.OpCodeImmediate.LDA, testValue, 0, 0);

            Assert.AreEqual(0, cpu.GetRegA());
            cpu.EvalImmediate(op);
            Assert.AreEqual(testValue, cpu.GetRegA());
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
            Operation op1 = new Operation((byte)RicohCPU.OpCodeImmediate.LDA, testValue, 0, 0);
            Operation op2 = new Operation((byte)RicohCPU.OpCodeImmediate.ASL, 0, 0, 0);

            Assert.AreEqual(0, cpu.GetRegA());
            cpu.EvalImmediate(op1);
            cpu.EvalImmediate(op2);
            Assert.AreEqual(testValue << 1, cpu.GetRegA());
        }

        [TestMethod()]
        public void EvalCMP()
        {
            const byte testValue = 1;
            const byte testValueMem = 1;
            const byte testValueMemGreater = 2;
            const byte testValueMemLower = 3;
            memory.Write(testValueMem, 1);
            memory.Write(testValueMemGreater, 2);
            memory.Write(testValueMemLower, 0);
            Operation op1 = new Operation((byte)RicohCPU.OpCodeImmediate.LDA, testValue, 0, 0);
            Operation op2 = new Operation((byte)RicohCPU.OpCodeImmediate.CMP, testValueMem, 0, 0);
            Operation op3 = new Operation((byte)RicohCPU.OpCodeImmediate.CMP, testValueMemGreater, 0, 0);
            Operation op4 = new Operation((byte)RicohCPU.OpCodeImmediate.CMP, testValueMemLower, 0, 0);            

            // zero
            cpu.EvalImmediate(op1);
            cpu.EvalImmediate(op2);
            Assert.AreEqual(testValue, cpu.GetRegA());
            Assert.AreEqual((byte)RicohCPU.Flags.Zero, cpu.GetFlagReg() & (byte)RicohCPU.Flags.Zero);
            Assert.AreEqual(0, (byte)(cpu.GetFlagReg() & (byte)RicohCPU.Flags.Sign));

            // negative
            Assert.AreEqual(testValue, cpu.GetRegA());
            cpu.EvalImmediate(op1);
            cpu.EvalImmediate(op3);
            Assert.AreEqual(0, cpu.GetFlagReg() & (byte)RicohCPU.Flags.Zero);
            Assert.AreEqual((byte)RicohCPU.Flags.Sign, cpu.GetFlagReg() & (byte)RicohCPU.Flags.Sign);

            // positive
            Assert.AreEqual(testValue, cpu.GetRegA());
            cpu.EvalImmediate(op1);
            cpu.EvalImmediate(op4);
            Assert.AreEqual(0, cpu.GetFlagReg() & (byte)RicohCPU.Flags.Zero);
            Assert.AreEqual(0, cpu.GetFlagReg() & (byte)RicohCPU.Flags.Sign);
        }


    }
}