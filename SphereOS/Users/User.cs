using SphereOS.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SphereOS.Users
{
    /// <summary>
    /// A user.
    /// </summary>
    internal class User
    {
        /// <summary>
        /// Create a user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The hashed password.</param>
        /// <param name="admin">Whether the user is an admin.</param>
        internal User(string username, string password, bool admin)
        {
            Username = username;
            Password = password;
            Admin = admin;
        }

        /// <summary>
        /// The username of the user.
        /// </summary>
        internal string Username { get; private set; }

        /// <summary>
        /// Whether the user is an admin.
        /// </summary>
        internal bool Admin { get; set; } = false;

        /// <summary>
        /// The hashed password of the user.
        /// </summary>
        internal string Password { get; set; }

        /// <summary>
        /// Unread messages to this user.
        /// </summary>
        internal List<Message> Messages { get; set; } = new List<Message>();

        internal void FlushMessages()
        {
            foreach (var message in Messages)
            {
                Console.WriteLine();
                Console.WriteLine($"Message from {message.From.Username} at {message.Sent.ToString("HH:mm")}");
                Console.WriteLine(message.Body);
                Console.WriteLine();
            }
            Messages.Clear();
        }

        /// <summary>
        /// Check if a password is valid.
        /// </summary>
        /// <param name="password">The password to check against.</param>
        /// <returns>Whether the password is valid.</returns>
        internal bool Authenticate(string password)
        {
            return UserManager.HashPasswordSha256(password) == Password;
        }
    }
}
