using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace NesEmulator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
            string dir = Directory.GetCurrentDirectory();
            string nextLine = "";
            var fr = new FileReader();
            fr.OpenFile("testdata/exists.txt", FileReader.FileType.FILE_TEXT);
            while(!fr.IsError() && !fr.IsEOF())
            {
                nextLine = fr.ReadNextLine();
            }
        }
    }
}
