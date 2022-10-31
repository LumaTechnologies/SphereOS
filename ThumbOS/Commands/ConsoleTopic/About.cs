using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sphereOS.Commands.ConsoleTopic
{
    internal class About : Command
    {
        public About() : base("about")
        {
            Description = "Show information about sphereOS.";

            Topic = "console";
        }

        internal override ReturnCode Execute(string[] args)
        {
            Util.Print(ConsoleColor.Magenta, "sphereOS");
            Util.PrintLine(ConsoleColor.Gray, $" - version {Kernel.Version}");
            Util.PrintLine(ConsoleColor.White, "Copyright (c) 2022. All rights reserved.");
            return ReturnCode.Success;
        }
    }
}
