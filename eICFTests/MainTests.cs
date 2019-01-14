using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace eICFTests
{
    [TestClass]
    public class MainTests
    {
        [TestMethod]
        public void ZeroFill_PadsNumber_WithZeroes()
        {
            int i = 2;
            int pad = 3;
            Assert.AreEqual(Utils.ZeroFill(i, pad), "002");
        }

        [TestMethod]
        public void GetFlagConsentValueReturnsYWithBlank()
        {
            Assert.AreEqual(Utils.GetFlagConsentValue(""), "Y");
        }


        [TestMethod]
        public void GetFlagConsentValueReturnsNWithNotBlank()
        {
            Assert.AreEqual(Utils.GetFlagConsentValue(""), "Y");
        }

        [TestMethod]
        public void WritesMessage_ToTestLog()
        {
            TestLogger.Write("Test Message", "Test");
            string readText = File.ReadAllText(@"c:\eicf\tests.txt");
            Assert.IsTrue(readText.Contains("Test"));

        }

        [TestMethod]
        public void TestLogger_WritesMessage_ToTestLog()
        {
            TestLogger.Write("Test Message", "Error");
            using (var fileStream = new FileStream(@"c:\eicf\tests.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var textReader = new StreamReader(fileStream))
            {
                var content = textReader.ReadToEnd();
                Assert.IsTrue(content.Contains("Error"));
            }
        }

        [TestMethod]
        public void ErrorLogger_WritesMessage_ToErrorLog()
        {
            ErrorLog.Write("Error Message", "Error");
            using (var fileStream = new FileStream(@"c:\eicf\errlog.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var textReader = new StreamReader(fileStream))
            {
                var content = textReader.ReadToEnd();
                Assert.IsTrue(content.Contains("Error"));
            }
        }

        [TestMethod]
        public void ErrorLogger_WritesException_ToErrorLog()
        {
            try
            { 
                  throw new Exception();
            }
            catch (Exception e)
            {
                ErrorLog.Write("Error Message", e.StackTrace, e.Message);
            }

            using (var fileStream = new FileStream(@"c:\eicf\errlog.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var textReader = new StreamReader(fileStream))
            {
                var content = textReader.ReadToEnd();
                Assert.IsTrue(content.Contains("Error Message"));
            }

        }

        [TestMethod]
        public void IsValidSubjectNumber()
        {
            Assert.IsTrue(EIcfValidation.IsValidSubjectNumber("S0001021"));
        }

        [TestMethod]
        public void IsValidYear()
        {
            Assert.IsTrue(EIcfValidation.IsValidYear("1/01/2018"));
        }

        [TestMethod]
        public void IsNotValidYear()
        {
            Assert.IsFalse(EIcfValidation.IsValidYear("1/01/1018"));
        }

        [TestMethod]
        public void IsNotValidSubjectNumber()
        {
            Assert.IsFalse(EIcfValidation.IsValidSubjectNumber("021"));
        }

        [TestMethod]
        public void IsNotValidSubjectNumberEnding()
        {
            Assert.IsFalse(EIcfValidation.IsValidSubjectNumberEnding("000"));
        }

        [TestMethod]
        public void IsValidSubjectNumberEnding()
        {
            Assert.IsTrue(EIcfValidation.IsValidSubjectNumberEnding("001"));
        }

        [TestMethod]
        public void IsValidSubjectInitialsWithUnderscore()
        {
            Assert.IsTrue(EIcfValidation.IsValidSubjectInitials("A_A"));
        }
        
        [TestMethod]
        public void IsValidSubjectInitialsWithDash()
        {
            Assert.IsTrue(EIcfValidation.IsValidSubjectInitials("A-A"));
        }


        [TestMethod]
        public void IsNotValidSubjectInitials()
        {
            Assert.IsTrue(EIcfValidation.IsValidSubjectInitials("A A"));

        }

        [TestMethod]
        public void IsValidSubjectInitials()
        {
            Assert.IsTrue(EIcfValidation.IsValidSubjectInitials("AZA"));

        }

        [TestMethod]
        public void IsNotValidVisitNumber()
        {
            Assert.IsFalse(EIcfValidation.IsValidVisitNumber("00"));
        }

        [TestMethod]
        public void IsNotValidVisitNumberLength()
        {
            Assert.IsFalse(EIcfValidation.IsValidVisitNumber("100"));
        }


        [TestMethod]
        public void IsValidVisitNumber()
        {
            Assert.IsTrue(EIcfValidation.IsValidVisitNumber("01"));
        }
    }
}
