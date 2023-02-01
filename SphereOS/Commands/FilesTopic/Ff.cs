using SphereOS.Core;
using SphereOS.Shell;
using System;
using System.IO;

namespace SphereOS.Commands.FilesTopic
{
    internal class Ff : Command
    {
        public Ff() : base("ff")
        {
            Description = "Find lines in a file.";

            Usage = "<file> <query>";

            Topic = "files";
        }

        internal override ReturnCode Execute(string[] args)
        {
            if (args.Length < 3)
            {
                Util.PrintLine(ConsoleColor.Red, "Invalid usage. Please provide the name of the file to read and the search query.");
                return ReturnCode.Invalid;
            }

            string path = PathUtil.JoinPaths(Shell.Shell.CurrentShell.WorkingDir, args[1]);

            string query = string.Empty;
            for (int i = 2; i < args.Length; i++)
            {
                query += args[i];
                if (i != args.Length - 1)
                {
                    query += " ";
                }
            }

            if (!FileSecurity.CanAccess(Kernel.CurrentUser, path))
            {
                Util.PrintLine(ConsoleColor.Red, "You do not have permission to access this file.");
                return ReturnCode.Unauthorised;
            }

            if (File.Exists(path))
            {
                foreach (var line in File.ReadAllLines(path))
                {
                    if (line.Contains(query))
                    {
                        Console.WriteLine(line);
                    }
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
