using ChangePasswords.PasswordServiceReference;
using System;
using System.Configuration;

namespace ChangePassword {
    /// <summary>
    /// Class to change the MiCo Password 
    /// </summary>
    public class MiCoPasswordChanger {
        /// <summary>
        /// Execute method to perform password change
        /// </summary>
        /// <param name="user">username</param>
        /// <param name="newPassword">new password</param>
        /// <returns>true if successful</returns>
        public bool Execute(object user, object newPassword) {
            try {
                return SetPassword(user, newPassword) == "True";
            }
            catch (Exception ex) {
                Console.WriteLine("Error executing password changer " + ex.Message);
                return false;
            }

        }

        private string SetPassword(object user, object newPassword) {

            var systemRequest = new CompositeType {
                CustomerName = tokenSettings.CustomerName,
                UserName = tokenSettings.UserName,
                Password = tokenSettings.Password
            };

            var userRequest = new CredentialParams {
                CustomerName = tokenSettings.CustomerName,
                Token = "",
                UserName = user.ToString(),
                NewPassword = newPassword.ToString()
            };

            using (var svc = new ServiceClient()) {
                var result = svc.GetTokenString(systemRequest);
                userRequest.Token = result;

                return svc.SetNewPassword(userRequest);
            }
        }

        static string customer = ConfigurationManager.AppSettings["Customer"];
        static string serviceAccount = ConfigurationManager.AppSettings["MicoServiceAccount"];
        static string password = ConfigurationManager.AppSettings["MicoServicePassword"];

        private TokenSettings tokenSettings = new TokenSettings(customer, "", serviceAccount, password);
    }
}