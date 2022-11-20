using SphereOS.Shell;
using SphereOS.Users;
using System;
using Sys = Cosmos.System;

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
            Util.PrintTask("Shutting down...");
            UserManager.Flush();
            Sys.Power.Shutdown();
            return ReturnCode.Success;
        }
    }
}
