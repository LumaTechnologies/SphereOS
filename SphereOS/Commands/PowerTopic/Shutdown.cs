using SphereOS.Core;
using SphereOS.Shell;
using System;

namespace SphereOS.Commands.PowerTopic
{
    internal class Shutdown : Command
    {
        public Shutdown() : base("shutdown")
        {
            Description = "Shut down your PC.";

            Topic = "power";
        }

        internal override ReturnCode Execute(string[] args)
        {
            Util.PrintLine(ConsoleColor.Green, "Goodbye!");
            Power.Shutdown(reboot: false);
            return ReturnCode.Success;
        }
    }
}
