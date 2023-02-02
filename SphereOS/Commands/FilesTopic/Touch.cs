using SphereOS.Core;
using SphereOS.Shell;
using System;
using System.IO;

namespace SphereOS.Commands.FilesTopic
{
    internal class Touch : Command
    {
        public Touch() : base("touch")
        {
            Description = "Create a file.";

            Usage = "<file>";

            Topic = "files";
        }

        internal override ReturnCode Execute(string[] args)
        {
            if (args.Length != 2)
            {
                Util.PrintLine(ConsoleColor.Red, "Invalid usage. Please provide the name of the file to create.");
                return ReturnCode.Invalid;
            }

            string path = PathUtil.JoinPaths(Shell.Shell.CurrentShell.WorkingDir, args[1]);

            if (!FileSecurity.CanAccess(Kernel.CurrentUser, path))
            {
                Util.PrintLine(ConsoleColor.Red, "You do not have permission to create this file.");
                return ReturnCode.Unauthorised;
            }

            if (File.Exists(path))
            {
                Util.PrintLine(ConsoleColor.Red, "This file already exists.");
                return ReturnCode.Failure;
            }

            File.Create(path).Close();

            return ReturnCode.Success;
        }
    }
}
