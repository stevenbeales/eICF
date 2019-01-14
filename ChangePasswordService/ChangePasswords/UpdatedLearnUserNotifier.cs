using ChangePasswords;
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

namespace ChangePassword {
    /// <summary>
    /// Monitor Learn for changed users 
    /// </summary>
    public class UpdatedLearnUserNotifier: BaseUserNotifier {
        public UpdatedLearnUserNotifier (string connectionString) {
            ConnectionString = connectionString;    
        }

        protected override ArrayList GetChangedUsers(DateTime lastRun) {
            ArrayList changedUsers = new ArrayList();
            try { 
                string sql = string.Format("Select username, password from Users where update_date >= '{0}' or password_active >= '{0}'", lastRun.ToString("MM/dd/yyyy hh:mm:ss tt"));

                System.Diagnostics.Trace.WriteLine("SQL: " + sql);

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                DataTable dt = GetData(cmd);
                Console.WriteLine("LMS Users found: " + dt.Rows.Count.ToString());

                foreach (DataRow row in dt.Rows) {
                    Credential credential = new Credential(row["Username"].ToString(), row["Password"].ToString());
                    changedUsers.Add(credential);
                } 
                return changedUsers;
            }
            catch (Exception ex) {
                Console.WriteLine("Error: " + ex.Message);
                return changedUsers;
            }
        }
    }
}
