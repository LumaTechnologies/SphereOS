using SphereOS.Core;
using SphereOS.Shell;
using System;
using System.IO;

namespace SphereOS.Commands.FilesTopic
{
    internal class Copy : Command
    {
        public Copy() : base("copy")
        {
            Description = "Copy a file.";

            Usage = "<src> <dest>";

            Topic = "files";
        }

        internal override ReturnCode Execute(string[] args)
        {
            if (args.Length != 3)
            {
                Util.PrintLine(ConsoleColor.Red, "Invalid usage. Please provide the source and destination.");
                return ReturnCode.Invalid;
            }

            string srcPath = PathUtil.JoinPaths(Shell.Shell.CurrentShell.WorkingDir, args[1]);
            string destPath = PathUtil.JoinPaths(Shell.Shell.CurrentShell.WorkingDir, args[2]);

            if (!FileSecurity.CanAccess(Kernel.CurrentUser, srcPath))
            {
                Util.PrintLine(ConsoleColor.Red, "You do not have permission to access this file.");
                return ReturnCode.Unauthorised;
            }
            if (!FileSecurity.CanAccess(Kernel.CurrentUser, destPath))
            {
                Util.PrintLine(ConsoleColor.Red, "You do not have permission to write to the destination.");
                return ReturnCode.Unauthorised;
            }

            if (File.Exists(srcPath))
            {
                File.WriteAllBytes(destPath, File.ReadAllBytes(srcPath));
            }
            else
            {
                Util.PrintLine(ConsoleColor.Red, $"No such file '{Path.GetFileName(srcPath)}'.");
                return ReturnCode.NotFound;
            }

            return ReturnCode.Success;
        }
    }
}
