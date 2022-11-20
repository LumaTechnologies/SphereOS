using SphereOS.Core;
using SphereOS.Shell;
using System;
using System.IO;

namespace SphereOS.Commands.FilesTopic
{
    internal class Ls : Command
    {
        public Ls() : base("ls")
        {
            Description = "List files and directories.";

            Topic = "files";
        }

        internal override ReturnCode Execute(string[] args)
        {
            if (!FileSecurity.CanAccess(Kernel.CurrentUser, Shell.Shell.WorkingDir))
            {
                Util.PrintLine(ConsoleColor.Red, "You do not have permission to access this directory.");
                return ReturnCode.Unauthorised;
            }

            foreach (var dir in Directory.GetDirectories(Shell.Shell.WorkingDir))
            {
                Util.Print(ConsoleColor.Green, Path.GetFileName(dir) + " ");
            }
            foreach (var file in Directory.GetFiles(Shell.Shell.WorkingDir))
            {
                Util.Print(ConsoleColor.Cyan, Path.GetFileName(file) + " ");
            }

            Console.WriteLine();

            return ReturnCode.Success;
        }
    }
}
