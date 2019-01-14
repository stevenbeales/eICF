using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace eICFTests
{
    [TestClass]
    public class XMLValueStoreTests
    {

        [TestInitialize]
        public void Setup() {
            _store = new XmlValueStore(GetPrefsFile());
        }

        [TestMethod]
        public void XMLStoreCanConstruct()
        {
            Assert.IsNotNull(_store);
        }

        [TestMethod]
        public void GetPortNumber() {
           Assert.IsTrue(_store.GetNumber("Network_Port") > 0);
        }

        [TestMethod]
        public void GetPortNumber_WithInvalidKey()
        {
            Assert.IsTrue(_store.GetNumber("NetworkPort") == 0);
        }

        [TestMethod]
        public void GetBoolean_WithInvalidKey()
        {
            Assert.IsFalse(_store.GetBool("Invalid"));
        }

        [TestMethod]
        public void GetBoolean()
        {
            //If not using an SSL connection
            Assert.IsFalse(_store.GetBool("Network_UseHTTPS"));
        }

        [TestMethod]
        public void GetPreference_WithInvalidKey()
        {
            //If not using an SSL connection
            Assert.AreEqual("", _store.Get("NetworkUseHTTPS"));
        }

        [TestMethod]
        public void GetPreference()
        {
            //If not using an SSL connection
            Assert.AreNotEqual("", _store.Get("Network_UseHTTPS"));
        }

        private string GetPrefsFile() {
            try   {
                string prefsFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                prefsFile = Path.Combine(prefsFile, "Mi-Co");
                prefsFile = Path.Combine(prefsFile, "Mi-Forms");
                prefsFile = Path.Combine(prefsFile, "prefs.xml");
                if (prefsFile == "") {
                    ErrorLog.Write("Cannot find Mi-Co prefs file", "GetPrefsFile()");
                }
                return prefsFile;
            }
            catch(Exception ex)
            {
                ErrorLog.Write("GetPrefsfile Error", ex.StackTrace, ex.Message);
                return "";
            }
        }

        private XmlValueStore _store;
    }
}