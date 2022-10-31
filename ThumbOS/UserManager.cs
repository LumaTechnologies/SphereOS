using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThumbOS
{
    internal static class UserManager
    {
        internal static List<User> Users = new List<User>();

        internal static User AddUser(string username, string password)
        {
            if (GetUser(username) != null)
                throw new InvalidOperationException($"A user named {username} already exists!");
            User user = new User(username, password);
            Users.Add(user);
            return user;
        }

        internal static User? GetUser(string username)
        {
            foreach (User user in Users)
            {
                if (user.Username == username)
                    return user;
            }
            return null;
        }
    }
}
