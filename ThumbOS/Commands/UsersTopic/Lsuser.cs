using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sphereOS.Commands.UsersTopic
{
    internal class Lsuser : Command
    {
        public Lsuser() : base("lsusr")
        {
            Description = "List all users on the system.";

            Topic = "users";
        }

        internal override ReturnCode Execute(string[] args)
        {
            foreach (User user in UserManager.Users)
            {
                Util.Print(ConsoleColor.White, user.Username);
                if (user.Admin)
                {
                    Util.Print(ConsoleColor.Cyan, " (admin)");
                }
                Console.WriteLine();
            }

            return ReturnCode.Success;
        }
    }
}
