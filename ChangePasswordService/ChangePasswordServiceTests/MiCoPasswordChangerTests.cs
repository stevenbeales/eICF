using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChangePassword;
using System;

namespace ChangePasswordService.Tests
{
    [TestClass()]
    public class MiCoPasswordChangerTests
    {
        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PasswordChangeTest()
        {
            //This will throw an exception because we don't have endpoints in our unit test project
            MiCoPasswordChanger mpc = new MiCoPasswordChanger();
            mpc.Execute("CA.Stan", "Welcome!1");
        }
    }
}