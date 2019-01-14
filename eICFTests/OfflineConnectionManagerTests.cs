using MiCo.MiForms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Common;
using System.IO;

namespace eICFTests
{
    /// <summary>
    /// Summary description for OfflineConnectionManagerTests
    /// </summary>
    [TestClass]
    public class OfflineConnectionManagerTests
    {
        [TestInitialize]
        public void Setup()
        {
            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=";
            string localdb = Path.Combine(@"..\..\data\", "eICF.MDB");
            _access = new EIcfAccess(_formstub, connectionString);
            _dbc = _access.OpenLocalConnection(localdb);
            _localDatabase = new LocalDatabase(_dbc);
            _ocm = new OfflineConnectionManager(_dbc, _provider);
        }

        [TestMethod]
        public void OfflineConnectionManagerCanConstruct()
        {
            Assert.IsNotNull(_ocm);
        }

        private OfflineConnectionManager _ocm;
        private SharePointContextProvider _provider = new SharePointContextProvider();
        private LocalDatabase _localDatabase;
        private EIcfAccess _access;
        private DbConnection _dbc;
        private readonly Form _formstub = new Form();

    }
}
