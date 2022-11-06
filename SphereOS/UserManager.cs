using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SphereOS.Commands.UsersTopic;

namespace SphereOS
{
    internal static class UserManager
    {
        internal static List<User> Users = new List<User>();

        /*internal static void LoadData()
        {
            try
            {
                string path = @"0:\users.dat";
                if (File.Exists(path)
                {
                    string text = File.ReadAllText(path);
                    // TODO
                }
            }
            catch
            {
                Util.PrintLine(ConsoleColor.Yellow, "Unable to load user configuration.");
            }
        }*/

        internal static User AddUser(string username, string password, bool admin)
        {
            if (GetUser(username) != null)
                throw new InvalidOperationException($"A user named {username} already exists!");
            User user = new User(username, password, admin);
            Users.Add(user);
            return user;
        }

        internal static bool DeleteUser(string username)
        {
            foreach (User user in Users)
            {
                if (user.Username == username)
                {
                    Users.Remove(user);
                    return true;
                }
            }
            return false;
        }

        internal static User GetUser(string username)
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
