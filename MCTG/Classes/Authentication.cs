using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MTCG.Classes
{
    public class Authentication
    {
        public string Username { get; set; }
        public string Password { get; private set; }

        public string Token { get; private set; }

        public void GenerateToken()
        {
            this.Token = sha512(RandomString(512));
        }

        public string getToken()
        {
            return this.Token;
        }

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        ///https://gist.github.com/obrassard/766951b3c65984273ce4b6475e568cf5
        public static string sha512(string inputString)
        {
            SHA512 sha512 = SHA512.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hash = sha512.ComputeHash(bytes);
            return GetStringFromHash(hash);
        }

        private static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }
        ///

        public Authentication(string username, string password)
        {
            string hashedPassword = sha512(password);

            this.Username = username;
            this.Password = hashedPassword;
        }
    }
}
