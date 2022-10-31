using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SphereOS.Commands.UsersTopic
{
    internal class Adduser : Command
    {
        public Adduser() : base("adduser")
        {
            Description = "Add a new user.";

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
                Util.PrintLine(ConsoleColor.Red, "Invalid usage. Please provide a new username.");
                return ReturnCode.Invalid;
            }

            var username = args[1].Trim();
            if (UserManager.GetUser(username) != null)
            {
                Util.PrintLine(ConsoleColor.Red, $"A user named {username} already exists.");
                return ReturnCode.Failure;
            }

            Util.Print(ConsoleColor.Cyan, $"New password for {username}: ");
            var newPassword = Util.ReadPassword();

            UserManager.AddUser(username, newPassword, admin: false);

            Util.PrintLine(ConsoleColor.Green, $"Successfully created user {username}.");

            return ReturnCode.Success;
        }
    }
}
