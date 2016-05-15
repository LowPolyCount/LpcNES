using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NesEmulator;
using System.IO;
using System.Diagnostics;

namespace UnitTest
{
    [TestClass]
    public class FileReaderTest
    {
        public static string BASE_DIR = "../../../NesEmulator/testdata"; //@todo: Find out why we're not running in the correct directory

        [TestMethod]
        public void FileReaderOpenFileOk()
        {
            string dir = Directory.GetCurrentDirectory();
            Console.WriteLine(dir);
            Trace.Write(dir);
            var fr = new FileReader();
            fr.OpenFile(BASE_DIR+"/exists.txt", FileReader.FileType.FILE_TEXT);

            Assert.IsFalse(fr.IsError());
        }
        [TestMethod]
        public void FileReaderOpenFileError()
        {
            var fr = new FileReader();
            fr.OpenFile("Does Not Exist", FileReader.FileType.FILE_TEXT);

            Assert.IsTrue(fr.IsError());
        }
    }
}
