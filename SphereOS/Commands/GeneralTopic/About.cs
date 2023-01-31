using SphereOS.Shell;
using System;

namespace SphereOS.Commands.GeneralTopic
{
    internal class About : Command
    {
        public About() : base("about")
        {
            Description = "Show information about SphereOS.";

            Topic = "general";
        }

        internal override ReturnCode Execute(string[] args)
        {
            Util.Print(ConsoleColor.Magenta, "SphereOS");
            Util.PrintLine(ConsoleColor.Gray, $" - version {Kernel.Version}");
            Util.PrintLine(ConsoleColor.White, "Copyright (c) 2023. All rights reserved.");
            return ReturnCode.Success;
        }
    }
}
