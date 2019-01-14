using ChangePasswords;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangePasswordServiceTests
{
    class FakeUserNotifier: BaseUserNotifier
    {
        protected override ArrayList GetChangedUsers(DateTime lastRun)
        {
            throw new NotImplementedException();
        }
    }

}
