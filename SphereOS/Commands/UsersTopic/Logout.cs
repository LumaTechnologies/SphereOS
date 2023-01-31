using SphereOS.Shell;
using System;

namespace SphereOS.Commands.UsersTopic
{
    internal class Logout : Command
    {
        public Logout() : base("logout")
        {
            Description = "Log out of this PC.";

            Topic = "users";
        }

        internal override ReturnCode Execute(string[] args)
        {
            Util.PrintLine(ConsoleColor.Green, $"Logging out {Kernel.CurrentUser.Username}.");

            Kernel.CurrentUser = null;
            Shell.Shell.CurrentShell.WorkingDir = @"0:\";

            Console.Clear();
            return ReturnCode.Success;
        }
    }
}
