using SphereOS.Core.Memory;
using SphereOS.Shell;
using System;
using System.Collections.Generic;

namespace SphereOS.Commands.GeneralTopic
{
    internal class Sysinfo : Command
    {
        public Sysinfo() : base("sysinfo")
        {
            Description = "Show system information.";

            Topic = "general";
        }

        private const string asciiArt = "    ____\n  .XXXxxx.\n.XXXxx+++--.\nXXxx++--..  \nXXx++-..   .\n`Xx+-.      \n  `X+.   .\n     \"\"";

        private const int itemTitleWidth = 6;

        private readonly (int Left, int Top) infoOffset = (2, 1);

        private void Display(List<(string Title, string Value)> items)
        {
            string[] asciiSplit = asciiArt.Split('\n');

            int longestAsciiLine = 0;
            int asciiLines = asciiSplit.Length;
            foreach (var line in asciiSplit)
            {
                longestAsciiLine = Math.Max(longestAsciiLine, line.Length);
            }

            Util.PrintLine(ConsoleColor.Cyan, asciiArt);

            (int Left, int Top) endingPos = Console.GetCursorPosition();
            (int Left, int Top) startingPos = (endingPos.Left, endingPos.Top - asciiLines);
            (int Left, int Top) infoStartingPos = (longestAsciiLine + infoOffset.Left, startingPos.Top + infoOffset.Top);

            var y = infoStartingPos.Top;
            Console.SetCursorPosition(infoStartingPos.Left, y);
            Util.Print(ConsoleColor.White, "SphereOS");
            y += 2;

            foreach (var item in items)
            {
                Console.SetCursorPosition(infoStartingPos.Left, y);
                Util.Print(ConsoleColor.Magenta, item.Title + new string(' ', itemTitleWidth - item.Title.Length));
                Util.Print(ConsoleColor.White, item.Value);
                y++;
            }

            Console.SetCursorPosition(endingPos.Left, endingPos.Top);
        }

        internal override ReturnCode Execute(string[] args)
        {
            var statistics = MemoryStatisticsProvider.GetMemoryStatistics();

            List<(string Title, string Value)> items = new()
            {
                ("OS", $"SphereOS - {Kernel.Version}"),

                ("MEM", string.Format(
                    "{0:d1} MB / {1} MB ({2:d1}%) ({3} MB free)",
                    statistics.UsedMB,
                    statistics.TotalMB,
                    statistics.PercentUsed,
                    statistics.FreeMB
                )),

                ("CPU", Cosmos.Core.CPU.GetCPUBrandString())
            };

            Display(items);

            return ReturnCode.Success;
        }
    }
}
