using SphereOS.Logging;
using SphereOS.Shell;
using SphereOS.Users;
using System;

namespace SphereOS.Commands.UsersTopic
{
    internal class Expire : Command
    {
        public Expire() : base("expire")
        {
            Description = "Require a user to change their password.";

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
                Util.PrintLine(ConsoleColor.Red, "Invalid usage. Please provide the username of the user that must change their password.");
                return ReturnCode.Invalid;
            }

            var username = args[1].Trim();

            User user = UserManager.GetUser(username);
            if (user != null)
            {
                if (user.PasswordExpired)
                {
                    Util.PrintLine(ConsoleColor.Red, $"{user.Username}'s password has already expired.");
                    return ReturnCode.Failure;
                }

                user.PasswordExpired = true;
                UserManager.Flush();

                Log.Info("Expire", $"{user.Username}'s password was set as expired by '{Kernel.CurrentUser.Username}'.");
                Util.PrintLine(ConsoleColor.Green, $"'{user.Username}' will be required to change their password the next time they log in.");
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
