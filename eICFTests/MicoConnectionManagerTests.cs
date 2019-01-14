using System;
using System.Data.Common;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiCo.MiForms;

namespace eICFTests
{
    [TestClass]
    public class MicoConnectionManagerTests
    {
        [TestInitialize]
        public void Setup()
        {
            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=";
            string localdb = Path.Combine(@"..\..\data\", "eICF.MDB");
            _access = new EIcfAccess(_formstub, connectionString);
            _dbc = _access.OpenLocalConnection(localdb);
            _localDatabase = new LocalDatabase(_dbc);
            _mcm = new MiCoConnectionManager(_dbc, _provider, _credentials);
        }

        [TestMethod]
        public void MicoConnectionManagerCanConstruct()
        {
            Assert.IsNotNull(_mcm);
        }

        private MiCoConnectionManager _mcm;
        private SharePointContextProvider _provider = new SharePointContextProvider();
        private LocalDatabase _localDatabase;
        private EIcfAccess _access;
        private DbConnection _dbc;
        private readonly Form _formstub = new Form();
        private Credentials _credentials = new Credentials();
    }
}
