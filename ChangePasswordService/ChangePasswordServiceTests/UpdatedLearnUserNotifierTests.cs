using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ChangePassword;

namespace ChangePasswordService.Tests
{
    [TestClass()]
    public class UpdatedLearnUserNotifierTests
    {

        [TestMethod()]
        public void ExecuteTest()
        {
            string connectionString = @"Server = PRODSQL13\PRODSQL13; Database = uat_cTPLearn_01; Trusted_Connection = True;";
            DateTime lastRun = new DateTime(2015, 10, 1);
            UpdatedLearnUserNotifier notifier = new UpdatedLearnUserNotifier(connectionString);
            notifier.Execute(lastRun);

            System.Diagnostics.Trace.WriteLine("Changed Users: " + notifier.ChangedUsers.Count.ToString());
            Assert.IsTrue(notifier.ChangedUsers.Count > 0);

        }
    }
}