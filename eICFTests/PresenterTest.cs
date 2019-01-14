using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiCo.MiForms;
using System.Globalization;

namespace eICFTests
{
    [TestClass]
    public class PresenterTest
    {
        [TestInitialize]
        public void Setup()
        {
            present = new Presenter(formstub);
        }

        [TestMethod]
        public void PresentCanConstruct()
        {
            Assert.IsNotNull(present);
        }

        [TestMethod]
        public void PresentDateFormatIsUS()
        {
            Assert.AreEqual(present.Fmt.ShortDatePattern, "MM/dd/yyyy");
        }

        [TestMethod]
        public void PresentTimeFormatIsUS()
        {
            Assert.AreEqual(present.Fmt.ShortTimePattern, "hh:mm");
        }

        private Form formstub = new Form();
        private Presenter present; 
    }
}
