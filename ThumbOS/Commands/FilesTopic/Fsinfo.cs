using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sphereOS.Commands.FilesTopic
{
    internal class Fsinfo : Command
    {
        public Fsinfo() : base("fsinfo")
        {
            Description = "Show volume information.";

            Topic = "files";
        }

        internal override ReturnCode Execute(string[] args)
        {
            var root = Directory.GetDirectoryRoot(Kernel.WorkingDir);

            Util.PrintLine(ConsoleColor.Green, $"Volume information for {root}");

            Util.Print(ConsoleColor.Cyan, "Label: ");
            Util.PrintLine(ConsoleColor.White, FsManager.Fs.GetFileSystemLabel(root));

            Util.Print(ConsoleColor.Cyan, "Total size: ");
            Util.PrintLine(ConsoleColor.White, FsManager.Fs.GetTotalSize(root));

            Util.Print(ConsoleColor.Cyan, "Available free space: ");
            Util.PrintLine(ConsoleColor.White, FsManager.Fs.GetAvailableFreeSpace(root));

            Util.Print(ConsoleColor.Cyan, "File system: ");
            Util.PrintLine(ConsoleColor.White, FsManager.Fs.GetFileSystemType(root));

            return ReturnCode.Success;
        }
    }
}
