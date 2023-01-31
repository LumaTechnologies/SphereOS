using SphereOS.Core;
using SphereOS.Shell;
using System;
using System.IO;

namespace SphereOS.Commands.FilesTopic
{
    internal class Cp : Command
    {
        public Cp() : base("cp")
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

            string srcPath;
            if (args[1].Contains(@":\"))
            {
                srcPath = args[1];
            }
            else
            {
                srcPath = Path.Join(Shell.Shell.CurrentShell.WorkingDir, args[1]);
            }

            string destPath;
            if (args[2].Contains(@":\"))
            {
                destPath = args[2];
            }
            else
            {
                destPath = Path.Join(Shell.Shell.CurrentShell.WorkingDir, args[2]);
            }

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
