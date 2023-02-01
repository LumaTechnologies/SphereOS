using SphereOS.Core;
using SphereOS.Shell;
using System;
using System.IO;

namespace SphereOS.Commands.FilesTopic
{
    internal class Cat : Command
    {
        public Cat() : base("cat")
        {
            Description = "Read a file.";

            Usage = "<file>";

            Topic = "files";
        }

        internal override ReturnCode Execute(string[] args)
        {
            if (args.Length != 2)
            {
                Util.PrintLine(ConsoleColor.Red, "Invalid usage. Please provide the name of the file to read.");
                return ReturnCode.Invalid;
            }

            string path = PathUtil.JoinPaths(Shell.Shell.CurrentShell.WorkingDir, args[1]);

            if (!FileSecurity.CanAccess(Kernel.CurrentUser, path))
            {
                Util.PrintLine(ConsoleColor.Red, "You do not have permission to access this file.");
                return ReturnCode.Unauthorised;
            }

            if (File.Exists(path))
            {
                foreach (var line in File.ReadAllLines(path))
                {
                    Console.WriteLine(line);
                }
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
