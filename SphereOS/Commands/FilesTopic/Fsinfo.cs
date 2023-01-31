using SphereOS.Core;
using SphereOS.Shell;
using System;
using System.IO;

namespace SphereOS.Commands.FilesTopic
{
    internal class Fsinfo : Command
    {
        public Fsinfo() : base("fsinfo")
        {
            Description = "Show filesystem information.";

            Topic = "files";
        }

        internal override ReturnCode Execute(string[] args)
        {
            var root = Directory.GetDirectoryRoot(Shell.Shell.CurrentShell.WorkingDir);

            long totalSize = FsManager.Fs.GetTotalSize(root) / 1024 / 1024;
            long freeSpace = FsManager.Fs.GetAvailableFreeSpace(root) / 1024 / 1024;
            long usedSpace = totalSize - freeSpace;

            string label = FsManager.Fs.GetFileSystemLabel(root);
            string fsType = FsManager.Fs.GetFileSystemType(root);

            Util.PrintLine(ConsoleColor.Green, $"Volume information for {root}");

            Util.Print(ConsoleColor.Cyan, "Label: ");
            Util.PrintLine(ConsoleColor.White, label);

            Util.Print(ConsoleColor.Cyan, "Total size: ");
            Util.PrintLine(ConsoleColor.White, $"{totalSize} MB");

            Util.Print(ConsoleColor.Cyan, "Available free space: ");
            Util.PrintLine(ConsoleColor.White, $"{freeSpace} MB");

            Util.Print(ConsoleColor.Cyan, "Used space: ");
            Util.PrintLine(ConsoleColor.White, $"{usedSpace} MB");

            Util.Print(ConsoleColor.Cyan, "File system: ");
            Util.PrintLine(ConsoleColor.White, fsType);

            return ReturnCode.Success;
        }
    }
}
