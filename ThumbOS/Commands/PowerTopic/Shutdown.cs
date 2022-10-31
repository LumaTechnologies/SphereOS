using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sphereOS.Commands;
using Sys = Cosmos.System;

namespace sphereOS.Commands.PowerTopic
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
            Util.PrintTask("Shutting down...");
            Sys.Power.Shutdown();
            return ReturnCode.Success;
        }
    }
}
