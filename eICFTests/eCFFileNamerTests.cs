using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace eICFTests
{
    /// <summary>
    /// Summary description for eCFFileNamerTests
    /// </summary>
    [TestClass]
    public class ECfFileNamerTests
    {
       
        [TestInitialize]
        public void Setup()
        {
            _namer.BackupPath = @"c:\remotesource\source";
            _namer.BioMarker = "N";
            _namer.ConsentorSigDate = DateTime.Today.ToShortDateString();
            _namer.Language = "en";
            _namer.Pgx = "Y";
            _namer.SiteNumber = "50001";
            _namer.SubjectNumber = "001";
            _namer.SubjectSigDate = DateTime.Today.ToShortDateString();
            _namer.Templatename = "50001_01_NOV_2015";
            _namer.Version = "1";
            _namer.Visit = "1.0";

            _namerUn.BackupPath = @"c:\remotesource\source";
            _namerUn.BioMarker = "N";
            _namerUn.ConsentorSigDate = DateTime.Today.ToShortDateString();
            _namerUn.Language = "en";
            _namerUn.Pgx = "Y";
            _namerUn.SiteNumber = "50001";
            _namerUn.SubjectNumber = "001";
            _namerUn.SubjectSigDate = DateTime.Today.ToShortDateString();
            _namerUn.Templatename = "50001_01_NOV_2015";
            _namerUn.Version = "1";
            _namerUn.Visit = "";
        }

        [TestMethod]
        public void NamerCanConstruct()
        {
            Assert.IsNotNull(_namer);
        }
      
        [TestMethod]
        public void NamerPdfFileNameIsCorrect()
        {
            Assert.AreEqual(_namer.PdfFileName, Path.Combine(_namer.BackupPath, eICFGlobals.SPONSOR + eICFGlobals.DELIMITER + eICFGlobals.STUDY + eICFGlobals.DELIMITER + _namer.Templatename + eICFGlobals.DELIMITER + _namer.SiteNumber + eICFGlobals.DELIMITER + _namer.SubjectNumber + eICFGlobals.DELIMITER + _namer.Version + eICFGlobals.DELIMITER
            + _namer.SubjectSigDate + eICFGlobals.DELIMITER + _namer.ConsentorSigDate + eICFGlobals.DELIMITER + _namer.Pgx + eICFGlobals.DELIMITER + _namer.BioMarker + eICFGlobals.DELIMITER + _namer.Language + eICFGlobals.DELIMITER + _namer.Visit + ".pdf"));
        }

        [TestMethod]
        public void NamerPdfFileNameUnscheduledVisitIsCorrect()
        {
            Assert.AreEqual(_namerUn.PdfFileName, Path.Combine(_namerUn.BackupPath, eICFGlobals.SPONSOR + eICFGlobals.DELIMITER + eICFGlobals.STUDY + eICFGlobals.DELIMITER + _namerUn.Templatename + eICFGlobals.DELIMITER + _namerUn.SiteNumber + eICFGlobals.DELIMITER + _namerUn.SubjectNumber + eICFGlobals.DELIMITER + _namerUn.Version + eICFGlobals.DELIMITER
            + _namerUn.SubjectSigDate + eICFGlobals.DELIMITER + _namerUn.ConsentorSigDate + eICFGlobals.DELIMITER + _namerUn.Pgx + eICFGlobals.DELIMITER + _namerUn.BioMarker + eICFGlobals.DELIMITER + _namerUn.Language + eICFGlobals.DELIMITER + "UN" + ".pdf"));
        }


        private readonly EIcfFileNamer _namer = new EIcfFileNamer();
        private readonly EIcfFileNamer _namerUn = new EIcfFileNamer();

    }
}
