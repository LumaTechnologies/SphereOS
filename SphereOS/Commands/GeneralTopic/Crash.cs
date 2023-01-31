using SphereOS.Core;
using SphereOS.Shell;
using System;

namespace SphereOS.Commands.GeneralTopic
{
    internal class Crash : Command
    {
        public Crash() : base("crash")
        {
            Description = "Forcefully trigger a crash.";

            Topic = "general";
        }

        internal override ReturnCode Execute(string[] args)
        {
            if (!Kernel.CurrentUser.Admin)
            {
                Util.PrintLine(ConsoleColor.Red, "Unauthorised. You must be an admin to run this command.");
                return ReturnCode.Unauthorised;
            }

            CrashScreen.ShowCrashScreen(new Exception("User triggered crash."));

            return ReturnCode.Success;
        }
    }
}
