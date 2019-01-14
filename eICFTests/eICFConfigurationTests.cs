using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiCo.MiForms;

namespace eICFTests
{
    [TestClass]
    public class eICFConfigurationTests
    {
        [TestInitialize]
        public void Setup()
        {
            _access = new EIcfAccess(_formstub, "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=");
            _config = new EIcfConfiguration(_formstub);
        }

        [TestMethod]
        public void ConfigurationCanConstruct()
        {
            Assert.IsNotNull(_config);
        }


        private EIcfConfiguration _config;
        private readonly Form _formstub = new Form();
        private EIcfAccess _access;

    }
}
