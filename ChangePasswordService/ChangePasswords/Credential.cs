namespace ChangePassword {
    public class Credential {
        public Credential(string userName, string password) {
            Username = userName;
            Password = password;
        }
        public string Username {get; set;}

        public string Password { get; set; }
      
    }
}
