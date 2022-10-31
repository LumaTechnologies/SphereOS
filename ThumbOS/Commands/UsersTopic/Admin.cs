using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sphereOS.Commands.UsersTopic
{
    internal class Admin : Command
    {
        public Admin() : base("admin")
        {
            Description = "Set or remove a user as an admin.";

            Topic = "users";
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
                        user.Admin = true;
                        break;
                    case "false":
                        user.Admin = false;
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
