using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThumbOS.Commands.ConsoleTopic
{
    internal class About : Command
    {
        public About() : base("about")
        {
            Description = "Show information about ThumbOS.";

            Topic = "console";
        }

        internal override ReturnCode Execute(string[] args)
        {
            Util.Print(ConsoleColor.Magenta, "ThumbOS");
            Util.PrintLine(ConsoleColor.Gray, $" - version {Kernel.Version}");
            Util.PrintLine(ConsoleColor.White, "Copyright (c) 2022. All rights reserved.");
            return ReturnCode.Success;
        }
    }
}
