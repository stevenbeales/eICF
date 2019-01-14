using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Data.Common;
using MiCo.MiForms;

namespace eICFTests
{
    [TestClass]
    public class VisitTests
    {
        [TestInitialize]
        public void Setup() {
            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=";
            string localdb = Path.Combine(Path.GetTempPath(), "eICF.MDB");
            access = new EIcfAccess(formstub, connectionString);
            dbc = access.OpenLocalConnection(localdb);
            cm = new ConnectionManager(dbc, provider);
            user = new User("SC.Samuel", "Samuel", "SC", cm);
            subject = new Subject(cm, "50001001", "ABC");
            visit = new Visit(subject, "eICF", "1.0", DateTime.Today, "01", "");
        }


        [TestMethod]
        public void TestVisit()
        {
            Assert.IsNotNull(visit);
        }

        [TestMethod]
        public void VisitHasSubject()
        {
            Assert.IsNotNull(visit.Subject);
        }

        [TestMethod]
        public void VisitHasDate()
        {
            Assert.AreEqual(visit.VisitDate, DateTime.Today);
        }

        [TestMethod]
        public void VisitHasNumber()
        {
            Assert.AreEqual(visit.VisitNumber, "01");
        }

        [TestMethod]
        public void VisitHasForm()
        {
            Assert.AreEqual(visit.Form, "eICF");
        }

        [TestMethod]
        public void VisitHasFormVersion()
        {
            Assert.AreEqual(visit.VersionNumber, "1.0");
        }

        [TestMethod]
        public void VisitHasStatus()
        {
            Assert.AreEqual(visit.Status, "");
        }

        [TestMethod]
        public void CannotSaveVisit_WithNoSavedSubject()
        {
            Assert.IsFalse(visit.InsertInDatabase(dbc));
        }

        private Form formstub = new Form();
        private DbConnection dbc;
        private SharePointContextProvider provider = new SharePointContextProvider();
        private ConnectionManager cm;
        private User user;
        private EIcfAccess access;
        private Subject subject;
        private Visit visit;
    }
}
