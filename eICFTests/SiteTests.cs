using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiCo.MiForms;
using System.Data.Common;
using System.IO;

namespace eICFTests
{
    [TestClass]
    public class SiteTests
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
            _site = new Site("SC.Samuel", _cm);
        }

        [TestMethod]
        public void TestSite()
        {
            Assert.IsNotNull(_site);
        }

        [TestMethod]
        public void SiteHasPi()
        {
            Assert.AreEqual("PI Last Name.PI First Name", _site.Pi.ToString());
        }

        [TestMethod]
        public void SiteHasPiFullName()
        {
            Assert.AreEqual(_site.PiFullName, _site.Pi.FullName);
        }

        [TestMethod]
        public void SiteHasPiFirstName()
        {
            Assert.AreEqual("PI First Name", _site.PiFirstName);
        }

        [TestMethod]
        public void SiteHasPiLastName()
        {
            Assert.AreEqual("PI Last Name", _site.PiLastName);
        }

        [TestMethod]
        public void SiteHasSitePhone()
        {
            Assert.AreNotEqual("", _site.Phone);
        }

        [TestMethod]
        public void SiteHasSiteNumber()
        {
            Assert.AreEqual("50001", _site.Number);
        }

        [TestMethod]
        public void SiteHasUserName()
        {
            Assert.AreEqual("SC.Samuel", _site.UserName);
        }

        [TestMethod]
        public void SiteHasAddress()
        {
            Assert.AreEqual("PHL Medical Center Address1" + Environment.NewLine + "Plymouth Meeting, PA 12345", _site.Address);
        }

        [TestMethod]
        public void SiteHasTestSubjects()
        {
            Assert.IsTrue(_site.Subjects.Count == 0 || _site.Subjects.Count == 1);
        }

        [TestMethod]
        public void NextSubjectNumber()
        {
            Assert.AreEqual("002", _site.NextSubjectNumber());
        }

        [TestMethod]
        public void SiteSubjectExists()
        {
            Assert.IsTrue(_site.SubjectExists("00001001"));
        }

        [TestMethod]
        public void SiteSubjectDoesNotExist()
        {
            Assert.IsFalse(_site.SubjectExists("001"));
        }

        private Form _formstub;
        private DbConnection _dbc;
        private SharePointContextProvider _provider;
        private ConnectionManager _cm;
        private EIcfAccess _access;
        private Site _site;
    }
}
