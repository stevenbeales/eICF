using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace eICFTests
{
    [TestClass]
    public class SharePointContextProviderTests
    {
        [TestInitialize]
        public void Setup()
        {
            _scp = new SharePointContextProvider();
        }

        [TestMethod]
        public void SharePointContextProviderCanConstruct()
        {
            Assert.IsNotNull(_scp);
        }

        SharePointContextProvider _scp;
    }
}
