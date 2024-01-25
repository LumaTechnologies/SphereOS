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
        private static string[] asciiArt = new[] { "    ____", "  .XXXxxx.", ".XXXxx+++--.", "XXxx++--..  ", "XXx++-..   .", "`Xx+-.      ", "  `X+.   .", "     \"\"" };

        private static void PrintOption(string text, bool selected)
        {
            Console.SetCursorPosition(1, Console.GetCursorPosition().Top);

            Console.BackgroundColor = selected ? ConsoleColor.White : ConsoleColor.Black;
            Console.ForegroundColor = selected ? ConsoleColor.Black : ConsoleColor.White;

            Console.WriteLine(text);
        }

        private static void PrintArt()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Cyan;

            for (int i = 0; i < asciiArt.Length; i++)
            {
                Console.SetCursorPosition(Console.WindowWidth - 12, i);

                Console.Write(asciiArt[i]);
            }
        }

        private static void Render(int selIdx)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.SetCursorPosition(0, 0);

            uint mem = Cosmos.Core.CPU.GetAmountOfRAM();
            Console.WriteLine($"SphereOS Version {Kernel.Version} [{mem} MB memory]\nSelect an option:\n");

            PrintOption("SphereOS (GUI)", selIdx == 0);
            PrintOption("SphereOS (CLI)", selIdx == 1);
            PrintOption("Shut Down", selIdx == 2);
            PrintOption("Restart", selIdx == 3);
        }

        private static BootMode Confirm(int selIdx)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            Console.Clear();

            Console.SetCursorPosition(0, 0);

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
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            Console.Clear();

            PrintArt();

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
