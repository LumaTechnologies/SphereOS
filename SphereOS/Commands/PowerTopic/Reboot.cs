using SphereOS.Shell;
using SphereOS.Users;
using System;
using Sys = Cosmos.System;

namespace SphereOS.Commands.PowerTopic
{
    internal class Reboot : Command
    {
        public Reboot() : base("reboot")
        {
            Description = "Reboot your PC.";

            Topic = "power";
        }

        internal override ReturnCode Execute(string[] args)
        {
            Util.PrintLine(ConsoleColor.Green, "Goodbye!");
            Util.PrintTask("Rebooting...");
            UserManager.Flush();
            Sys.Power.Reboot();
            return ReturnCode.Success;
        }
    }
}
