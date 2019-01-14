using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiCo.MiForms;
using System.Data.Common;
using System.IO;
using System.Data;

namespace eICFTests
{
    [TestClass]
    public class SubjectTests
    {

        [TestInitialize]
        public void Setup()
        {
            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=";
            string localdb = Path.Combine(Path.GetTempPath(), "eICF.MDB");
            _access = new EIcfAccess(_formstub, connectionString);
            _dbc = _access.OpenLocalConnection(localdb);
            _cm = new ConnectionManager(_dbc, _provider);
            _subject = new Subject(_cm, "50001001", "ABC");
        }

        [TestMethod]
        public void ValidSubject()
        {
            Assert.IsNotNull(_subject);
        }

        [TestMethod]
        [ExpectedException(typeof(ConstraintException))]
        public void InvalidSubjectNumber()
        {
            Subject subject2 = new Subject(_cm, "001", "DEF");
        }

        [TestMethod]
        public void SubjectVisitDoesNotExist()
        {
            Assert.IsFalse(_subject.VisitExists("01"));
        }

        [TestMethod]
        public void SubjectVisitExists()
        {
            Visit visit = new Visit(_subject, "eICF", "1.0", DateTime.Now, "01", "");
            _subject.Visits.Add(visit);
            Assert.IsTrue(_subject.VisitExists("01"));
        }

        [TestMethod]
        public void SubjectUnscheduledVisitDoesNotExist()
        {
            Visit visit = new Visit(_subject, "eICF", "1.0", DateTime.Now, "", "");
            _subject.Visits.Add(visit);
            Assert.IsFalse(_subject.VisitExists(""));
        }

        [TestMethod]
        public void SubjectFormExists()
        {
            Visit visit = new Visit(_subject, "eICF", "1.0", DateTime.Now, "01", "");
            _subject.Visits.Add(visit);
            Assert.IsTrue(_subject.IsFormInVisits("eICF"));
        }

        [TestMethod]
        public void SubjectFormDoesNotExist()
        {
            Assert.IsFalse(_subject.IsFormInVisits("eICF2"));
        }

        private readonly Form _formstub = new Form();
        private DbConnection _dbc;
        private readonly SharePointContextProvider _provider = new SharePointContextProvider();
        private ConnectionManager _cm;
        private EIcfAccess _access;
        private Subject _subject;
    }
}
