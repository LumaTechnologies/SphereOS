using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sphereOS.Commands;
using Sys = Cosmos.System;

namespace sphereOS.Commands.PowerTopic
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
            Sys.Power.Reboot();
            return ReturnCode.Success;
        }
    }
}
