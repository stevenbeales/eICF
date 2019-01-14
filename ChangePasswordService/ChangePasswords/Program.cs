using System;
using System.Configuration;

namespace ChangePassword {
    /// <summary>
    /// Program to find all recently changed users in Learn and change their MiCo passwords.
    /// </summary>
    public static class Program
    {
        static void Main(string[] args) {   
            int hours = 24;

            if (args.Length > 0) {
                foreach (string arg in args)  {
                    try {
                        //see if another time period in hours was sent on command line
                        hours = Convert.ToInt32(arg);
                    }
                    catch {
                        hours = 24;
                    }
                }
            }
            //Get users from LMS

            string learnconnectionString = ConfigurationManager.ConnectionStrings["LearnConnection"].ConnectionString;
            IUpdateUserNotifier usersWithChangedPasswordslearn = new UpdatedLearnUserNotifier(learnconnectionString);

            ChangePasswordService myServlms = new ChangePasswordService(usersWithChangedPasswordslearn, DateTime.Now.AddHours(-hours));

            //Keep our console app alive till user presses enter
            Console.ReadLine();
        }
    }
}
