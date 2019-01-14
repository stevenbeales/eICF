using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Common;
using System.IO;
using MiCo.MiForms;

namespace eICFTests
{
    [TestClass]
    public class SharePointConnectionManagerTests
    {
        [TestInitialize]
        public void Setup()
        {
            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=";
            string localdb = Path.Combine(@"..\..\data\", "eICF.MDB");
            _access = new EIcfAccess(_formstub, connectionString);
            _dbc = _access.OpenLocalConnection(localdb);
            _localDatabase = new LocalDatabase(_dbc);
            _scm = new SharePointConnectionManager(_dbc, _provider);
        }

        [TestMethod]
        public void SharePointConnectionManagerCanConstruct()
        {
            Assert.IsNotNull(_scm);
        }

        private SharePointConnectionManager _scm;
        private SharePointContextProvider _provider = new SharePointContextProvider();
        private LocalDatabase _localDatabase;
        private EIcfAccess _access;
        private DbConnection _dbc;
        private readonly Form _formstub = new Form();
    }
}
