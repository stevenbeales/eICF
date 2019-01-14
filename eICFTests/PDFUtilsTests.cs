using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace eICFTests
{
    [TestClass]
    public class PdfUtilsTests
    {    
        [TestMethod]
        public void CanEncryptPdfFile()
        {
            string encryptedFile = @"C:\eicf\blankencrypt.pdf";
            File.Copy(@"c:\eicf\blank.pdf", encryptedFile, true);
            PdfUtils.EncryptPdfFile(encryptedFile);
            Assert.IsFalse(File.ReadAllText(encryptedFile).Contains("Informed"));
        }
    }
}
