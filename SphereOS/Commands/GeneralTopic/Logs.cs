using SphereOS.Logging;
using SphereOS.Shell;
using System;

namespace SphereOS.Commands.GeneralTopic
{
    internal class Logs : Command
    {
        public Logs() : base("logs")
        {
            Description = "Show recent log messages.";

            Topic = "general";
        }

        internal override ReturnCode Execute(string[] args)
        {
            if (!Kernel.CurrentUser.Admin)
            {
                Util.PrintLine(ConsoleColor.Red, "Unauthorised. You must be an admin to run this command.");
                return ReturnCode.Unauthorised;
            }

            foreach (LogEvent log in Log.Logs)
            {
                log.Print();
            }

            Util.PrintLine(ConsoleColor.Gray, $"{Log.Logs.Count} log message(s)");

            return ReturnCode.Success;
        }
    }
}
