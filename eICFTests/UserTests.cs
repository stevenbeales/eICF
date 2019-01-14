using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Common;
using System.IO;
using MiCo.MiForms;

namespace eICFTests
{
    [TestClass]
    public class UserTests
    {
        [TestInitialize]
        public void Setup()
        {
            _formstub = new Form();
            _provider = new SharePointContextProvider();
            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=";
            string localdb = Path.Combine(@"..\..\data\", "eICF.MDB");
            _access = new EIcfAccess(_formstub, connectionString);
            _dbc = _access.OpenLocalConnection(localdb);
            _cm = new ConnectionManager(_dbc, _provider);
            _user = new User("SC.Samuel", "Samuel", "SC", _cm);
        }

        [TestMethod]
        public void TestUser()
        {
            Assert.IsNotNull(_user);
        }

        [TestMethod]
        public void UserHasPhone()
        {
            Assert.IsNotNull(_user.Phone);
        }

        [TestMethod]
        public void UserHasFormTemplates()
        {
            Assert.IsNotNull(_user.FormTemplates);
        }

        private Form _formstub;
        private DbConnection _dbc;
        private SharePointContextProvider _provider;
        private ConnectionManager _cm;
        private User _user;
        private EIcfAccess _access;
    }
}
