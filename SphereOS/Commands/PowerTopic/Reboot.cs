using SphereOS.Core;
using SphereOS.Shell;
using System;

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
            Power.Shutdown(reboot: true);
            return ReturnCode.Success;
        }
    }
}
