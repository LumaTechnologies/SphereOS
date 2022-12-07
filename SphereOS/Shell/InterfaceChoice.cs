using SphereOS.Core;
using System;

namespace SphereOS.Shell
{
    internal static class InterfaceChoice
    {
        internal static void ChooseInterface()
        {
            Util.PrintTask("Which interface would you like to use?");
            Util.PrintLine(ConsoleColor.Cyan, "[1] Graphical");
            Util.PrintLine(ConsoleColor.Cyan, "[2] Console");

            while (true)
            {
                switch (Console.ReadKey(true).KeyChar)
                {
                    case '1':
                        Gui.Gui.StartGui();
                        return;
                    case '2':
                        ProcessManager.AddProcess(new Shell()).Start();
                        return;
                }
            }
        }
    }
}
