using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThumbOS
{
    internal class User
    {
        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string Username { get; private set; }

        private string Password { get; set; }

        internal bool Authenticate(string password)
        {
            return password == Password;
        }
    }
}
