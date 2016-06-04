using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NesEmulator
{
    public class Operation
    {
        public ushort m_op = 0;
        public ushort m_arg1 = 0;
        public ushort m_arg2 = 0;
        public ushort m_arg3 = 0;

        public Operation(ushort op, ushort arg1, ushort arg2, ushort arg3)
        {
            m_op = op;
            m_arg1 = arg1;
            m_arg2 = arg2;
            m_arg3 = arg3;
        }
    }
}
