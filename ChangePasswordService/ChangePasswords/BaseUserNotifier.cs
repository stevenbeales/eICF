using ChangePassword;
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

namespace ChangePasswords {
    public abstract class BaseUserNotifier: IUpdateUserNotifier {

        public void Execute(DateTime lastRun)  {
            ChangedUsers = GetChangedUsers(lastRun);
        }

        public ArrayList ChangedUsers {
            get
            {
                return _ChangedUsers;
            }
            set
            {
                _ChangedUsers = value;
            }

        }

        public string ConnectionString {
            get
            {
                return _ConnectionString;
            }

            set
            {
                _ConnectionString = value;
            }
        }

        protected DataTable GetData(SqlCommand cmd) {
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(ConnectionString);
            cmd.Connection = con;
            SqlDataAdapter sda = new SqlDataAdapter();
            cmd.CommandType = CommandType.Text;
            try {
                con.Open();
                sda.SelectCommand = cmd;
                sda.Fill(dt);
            }
            catch {
                return dt;
            }
            finally {
                con.Close();
                cmd.Dispose();
                sda.Dispose();
            }
            return dt;
        }

        abstract protected ArrayList GetChangedUsers(DateTime lastRun);

        private string _ConnectionString = "";
        private ArrayList _ChangedUsers = new ArrayList();
    }
}
