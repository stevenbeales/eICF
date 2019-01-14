using System;
using System.Collections;

namespace ChangePassword {
    public interface IUpdateUserNotifier {
        void Execute(DateTime lastRun);
        ArrayList ChangedUsers { get; }
    }
}