using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NesEmulator
{
    internal class MemoryLocation
    {
        public MemoryLocation(int inStart, int inLength) { m_start = inStart;  m_length = inLength; }
        public int m_start;
        public int m_length;
    }
    /**
    * Emulates Main Memory
    */
    public class MainMemory
    {
        public enum MemoryMap
        {
            InternalRam = 0,
            InternalRamMirror1,
            InternalRamMirror2,
            InternalRamMirror3,
            PPURegister,
            PPURegisterMirror,
            APUIO,
            Cartridge,
            MaxSize
        }

        private MemoryLocation[] m_memoryMapLocations = new MemoryLocation[(int)MemoryMap.MaxSize];
        const uint kMemSize = 65536;  // in bytes

        byte[] m_mainMemory = new byte[kMemSize];

        public MainMemory()
        {
            m_memoryMapLocations[(int)MemoryMap.InternalRam] = new MemoryLocation(0x0000, 0x0800);
            m_memoryMapLocations[(int)MemoryMap.InternalRamMirror1] = new MemoryLocation(0x0800, 0x0800);
            m_memoryMapLocations[(int)MemoryMap.InternalRamMirror2] = new MemoryLocation(0x1000, 0x0800);
            m_memoryMapLocations[(int)MemoryMap.InternalRamMirror3] = new MemoryLocation(0x1800, 0x0800);
            m_memoryMapLocations[(int)MemoryMap.PPURegister] = new MemoryLocation(0x2000, 0x0008);
            m_memoryMapLocations[(int)MemoryMap.PPURegisterMirror] = new MemoryLocation(0x2008, 0x1FF8);    // repeats every 8 bytes
            m_memoryMapLocations[(int)MemoryMap.APUIO] = new MemoryLocation(0x4000, 0x0020);
            m_memoryMapLocations[(int)MemoryMap.Cartridge] = new MemoryLocation(0x4020, 0xBFE0);    //Cartridge space: PRG ROM, PRG RAM, and mapper registers
 
        }

        public void Init()
        {
            for (int i = 0; i < m_mainMemory.Length; ++i)
            {
                m_mainMemory[i] = 0;
            }
        }

        public void Write(byte address, byte value)
        {
            if (CheckAddress(address))
            {
                m_mainMemory[address] = value;
            }
        }

        public byte Read(byte address)
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
