using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThumbOS.Commands.UsersTopic
{
    internal class Deluser : Command
    {
        public Deluser() : base("deluser")
        {
            Description = "Delete a user.";

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
            
            if (UserManager.DeleteUser(username))
            {
                Util.PrintLine(ConsoleColor.Green, $"Successfully deleted user {username}.");
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
