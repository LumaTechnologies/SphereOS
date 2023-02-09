using SphereOS.Core;
using System;

namespace SphereOS.Shell
{
    internal static class InterfaceChoice
    {
        private static void PrintChoices()
        {
            Util.PrintSystem("Select an interface.");
            Util.PrintLine(ConsoleColor.Cyan, "[1] Graphical");
            Util.PrintLine(ConsoleColor.Cyan, "[2] Console");
        }

        internal static void ChooseInterface()
        {
            PrintChoices();

            while (true)
            {
                switch (Console.ReadKey(true).KeyChar)
                {
                    case '1':
                        bool result = Gui.Gui.StartGui();
                        if (result)
                        {
                            return;
                        }
                        else
                        {
                            PrintChoices();
                            continue;
                        }
                    case '2':
                        ProcessManager.AddProcess(new Shell()).Start();
                        return;
                }
            }
        }
    }
}
