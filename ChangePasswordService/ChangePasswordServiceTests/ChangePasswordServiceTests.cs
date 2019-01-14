using ChangePasswordServiceTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ChangePassword.Tests
{
    [TestClass()]
    public class ChangePasswordServiceTests
    {
        [TestInitialize()]
        public void Setup()
        {
            fakeUserNotifier = new FakeUserNotifier();
        }

        [TestMethod()]
        public void ChangePasswordServiceTest()
        {
            ChangePasswordService cps = new ChangePasswordService(fakeUserNotifier, DateTime.Now.AddHours(-24));
            Assert.IsNotNull(cps);
        }

        [TestMethod()]
        public void OnTimerTest()
        {
            ChangePasswordService cps = new ChangePasswordService(fakeUserNotifier, DateTime.Now.AddHours(-24));
            Assert.IsFalse(cps.BackgroundWorker.IsBusy);
        }

        private FakeUserNotifier fakeUserNotifier;
    }
}
