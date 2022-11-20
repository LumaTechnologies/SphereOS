using SphereOS.Core;
using SphereOS.Shell;
using System;
using System.IO;

namespace SphereOS.Commands.FilesTopic
{
    internal class Mkdir : Command
    {
        public Mkdir() : base("mkdir")
        {
            Description = "Create a directory.";

            Topic = "files";
        }

        internal override ReturnCode Execute(string[] args)
        {
            if (args.Length != 2)
            {
                Util.PrintLine(ConsoleColor.Red, "Invalid usage. Please provide the new directory name.");
                return ReturnCode.Invalid;
            }

            if (!FileSecurity.CanAccess(Kernel.CurrentUser, Shell.Shell.WorkingDir))
            {
                Util.PrintLine(ConsoleColor.Red, "You do not have permission to create a directory here.");
                return ReturnCode.Unauthorised;
            }

            var newDir = Path.Combine(Shell.Shell.WorkingDir, args[1]);

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
