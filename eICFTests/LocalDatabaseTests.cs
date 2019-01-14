using System;
using System.Data.Common;
using System.IO;
using MiCo.MiForms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace eICFTests
{
    [TestClass]
    public class LocalDatabaseTests
    {

        [TestInitialize]
        public void Setup()
        {
            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=";
            string localdb = Path.Combine(@"..\..\data\", "eICF.MDB");
            _access = new EIcfAccess(_formstub, connectionString);
            _dbc = _access.OpenLocalConnection(localdb);
            _localDatabase = new LocalDatabase(_dbc);
            _connectionManager = new ConnectionManager(_dbc, _provider);
            _site = new Site("SC.Samuel", _connectionManager);
            _user = new User("SC.Samuel", "Samuel", "SC", _connectionManager);
            _subject = new Subject(_connectionManager, "00001001", "AAA");
            _visit = new Visit(_subject, "eICF", "1", DateTime.Today, "01", "Completed");
        }

        [TestMethod]
        public void LocalDatabaseCanConstruct()
        {
            Assert.IsNotNull(_localDatabase);
        }

        [TestMethod]
        public void LocalDatabaseCanSaveSite()
        {
            string siteNumber = _site.Number; //because calling site.Number in SaveSiteInDatabase can cause infinite recursion
            Assert.IsTrue(_localDatabase.SaveSiteInDatabase(siteNumber, _site.PiFirstName, _site.PiLastName));
        }

        [TestMethod]
        public void LocalDatabaseCanSaveSiteUser()
        {
            Assert.IsTrue(_localDatabase.SaveSiteUserInDatabase(_site, _user));
        }

        [TestMethod]
        public void LocalDatabaseCanSaveSubject()
        {
            Assert.IsTrue(_localDatabase.SaveSubjectInDatabase(_subject, _site.Id));
        }

        [TestMethod]
        public void LocalDatabaseCanSaveSubjectVisit()
        {
            Assert.IsTrue(_localDatabase.SaveVisitInDatabase(_visit));
        }


        private ConnectionManager _connectionManager;
        private SharePointContextProvider _provider = new SharePointContextProvider();
        private LocalDatabase _localDatabase;
        private EIcfAccess _access;
        private DbConnection _dbc;
        private readonly Form _formstub = new Form();
        private Site _site;
        private User _user;
        private Subject _subject;
        private Visit _visit;
    }
}
