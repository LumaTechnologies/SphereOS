using SphereOS.Shell;
using SphereOS.Users;
using System;

namespace SphereOS.Commands.UsersTopic
{
    internal class Pass : Command
    {
        public Pass() : base("pass")
        {
            Description = "Change your password.";

            Topic = "users";
        }

        internal override ReturnCode Execute(string[] args)
        {
            Util.Print(ConsoleColor.Cyan, "Current password: ");
            string currentPassword = Util.ReadLineEx(cancelKey: null, mask: true);

            if (Kernel.CurrentUser.Authenticate(currentPassword))
            {
                Util.Print(ConsoleColor.Cyan, "New password: ");
                string newPassword = Util.ReadLineEx(cancelKey: null, mask: true);

                Kernel.CurrentUser.ChangePassword(currentPassword, newPassword);
                UserManager.Flush();
                Util.PrintLine(ConsoleColor.Green, "Password successfully changed.");

                return ReturnCode.Success;
            }
            else
            {
                Util.PrintLine(ConsoleColor.Red, "Incorrect password.");
                return ReturnCode.Unauthorised;
            }
        }
    }
}
