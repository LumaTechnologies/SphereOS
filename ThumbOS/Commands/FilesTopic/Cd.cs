using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sphereOS.Commands.FilesTopic
{
    internal class Cd : Command
    {
        public Cd() : base("cd")
        {
            Description = "Change the working directory.";

            Topic = "files";
        }

        internal override ReturnCode Execute(string[] args)
        {
            if (args.Length != 2)
            {
                Util.PrintLine(ConsoleColor.Red, "Invalid usage. Please provide the path to navigate to.");
                return ReturnCode.Invalid;
            }

            Kernel.WorkingDir = Path.GetFullPath(Path.Combine(Kernel.WorkingDir, args[1]));

            return ReturnCode.Success;
        }
    }
}
