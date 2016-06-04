using NesEmulator;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NesEmulator.Tests
{
    [TestClass()]
    public class CPUTest
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
        public void EvalADC()
        {
            const byte testValue = 5;
            const byte testAddress = 0x0a;
            Operation op = new Operation((ushort)RicohCPU.OpCodeImmediate.ADC, testAddress, 0, 0);

            Assert.AreEqual(cpu.GetRegA(), 0);
            memory.Write(testAddress, testValue);
            cpu.EvalImmediate(op);
            Assert.AreEqual(cpu.GetRegA(), testValue);
        }


    }
}