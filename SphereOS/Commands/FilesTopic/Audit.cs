using SphereOS.Core;
using SphereOS.Shell;
using SphereOS.Users;
using System;
using System.IO;

namespace SphereOS.Commands.FilesTopic
{
    internal class Audit : Command
    {
        public Audit() : base("audit")
        {
            Description = "List who can access a path.";

            Usage = "<path>";

            Topic = "files";
        }

        internal override ReturnCode Execute(string[] args)
        {
            if (args.Length != 2)
            {
                Util.PrintLine(ConsoleColor.Red, "Invalid usage. Please provide the path to audit.");
                return ReturnCode.Invalid;
            }

            string path = PathUtil.JoinPaths(Shell.Shell.CurrentShell.WorkingDir, args[1]);

            if (!FileSecurity.CanAccess(Kernel.CurrentUser, path))
            {
                Util.PrintLine(ConsoleColor.Red, "You do not have permission to audit this path.");
                return ReturnCode.Unauthorised;
            }

            if (File.Exists(path) || Directory.Exists(path))
            {
                int count = 0;
                foreach (User user in UserManager.Users)
                {
                    if (FileSecurity.CanAccess(user, path))
                    {
                        Util.Print(ConsoleColor.White, user.Username);
                        Util.PrintLine(ConsoleColor.Gray, " - full access");
                        count++;
                    }
                }

                Util.PrintLine(ConsoleColor.Gray, $"{count} users total");
            }
            else
            {
                Util.PrintLine(ConsoleColor.Red, $"No such file or directory '{path}'.");
                return ReturnCode.NotFound;
            }

            return ReturnCode.Success;
        }
    }
}
