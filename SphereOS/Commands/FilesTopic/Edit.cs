using SphereOS.ConsoleApps;
using SphereOS.Core;
using SphereOS.Shell;
using System;
using System.IO;

namespace SphereOS.Commands.FilesTopic
{
    internal class Edit : Command
    {
        public Edit() : base("edit")
        {
            Description = "Edit a file.";

            Usage = "<file>";

            Topic = "files";
        }

        internal override ReturnCode Execute(string[] args)
        {
            if (args.Length != 2)
            {
                Util.PrintLine(ConsoleColor.Red, "Invalid usage. Please provide the name of the file to edit.");
                return ReturnCode.Invalid;
            }

            string editPath = Path.Join(Shell.Shell.CurrentShell.WorkingDir, args[1]);

            /*if (!FileSecurity.CanAccess(Kernel.CurrentUser, editPath))
            {
                Util.PrintLine(ConsoleColor.Red, "You do not have permission to access this file.");
                return ReturnCode.Unauthorised;
            }*/

            TextEditor textEditor = new TextEditor(editPath);
            textEditor.Start();

            return ReturnCode.Success;
        }
    }
}
