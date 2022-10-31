using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThumbOS.Commands.FilesTopic
{
    internal class Edit : Command
    {
        public Edit() : base("edit")
        {
            Description = "Edit a file.";

            Topic = "files";
        }

        internal override ReturnCode Execute(string[] args)
        {
            if (args.Length != 2)
            {
                Util.PrintLine(ConsoleColor.Red, "Invalid usage. Please provide the name of the file to edit.");
                return ReturnCode.Invalid;
            }

            string editPath = Path.Join(Kernel.WorkingDir, args[1]);

            TextEditor textEditor = new TextEditor(editPath);
            textEditor.Start();

            return ReturnCode.Success;
        }
    }
}
