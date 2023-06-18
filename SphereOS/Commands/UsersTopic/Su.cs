using SphereOS.Logging;
using SphereOS.Shell;
using SphereOS.Users;
using System;

namespace SphereOS.Commands.UsersTopic
{
    internal class Su : Command
    {
        public Su() : base("su")
        {
            Description = "Switch to a specific user.";

            Usage = "<user>";

            Topic = "users";
        }

        internal override ReturnCode Execute(string[] args)
        {
            if (args.Length != 2)
            {
                Util.PrintLine(ConsoleColor.Red, "Invalid usage. Please provide the username of the user to switch to.");
                return ReturnCode.Invalid;
            }

            var username = args[1].Trim();

            if (Kernel.CurrentUser.Username == username)
            {
                return ReturnCode.Success;
            }

            User user = UserManager.GetUser(username);

            if (user == null)
            {
                Util.PrintLine(ConsoleColor.Red, "Unknown user.");
                return ReturnCode.Failure;
            }

            if (user.LockedOut)
            {
                Util.PrintLine(ConsoleColor.Red, $"This account has been locked out due to too many failed login attempts.");

                TimeSpan remaining = user.LockoutEnd - DateTime.Now;
                if (remaining.Minutes > 0)
                {
                    Util.PrintLine(ConsoleColor.White, $"Try again in {remaining.Minutes}m, {remaining.Seconds}s.");
                }
                else
                {
                    Util.PrintLine(ConsoleColor.White, $"Try again in {remaining.Seconds}s.");
                }

                return ReturnCode.Unauthorised;
            }

            if (!Kernel.CurrentUser.Admin)
            {
                Util.Print(ConsoleColor.Cyan, $"Password for {username}: ");
                var result = Util.ReadLineEx(mask: true);
                if (!user.Authenticate(result.Input))
                {
                    Util.PrintLine(ConsoleColor.Red, "Incorrect password.");
                    return ReturnCode.Unauthorised;
                }
            }

            Log.Info("Su", $"User '{Kernel.CurrentUser.Username}' switched to user '{user.Username}'.");

            Kernel.CurrentUser = user;

            return ReturnCode.Success;
        }
    }
}
