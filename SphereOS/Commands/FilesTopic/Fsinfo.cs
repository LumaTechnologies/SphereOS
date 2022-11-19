using SphereOS.Core;
using System;
using System.IO;

namespace SphereOS.Commands.FilesTopic
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

            long totalSize = FsManager.Fs.GetTotalSize(root) / 1024 / 1024;
            long freeSpace = FsManager.Fs.GetAvailableFreeSpace(root) / 1024 / 1024;
            long usedSpace = totalSize - freeSpace;

            Util.Print(ConsoleColor.Cyan, "Total size: ");
            Util.PrintLine(ConsoleColor.White, $"{totalSize} MB");

            Util.Print(ConsoleColor.Cyan, "Available free space: ");
            Util.PrintLine(ConsoleColor.White, $"{freeSpace} MB");

            Util.Print(ConsoleColor.Cyan, "Used space: ");
            Util.PrintLine(ConsoleColor.White, $"{usedSpace} MB");

            Util.Print(ConsoleColor.Cyan, "File system: ");
            Util.PrintLine(ConsoleColor.White, FsManager.Fs.GetFileSystemType(root));

            return ReturnCode.Success;
        }
    }
}
