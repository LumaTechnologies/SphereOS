using Cosmos.Core;
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
            Util.PrintLine(ConsoleColor.Cyan, $"{Cosmos.Core.CPU.GetCPUBrandString()}");

            uint memTotal = Cosmos.Core.CPU.GetAmountOfRAM();
            uint memUsed = (uint)(memTotal - GCImplementation.GetAvailableRAM());
            uint memFree = memTotal - memUsed;

            uint memPercentUsed = ((uint)(((float)memUsed / (float)memTotal) * 100f));

            Util.PrintLine(ConsoleColor.White, $"{memUsed} MB / {memTotal} MB ({memPercentUsed}%) memory used, {memFree} MB free");

            return ReturnCode.Success;
        }
    }
}
