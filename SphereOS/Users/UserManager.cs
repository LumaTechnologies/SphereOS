using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SphereOS.Commands.UsersTopic;
using SphereOS.Crypto;
using SphereOS.Logging;
using SphereOS.Text;

namespace SphereOS.Users
{
    internal static class UserManager
    {
        internal static List<User> Users = new List<User>();

        private const string userDataPath = @"0:\users.ini";

        internal static void Load()
        {
            try
            {
                if (File.Exists(userDataPath))
                {
                    string text = File.ReadAllText(userDataPath);
                    Kernel.PrintDebug(text);
                    var reader = new IniReader(text);
                    foreach (string section in reader.GetSections())
                    {
                        int dotIndex = section.IndexOf('.');
                        if (dotIndex == -1)
                        {
                            throw new FormatException("Unrecognised key in users.ini.");
                        }
                        string type = section.Substring(0, dotIndex);
                        string key = section.Substring(dotIndex + 1);
                        switch (type)
                        {
                            case "User":
                                string username = key;
                                string password = reader.ReadString("password", section);
                                bool admin = reader.ReadBool("admin", section);

                                // Create the user without hashing the password, because it is already hashed on disk.
                                AddUser(username, password, admin, hash: false);

                                //Util.PrintTask($"DEBUG: UserMgr loadusr {username} {admin}");
                                break;
                            case "Message":
                                User from = GetUser(reader.ReadString("from", section));
                                User to = GetUser(reader.ReadString("to", section));
                                string body = reader.ReadString("body", section);
                                DateTime sent = new DateTime(reader.ReadLong("sent", section));

                                Message message = new Message(from, body, sent);
                                to.Messages.Add(message);
                                //Util.PrintTask($"DEBUG: UserMgr loadmsg {from.Username} {to.Username}");
                                break;
                            default:
                                throw new FormatException($"Unrecognised type {type}.");
                        }
                    }
                }
                else
                {
                    AddUser("admin", "password", admin: true);
                }
            }
            catch (Exception e)
            {
                AddUser("admin", "password", admin: true);

                Util.PrintLine(ConsoleColor.Yellow, "Unable to load user data.");
                Log.Warning("UserManager", $"Unable to load user data: {e.Message}");
            }
        }

        internal static User AddUser(string username, string password, bool admin, bool hash = true)
        {
            if (GetUser(username) != null)
                throw new InvalidOperationException($"A user named {username} already exists!");

            User user = new User(username, hash ? HashPasswordSha256(password) : password, admin);
            Users.Add(user);

            Flush();

            if (admin)
            {
                Log.Info("UserManager", $"Admin user {username} was added.");
            }
            else
            {
                Log.Info("UserManager", $"User {username} was added.");
            }

            return user;
        }

        internal static bool DeleteUser(string username)
        {
            foreach (User user in Users)
            {
                if (user.Username == username)
                {
                    Users.Remove(user);
                    Flush();
                    Log.Info("UserManager", $"User {username} was deleted.");
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

        internal static bool SetAdmin(string username, bool admin)
        {
            foreach (User user in Users)
            {
                if (user.Username == username)
                {
                    user.Admin = admin;
                    Flush();
                    Log.Info("UserManager", $"{user.Username}'s admin status was set to {admin}.");
                    return true;
                }
            }
            return false;
        }

        internal static void Flush()
        {
            try
            {
                var builder = new IniBuilder();
                foreach (User user in Users)
                {
                    builder.BeginSection($"User.{user.Username}");
                    builder.AddKey("password", user.Password);
                    builder.AddKey("admin", user.Admin);
                }
                Random random = new Random((int)DateTime.Now.Ticks);
                foreach (User user in Users)
                {
                    foreach (Message message in user.Messages)
                    {
                        builder.BeginSection($"Message.{random.Next()}");
                        builder.AddKey("from", message.From.Username);
                        builder.AddKey("to", user.Username);
                        builder.AddKey("body", message.Body.ToString());
                        builder.AddKey("sent", message.Sent.Ticks);
                    }
                }
                File.WriteAllText(userDataPath, builder.ToString());
            }
            catch (Exception e)
            {
                Log.Warning("UserManager", $"Failed to flush user data: {e.Message}");
            }
        }

        internal static string HashPasswordSha256(string password)
        {
            Sha256 sha256 = new Sha256();
            byte[] passwordBytesUnhashed = Encoding.Unicode.GetBytes(password);
            sha256.AddData(passwordBytesUnhashed, 0, (uint)passwordBytesUnhashed.Length);
            return Convert.ToBase64String(sha256.GetHash());
        }
    }
}
