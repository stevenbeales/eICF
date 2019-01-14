using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace eICFTests
{
    [TestClass]
    public class EncrypterTests
    {
        [TestMethod]
        public void CanEncryptAndDecrypt()
        {
            Assert.AreEqual("DaffyDuck's Legs", Encrypter.ToInsecureString(Encrypter.DecryptString(Encrypter.EncryptString(Encrypter.ToSecureString("DaffyDuck's Legs")))));
        }

        [TestMethod]
        public void CanEncrypt()
        {
            Assert.AreNotEqual("DaffyDuck'sLegs", Encrypter.ToInsecureString(Encrypter.DecryptString(Encrypter.EncryptString(Encrypter.ToSecureString("DaffyDuck's Legs")))));
        }

    }
}
