using Cosmos.HAL;
using SphereOS.Core;
using System;

namespace SphereOS.Shell
{
    internal enum BootMode
    {
        Gui,
        Console,
        Cancel
    }

    internal static class BootMenu
    {
        private static void PrintOption(string text, bool selected)
        {
            Console.BackgroundColor = selected ? ConsoleColor.White : ConsoleColor.Blue;
            Console.ForegroundColor = selected ? ConsoleColor.Blue  : ConsoleColor.White;

            Console.WriteLine(text);
        }

        private static void Render(int selIdx)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;

            Console.CursorLeft = 0;
            Console.CursorTop = 0;

            uint mem = Cosmos.Core.CPU.GetAmountOfRAM();
            Console.WriteLine($"Sphere Systems SphereOS Version {Kernel.Version} Boot Manager [{mem} MB memory]\n\nPlease select an option:\n");

            PrintOption("SphereOS (VMware GUI)", selIdx == 0);
            PrintOption("SphereOS (Console)", selIdx == 1);
            PrintOption("Shut Down", selIdx == 2);
            PrintOption("Restart", selIdx == 3);
        }

        private static BootMode Confirm(int selIdx)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            Console.Clear();

            Console.CursorLeft = 0;
            Console.CursorTop = 0;

            Console.CursorVisible = true;

            switch (selIdx)
            {
                case 0:
                    return BootMode.Gui;
                case 1:
                    return BootMode.Console;
                case 2:
                    Cosmos.System.Power.Shutdown();
                    return BootMode.Cancel;
                case 3:
                    Cosmos.System.Power.Reboot();
                    return BootMode.Cancel;
                default:
                    return BootMode.Cancel;
            }
        }

        internal static BootMode ChooseBootMode()
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;

            Console.Clear();

            Console.CursorVisible = false;

            int selIdx = 0;

            while (true)
            {
                Render(selIdx);

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.Enter:
                        return Confirm(selIdx);
                    case ConsoleKey.DownArrow:
                        selIdx++;
                        break;
                    case ConsoleKey.UpArrow:
                        selIdx--;
                        break;
                }

                if (selIdx < 0)
                {
                    selIdx = 3;
                }

                if (selIdx > 3)
                {
                    selIdx = 0;
                }
            }
        }
    }
}
