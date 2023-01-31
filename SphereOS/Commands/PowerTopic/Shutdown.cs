using SphereOS.Core;
using SphereOS.Shell;
using System;

namespace SphereOS.Commands.PowerTopic
{
    internal class Shutdown : Command
    {
        public Shutdown() : base("shutdown")
        {
            Description = "Shut down this PC.";

            Topic = "power";
        }

        internal override ReturnCode Execute(string[] args)
        {
            if (!Kernel.CurrentUser.Admin)
            {
                Util.PrintLine(ConsoleColor.Red, "Unauthorised. You must be an admin to run this command.");
                return ReturnCode.Unauthorised;
            }

            Power.Shutdown(reboot: false);
            return ReturnCode.Success;
        }
    }
}
