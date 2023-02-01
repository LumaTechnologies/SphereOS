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
            ReadLineExResult currentPasswordResult = Util.ReadLineEx(mask: true);

            if (Kernel.CurrentUser.Authenticate(currentPasswordResult.Input))
            {
                Util.Print(ConsoleColor.Cyan, "New password: ");
                var newPasswordResult = Util.ReadLineEx(mask: true);

                Kernel.CurrentUser.ChangePassword(currentPasswordResult.Input, newPasswordResult.Input);
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
