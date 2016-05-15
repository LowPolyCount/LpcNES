using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NesEmulator
{
    /**
    * Emulates Main Memory
    */
    class MemoryMap
    {
        const uint kMemSize = 2048;  // in bytes

        byte[] m_mainMemory = new byte[kMemSize];

        public MemoryMap()
        {
            for(int i =0; i<m_mainMemory.Length; ++i)
            {
                m_mainMemory[i] = 0;
            }
        }

        public void Write(uint address, byte value)
        {
            if (CheckAddress(address))
            {
                m_mainMemory[address] = value;
            }
        }

        public byte Read(uint address)
        {
            if(CheckAddress(address))
            {
                return m_mainMemory[address];
            }

            return 0;
        }

        //@todo: change error to a throw
        private bool CheckAddress(uint address)
        {
            if (address > kMemSize)
            {
                Console.WriteLine("CheckAddress: Address out of boundary " + address);
                return false;
            }

            return true;
        }

    }
}
