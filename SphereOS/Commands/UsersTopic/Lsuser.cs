using SphereOS.Shell;
using SphereOS.Users;
using System;

namespace SphereOS.Commands.UsersTopic
{
    internal class Lsuser : Command
    {
        public Lsuser() : base("lsusr")
        {
            Description = "List all users on this PC.";

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
                if (user.PasswordExpired)
                {
                    Util.Print(ConsoleColor.Yellow, " (password expired)");
                }
                if (user.Messages.Count > 0)
                {
                    Util.Print(ConsoleColor.Gray, $" ({user.Messages.Count} unread message(s))");
                }
                Console.WriteLine();
            }

            return ReturnCode.Success;
        }
    }
}
