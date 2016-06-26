using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NesEmulator
{
    public class Operation
    {
        public byte m_op = 0;
        public byte m_arg1 = 0;
        public byte m_arg2 = 0;
        public byte m_arg3 = 0;

        public Operation(byte op, byte arg1, byte arg2, byte arg3)
        {
            m_op = op;
            m_arg1 = arg1;
            m_arg2 = arg2;
            m_arg3 = arg3;
        }
    }
}
