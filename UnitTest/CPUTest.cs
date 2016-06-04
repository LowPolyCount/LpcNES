using NesEmulator;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NesEmulator.Tests
{
    [TestClass()]
    public class CPUTest
    {
        private RicohCPU cpu = new RicohCPU();
        [TestMethod()]
        public void EvalOpCodeTest()
        {
            cpu.EvalImmediate(RicohCPU.OpCode.ADC, 0, 0, 0);
            Assert.Fail();
        }
    }
}