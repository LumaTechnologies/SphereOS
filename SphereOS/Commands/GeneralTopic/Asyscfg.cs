using SphereOS.Core;
using SphereOS.Shell;
using System;

namespace SphereOS.Commands.GeneralTopic
{
    internal class Asyscfg : Command
    {
        public Asyscfg() : base("asyscfg")
        {
            Description = "Apply SysCfg changes.";

            Topic = "general";
        }

        internal override ReturnCode Execute(string[] args)
        {
            SysCfg.Load();

            Util.PrintLine(ConsoleColor.Green, "SysCfg applied.");

            return ReturnCode.Success;
        }
    }
}
