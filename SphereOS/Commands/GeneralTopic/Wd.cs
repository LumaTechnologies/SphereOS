using System;

namespace SphereOS.Commands.GeneralTopic
{
    internal class Wd : Command
    {
        public Wd() : base("wd")
        {
            Description = "Show the full working directory.";

            Topic = "general";
        }

        internal override ReturnCode Execute(string[] args)
        {
            Shell.Shell shell = Shell.Shell.CurrentShell;
            Console.WriteLine(shell.WorkingDir);
            return ReturnCode.Success;
        }
    }
}
