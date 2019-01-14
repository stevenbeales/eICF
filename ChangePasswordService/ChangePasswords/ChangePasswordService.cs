using ChangePasswords;
using System;
using System.ComponentModel;
using System.Configuration;

namespace ChangePassword {
    public class ChangePasswordService {
        public ChangePasswordService(IUpdateUserNotifier changedUsers, DateTime lastRun) {
            _LastRun = lastRun;
            usersWithChangedPasswords = changedUsers;

            InitializeComponents();
        }
        
        protected void InitializeComponents() {
            backgroundWorker.DoWork += new DoWorkEventHandler(OnDoWork);
            InitializeTimers();
        }

        private void InitializeTimers() {
            timer = new NoLockTimer(GetInterval());
            timer.Timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimer);
            timer.Start();
         }

        public void OnDoWork(object sender, DoWorkEventArgs args) {
            ChangeMicoPasswords(usersWithChangedPasswords);
            args.Result = true;
        }

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args) {
            // call mico web service to change password 
            backgroundWorker.RunWorkerAsync();
            Console.WriteLine(DateTime.Now);
        }

        public BackgroundWorker BackgroundWorker {
            get {
                return backgroundWorker;
            }
        }
        
        public void ChangeMicoPasswords(IUpdateUserNotifier usersWithChangedPasswords) {
         
            //if we're not busy with previous requests
                //Find users in Learn who have been updated since last run
            usersWithChangedPasswords.Execute(_LastRun);

            //Set Last Run till now for next timer iteration
            _LastRun = DateTime.Now;

            try {
                Console.WriteLine("Changed user count: " + usersWithChangedPasswords.ChangedUsers.Count.ToString());
                //for each changed user
                foreach (Credential credential in usersWithChangedPasswords.ChangedUsers) {
                    Console.WriteLine("Changing Password for Username " + credential.Username);
                    //Change their MiCo Password
                    try {
                        bool result = MiCoPasswordChange(credential);
                        System.Threading.Thread.Sleep(1000);

                        //if no error changing password
                        if (result) {
                            Console.WriteLine("Changed Password for " + credential.Username + " to " + credential.Password);
                        }
                        else {
                            Console.WriteLine(string.Format("Error changing Passwords for {0}", credential.Username + " " + credential.Password));
                        }
                    }
                    catch (Exception ex) {
                        Console.WriteLine("Error Changing MiCo Passwords for User ", credential.Username + " " + ex.Message);
                    }
                }
                Console.WriteLine(string.Format("Changed Passwords for {0} users", usersWithChangedPasswords.ChangedUsers.Count.ToString()));
                timer.Start();
            } catch (Exception ex) {
                Console.WriteLine("Error Changing MiCo Passwords ", ex.Message);
                timer.Start();
            }
        }

        private bool MiCoPasswordChange(Credential credential) {
            try {
                passwordChanger = new MiCoPasswordChanger();
                return passwordChanger.Execute(credential.Username, credential.Password);
            }
            catch (Exception ex) {
                Console.WriteLine("Error in MiCoPasswordChange:  ", ex.Message + " " + credential.Username);
                return false;
            }
        }

        private int GetInterval()
        {
            int interval;
            try
            {
                interval = Convert.ToInt32(ConfigurationManager.AppSettings["Interval"]);
            }
            catch
            { interval = 60000; }
            return interval;
        }

        private IUpdateUserNotifier usersWithChangedPasswords;
        private DateTime _LastRun = DateTime.Now;
        private MiCoPasswordChanger passwordChanger;
        private BackgroundWorker backgroundWorker = new BackgroundWorker();
        private NoLockTimer timer;
    }
}
