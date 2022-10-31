using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SphereOS.Commands.ConsoleTopic
{
    internal class Sysinfo : Command
    {
        public Sysinfo() : base("sysinfo")
        {
            Description = "Show system information.";

            Topic = "console";
        }

        internal override ReturnCode Execute(string[] args)
        {
            Util.PrintLine(ConsoleColor.Green, $"System information as of {DateTime.Now.ToString("HH:mm")}");
            Util.Print(ConsoleColor.Cyan, Cosmos.HAL.PCI.Count);
            Util.PrintLine(ConsoleColor.White, " PCI devices");
            Util.PrintLine(ConsoleColor.Cyan, $"{Cosmos.Core.CPU.GetCPUBrandString()} ({Cosmos.Core.CPU.GetAmountOfRAM()} MB memory)");
            return ReturnCode.Success;
        }
    }
}
