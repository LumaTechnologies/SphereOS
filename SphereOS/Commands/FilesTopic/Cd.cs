using SphereOS.Core;
using SphereOS.Shell;
using System;
using System.IO;

namespace SphereOS.Commands.FilesTopic
{
    internal class Cd : Command
    {
        public Cd() : base("cd")
        {
            Description = "Change the working directory.";

            Usage = "<dir>";

            Topic = "files";
        }

        internal override ReturnCode Execute(string[] args)
        {
            if (args.Length != 2)
            {
                Util.PrintLine(ConsoleColor.Red, "Invalid usage. Please provide the path to navigate to.");
                return ReturnCode.Invalid;
            }

            var newDir = PathUtil.JoinPaths(Shell.Shell.CurrentShell.WorkingDir, args[1]);
            if (Directory.Exists(newDir) && FileSecurity.CanAccess(Kernel.CurrentUser, newDir))
            {
                Shell.Shell.CurrentShell.WorkingDir = Path.GetFullPath(newDir);
            }
            else
            {
                string dirname = Path.GetFileName(newDir);
                if (dirname.Trim().Length > 0)
                {
                    Util.PrintLine(ConsoleColor.Red, $"No such directory '{dirname}'.");
                }
                else
                {
                    Util.PrintLine(ConsoleColor.Red, $"No such directory.");
                }
                return ReturnCode.NotFound;
            }

            return ReturnCode.Success;
        }
    }
}
