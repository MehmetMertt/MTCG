using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MCTG.Classes
{
    public class Authentication
    {
        public string Username { get; set; }
        private string Password { get; set; }
        public Authentication(string username, string password)
        {
            string hashedPassword = "";
            var data = Encoding.UTF8.GetBytes(password);
            hashedPassword = SHA512.HashData(data).ToString();
            this.Username = username;
            this.Password = hashedPassword;
        }
    }
}
