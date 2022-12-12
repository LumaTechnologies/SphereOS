using SphereOS.Logging;
using System;
using System.Collections.Generic;

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

        /// <summary>
        /// How many failed login attempts there have been since the last login or lockout.
        /// </summary>
        private int FailedAttempts = 0;

        private bool _lockedOut = false;

        /// <summary>
        /// If the user is currently locked out.
        /// </summary>
        internal bool LockedOut
        {
            get
            {
                return _lockedOut && DateTime.Now < LockoutEnd;
            }
        }

        /// <summary>
        /// When the user's lockout ends.
        /// </summary>
        internal DateTime LockoutEnd { get; private set; }

        /// <summary>
        /// Flush all of the user's unread messages to the console.
        /// </summary>
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

        internal void ChangePassword(string currentPassword, string newPassword)
        {
            if (Authenticate(currentPassword))
            {
                Password = UserManager.HashPasswordSha256(newPassword);
            }
            else
            {
                throw new Exception("Incorrect password.");
            }
        }

        /// <summary>
        /// Check if a password is valid.
        /// Too many failed calls to this method may lock the user out temporarily. You can query this status with <see cref="LockedOut"/>.
        /// </summary>
        /// <param name="password">The password to check against.</param>
        /// <returns>Whether the password is valid.</returns>
        internal bool Authenticate(string password)
        {
            if (LockedOut)
            {
                return false;
            }

            bool valid = UserManager.HashPasswordSha256(password) == Password;

            if (valid)
            {
                FailedAttempts = 0;
            }
            else
            {
                FailedAttempts++;
                if (FailedAttempts >= 3)
                {
                    _lockedOut = true;
                    LockoutEnd = DateTime.Now + TimeSpan.FromMinutes(3);

                    FailedAttempts = 0;

                    Log.Info("User", $"{Username} was locked out due to too many incorrect attempts.");
                }
            }

            return valid;
        }
    }
}
