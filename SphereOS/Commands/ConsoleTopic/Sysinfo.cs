using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

        private const string asciiArt = "    ____\n  .XXXxxx.\n.XXXxx+++--.\nXXxx++--..  \nXXx++-..   .\n`Xx+-.      \n  `X+.   .\n     \"\"";

        private const int itemTitleWidth = 6;

        private readonly (int Left, int Top) infoOffset = (2, 1);

        private void Display(List<(string Title, string Value)> items)
        {
            (int Left, int Top) startingPos = Console.GetCursorPosition();
            int longestAsciiLine = 0;
            foreach (var line in asciiArt.Split('\n'))
            {
                longestAsciiLine = Math.Max(longestAsciiLine, line.Length);
            }

            (int Left, int Top) infoStartingPos = (longestAsciiLine + infoOffset.Left, startingPos.Top + infoOffset.Top);

            Util.PrintLine(ConsoleColor.White, asciiArt);
            (int Left, int Top) endingPos = Console.GetCursorPosition();

            var y = infoStartingPos.Top;
            Console.SetCursorPosition(infoStartingPos.Left, y);
            Util.Print(ConsoleColor.Green, "SphereOS");
            y += 2;

            foreach (var item in items)
            {
                Console.SetCursorPosition(infoStartingPos.Left, y);
                Util.Print(ConsoleColor.Cyan, item.Title + new string(' ', itemTitleWidth - item.Title.Length));
                Util.Print(ConsoleColor.White, item.Value);
                y++;
            }

            Console.SetCursorPosition(endingPos.Left, endingPos.Top);
        }

        internal override ReturnCode Execute(string[] args)
        {
            uint memTotal = Cosmos.Core.CPU.GetAmountOfRAM();
            uint memUsed = Cosmos.Core.GCImplementation.GetUsedRAM() / 1024 / 1024;
            uint memFree = memTotal - memUsed;
            uint memPercentUsed = (uint)((float)memUsed / memTotal * 100f);

            List<(string Title, string Value)> items = new()
            {
                ("OS", $"SphereOS {Kernel.Version}"),

                ("RAM", string.Format(
                    "{0:d1} MB / {1} MB ({2:d1}%) memory used, {3} MB free",
                    memUsed,
                    memTotal,
                    memPercentUsed,
                    memFree
                )),

                ("CPU", Cosmos.Core.CPU.GetCPUBrandString())
            };

            Display(items);

            return ReturnCode.Success;
        }
    }
}
