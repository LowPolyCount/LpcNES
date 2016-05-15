using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NesEmulator
{
    public class FileReader
    {
        private FileType m_fileType = FileType.FILE_TEXT;
        private StreamReader m_fileRead = null;
        private bool m_error = false;

        public enum FileType
        {
            FILE_TEXT,
            FILE_NES
        }

        public FileReader()
        {

        }

        public bool IsError() {return m_error; }

        public bool IsEOF() { return m_fileRead.EndOfStream; }

        public string ReadNextLine()
        {
            return "";
        }

        public bool OpenFile(string filename, FileType type)
        {
            m_fileType = type;

            switch(m_fileType)
            {
                case FileType.FILE_TEXT:
                    break;
                case FileType.FILE_NES:
                    break;
                default:
                    m_error = true;
                    Console.WriteLine("OpenFile Unknown type: " + m_fileType);
                    return false;
            }

            try
            {
                m_fileRead = new StreamReader(filename, true);
            }
            catch(IOException ex)
            {
                Console.WriteLine(ex.GetType().FullName);
                Console.WriteLine(ex.Message);
                m_error = true;
                return false;
            }
            
            return true;
        }
    }
}
