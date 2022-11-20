using SphereOS.Core;
using SphereOS.Shell;
using System;
using System.IO;

namespace SphereOS.Commands.FilesTopic
{
    internal class Rmdir : Command
    {
        public Rmdir() : base("rmdir")
        {
            Description = "Delete a directory.";

            Topic = "files";
        }

        internal override ReturnCode Execute(string[] args)
        {
            if (args.Length != 2)
            {
                Util.PrintLine(ConsoleColor.Red, "Invalid usage. Please provide the name of the directory to delete.");
                return ReturnCode.Invalid;
            }

            string path = Path.Join(Shell.Shell.WorkingDir, args[1]);

            if (!FileSecurity.CanAccess(Kernel.CurrentUser, path))
            {
                Util.PrintLine(ConsoleColor.Red, "You do not have permission to access this directory.");
                return ReturnCode.Unauthorised;
            }

            if (Directory.Exists(path))
            {
                Directory.Delete(path);
            }
            else
            {
                Util.PrintLine(ConsoleColor.Red, $"No such directory '{Path.GetFileName(path)}'.");
                return ReturnCode.NotFound;
            }

            return ReturnCode.Success;
        }
    }
}
