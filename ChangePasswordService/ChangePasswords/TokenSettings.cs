namespace ChangePassword {
    public class TokenSettings {

        public TokenSettings (string customerName, string token, string userName, string passWord) {
            CustomerName = customerName;
            Token = token;
            UserName = userName;
            Password = passWord;
        }

        public string CustomerName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
   
        public string Token { get; set; }
    }
}
