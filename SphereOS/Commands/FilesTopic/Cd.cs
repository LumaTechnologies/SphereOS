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

            Topic = "files";
        }

        internal override ReturnCode Execute(string[] args)
        {
            if (args.Length != 2)
            {
                Util.PrintLine(ConsoleColor.Red, "Invalid usage. Please provide the path to navigate to.");
                return ReturnCode.Invalid;
            }

            if (args[1] == "..")
            {
                Shell.Shell.CurrentShell.WorkingDir = Directory.GetParent(Shell.Shell.CurrentShell.WorkingDir).FullName;
            }
            else if (args[1] == "~")
            {
                Shell.Shell.CurrentShell.WorkingDir = $@"0:\users\{Kernel.CurrentUser.Username}";
            }
            else
            {
                var newDir = Path.Combine(Shell.Shell.CurrentShell.WorkingDir, args[1]);
                if (Directory.Exists(newDir))
                {
                    Shell.Shell.CurrentShell.WorkingDir = Path.GetFullPath(newDir);
                }
                else
                {
                    Util.PrintLine(ConsoleColor.Red, $"No such directory '{Path.GetFileName(newDir)}'.");
                    return ReturnCode.NotFound;
                }
            }

            return ReturnCode.Success;
        }
    }
}
