using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sphereOS
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
        /// <param name="password">The password.</param>
        /// <param name="admin">Whether the user is an admin.</param>
        public User(string username, string password, bool admin)
        {
            Username = username;
            Password = password;
            Admin = admin;
        }

        /// <summary>
        /// The username of the user.
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// Whether the user is an admin.
        /// </summary>
        public bool Admin { get; set; } = false;

        /// <summary>
        /// The password of the user.
        /// </summary>
        private string Password { get; set; }

        /// <summary>
        /// Check if a password is valid.
        /// </summary>
        /// <param name="password">The password to check against.</param>
        /// <returns>Whether the password is valid.</returns>
        internal bool Authenticate(string password)
        {
            return password == Password;
        }
    }
}
