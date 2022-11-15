using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SphereOS.Commands.FilesTopic
{
    internal class Mkdir : Command
    {
        public Mkdir() : base("mkdir")
        {
            Description = "Create a directory";

            Topic = "files";
        }

        internal override ReturnCode Execute(string[] args)
        {
            if (args.Length != 2)
            {
                Util.PrintLine(ConsoleColor.Red, "Invalid usage. Please provide the new directory name.");
                return ReturnCode.Invalid;
            }

            var newDir = Path.Combine(Kernel.WorkingDir, args[1]);

            if (Directory.Exists(newDir))
            {
                Util.PrintLine(ConsoleColor.Red, "This directory already exists.");
                return ReturnCode.Failure;
            }
            else
            {
                Directory.CreateDirectory(newDir);
            }

            return ReturnCode.Success;
        }
    }
}
