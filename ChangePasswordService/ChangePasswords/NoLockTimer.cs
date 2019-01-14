using System;
using System.Configuration;
using System.Timers;

namespace ChangePasswords {
    public class NoLockTimer : IDisposable {
        private readonly Timer _timer;

        public Timer Timer {
            get {
                return _timer;
            }
        }

        public NoLockTimer(int interval) {
           
            _timer = new Timer { AutoReset = false, Interval = interval };
        }

        public void Start() {
            Timer.Start();
        }
    
        public void Dispose() {
            if (Timer != null) {
                Timer.Dispose();
            }
        }
    }
}
