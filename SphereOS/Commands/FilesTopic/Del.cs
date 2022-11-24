using SphereOS.Core;
using SphereOS.Shell;
using System;
using System.IO;

namespace SphereOS.Commands.FilesTopic
{
    internal class Del : Command
    {
        public Del() : base("del")
        {
            Description = "Delete a file.";

            Topic = "files";
        }

        internal override ReturnCode Execute(string[] args)
        {
            if (args.Length != 2)
            {
                Util.PrintLine(ConsoleColor.Red, "Invalid usage. Please provide the name of the file to delete.");
                return ReturnCode.Invalid;
            }

            string path = Path.Join(Shell.Shell.CurrentShell.WorkingDir, args[1]);

            if (!FileSecurity.CanAccess(Kernel.CurrentUser, path))
            {
                Util.PrintLine(ConsoleColor.Red, "You do not have permission to access this file.");
                return ReturnCode.Unauthorised;
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else
            {
                Util.PrintLine(ConsoleColor.Red, $"No such file '{Path.GetFileName(path)}'.");
                return ReturnCode.NotFound;
            }

            return ReturnCode.Success;
        }
    }
}
