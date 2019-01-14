using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Common;
using System.IO;
using MiCo.MiForms;

namespace eICFTests
{
    [TestClass]
    public class EicfAccessTests
    {
        [TestInitialize]
        public void Setup() {
            _formstub = new Form();
            _access = new EIcfAccess(_formstub, "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=");
        }

        [TestMethod]
        public void AccessCanConstruct()
        {
            Assert.IsNotNull(_access);
        }

        [TestMethod]
        public void OpenLocalConnection()  {
            string localDb = Path.Combine(@"..\..\data\" , "eICF.MDB");
            DbConnection dbc = _access.OpenLocalConnection(localDb);
            Assert.IsNotNull(dbc);          
        }

        [TestMethod]
        public void GetLocalDatabase_ReturnsFile()
        {
            string localDb = Path.Combine(Path.GetTempPath(), "eICF.MDB");
            Assert.AreNotEqual("", _access.GetLocalDatabase(localDb));
        }

        [TestMethod]
        public void GetLocalDatabaseAttachmentName_WithoutAttachment()
        {
            Assert.AreEqual("", _access.GetLocalDatabaseAttachmentName());
        }

        private Form _formstub;
        private EIcfAccess _access;
    }
}
