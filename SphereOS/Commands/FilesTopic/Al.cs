using SphereOS.Core;
using SphereOS.Shell;
using System;
using System.IO;

namespace SphereOS.Commands.FilesTopic
{
    internal class Al : Command
    {
        public Al() : base("al")
        {
            Description = "Append a line to a file.";

            Usage = "<file> <text>";

            Topic = "files";
        }

        internal override ReturnCode Execute(string[] args)
        {
            if (args.Length < 3)
            {
                Util.PrintLine(ConsoleColor.Red, "Invalid usage. Please provide the name of the file to append and the text to append.");
                return ReturnCode.Invalid;
            }

            string appending = string.Empty;
            for (int i = 2; i < args.Length; i++)
            {
                appending += args[i];
                if (i != args.Length - 1)
                {
                    appending += " ";
                }
            }

            string path = PathUtil.JoinPaths(Shell.Shell.CurrentShell.WorkingDir, args[1]);

            if (!FileSecurity.CanAccess(Kernel.CurrentUser, path))
            {
                Util.PrintLine(ConsoleColor.Red, "You do not have permission to access this file.");
                return ReturnCode.Unauthorised;
            }

            if (File.Exists(path))
            {
                string text = File.ReadAllText(path);
                if (!text.EndsWith("\n"))
                {
                    text += "\n";
                }
                text += appending + "\n";

                File.WriteAllText(path, text);
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
