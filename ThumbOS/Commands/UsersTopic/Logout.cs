using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThumbOS.Commands.UsersTopic
{
    internal class Logout : Command
    {
        public Logout() : base("logout")
        {
            Description = "Log out of ThumbOS.";

            Topic = "users";
        }

        internal override ReturnCode Execute(string[] args)
        {
            Util.PrintLine(ConsoleColor.Green, "Goodbye!");
            Kernel.CurrentUser = null;
            return ReturnCode.Success;
        }
    }
}
