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

            if (args[1].Trim() == "-")
            {
                Shell.Shell.CurrentShell.WorkingDirHistoryBack();

                return ReturnCode.Success;
            }

            var newDir = PathUtil.JoinPaths(Shell.Shell.CurrentShell.WorkingDir, args[1]);
            if (Directory.Exists(newDir))
            {
                if (FileSecurity.CanAccess(Kernel.CurrentUser, newDir))
                {
                    Shell.Shell.CurrentShell.WorkingDir = Path.GetFullPath(newDir);
                }
                else
                {
                    Util.PrintLine(ConsoleColor.Red, $"You do not have permission to access this directory.");
                    return ReturnCode.Unauthorised;
                }
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
