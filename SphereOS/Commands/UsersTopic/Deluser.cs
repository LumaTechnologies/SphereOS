using SphereOS.Shell;
using SphereOS.Users;
using System;
using System.IO;

namespace SphereOS.Commands.UsersTopic
{
    internal class Deluser : Command
    {
        public Deluser() : base("deluser")
        {
            Description = "Delete a user.";

            Usage = "<user>";

            Topic = "users";
        }

        internal override ReturnCode Execute(string[] args)
        {
            if (!Kernel.CurrentUser.Admin)
            {
                Util.PrintLine(ConsoleColor.Red, "Unauthorised. You must be an admin to run this command.");
                return ReturnCode.Unauthorised;
            }

            if (args.Length != 2)
            {
                Util.PrintLine(ConsoleColor.Red, "Invalid usage. Please provide the username of the user to delete.");
                return ReturnCode.Invalid;
            }

            var username = args[1].Trim();

            if (Kernel.CurrentUser.Username == username)
            {
                Util.PrintLine(ConsoleColor.Red, "Cannot delete the current user.");
                return ReturnCode.Failure;
            }

            if (UserManager.DeleteUser(username))
            {
                Util.PrintLine(ConsoleColor.Green, $"Successfully deleted user {username}.");

                string home = @$"0:\users\{username}";
                if (Directory.Exists(home))
                {
                    Util.PrintWarning($"This user still has a home directory at '{home}'.\nYou may want to delete it.");
                }

                return ReturnCode.Success;
            }
            else
            {
                Util.PrintLine(ConsoleColor.Red, $"No users were affected.");
                return ReturnCode.Failure;
            }
        }
    }
}
