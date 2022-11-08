using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using SphereOS.Logging;
using SphereOS.Users;

namespace SphereOS.Commands.UsersTopic
{
    internal class Admin : Command
    {
        public Admin() : base("admin")
        {
            Description = "Set or remove a user as an admin.";

            Topic = "users";
        }

        /// <summary>
        /// Check if any admins would be left on the system after removing a user as admin.
        /// </summary>
        /// <param name="removingUser">The user to be removed as an admin.</param>
        /// <returns>Whether there would be admins left on the system.</returns>
        private bool ValidateAdminRemoval(User removingUser)
        {
            foreach (User user in UserManager.Users)
            {
                if (user == removingUser)
                    continue;

                if (user.Admin)
                    return true;
            }
            return false;
        }

        internal override ReturnCode Execute(string[] args)
        {
            if (!Kernel.CurrentUser.Admin)
            {
                Util.PrintLine(ConsoleColor.Red, "Unauthorised. You must be an admin to run this command.");
                return ReturnCode.Unauthorised;
            }

            if (args.Length != 3)
            {
                Util.PrintLine(ConsoleColor.Red, "Invalid usage.\nUsage: admin <true/false> <username>");
                return ReturnCode.Invalid;
            }

            var username = args[2].Trim();

            User user = UserManager.GetUser(username);

            if (user != null)
            {
                switch (args[1])
                {
                    case "true":
                        UserManager.SetAdmin(username, true);
                        break;
                    case "false":
                        if (!ValidateAdminRemoval(user))
                        {
                            Util.PrintLine(ConsoleColor.Red, "There must be at least one admin user.");
                            return ReturnCode.Invalid;
                        }
                        UserManager.SetAdmin(username, false);
                        break;
                    default:
                        Util.PrintLine(ConsoleColor.Red, "Invalid usage.\nUsage: admin <true/false> <username>");
                        return ReturnCode.Invalid;
                }
            }
            else
            {
                Util.PrintLine(ConsoleColor.Red, "No users were affected.");
                return ReturnCode.Failure;
            }

            return ReturnCode.Success;
        }
    }
}
