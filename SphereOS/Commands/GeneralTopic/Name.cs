using SphereOS.Core;
using SphereOS.Logging;
using SphereOS.Shell;
using System;

namespace SphereOS.Commands.GeneralTopic
{
    internal class Name : Command
    {
        public Name() : base("name")
        {
            Description = "View or set the system name.";

            Usage = "[name]";

            Topic = "general";
        }

        internal override ReturnCode Execute(string[] args)
        {
            if (!Kernel.CurrentUser.Admin)
            {
                Util.PrintLine(ConsoleColor.Red, "Unauthorised. You must be an admin to run this command.");
                return ReturnCode.Unauthorised;
            }

            if (args.Length > 2)
            {
                Util.PrintLine(ConsoleColor.Red, $"Invalid usage.\nUsage: {Name} {Usage}");
                return ReturnCode.Invalid;
            }

            if (args.Length == 2)
            {
                SysCfg.Name = args[1];
                SysCfg.Flush();

                Util.PrintLine(ConsoleColor.Green, $"System name set to '{SysCfg.Name}'.");
                Log.Info("Name", $"User '{Kernel.CurrentUser.Username}' set the system name to '{SysCfg.Name}'.");
            }
            else
            {
                Console.WriteLine(SysCfg.Name);
            }

            return ReturnCode.Success;
        }
    }
}
