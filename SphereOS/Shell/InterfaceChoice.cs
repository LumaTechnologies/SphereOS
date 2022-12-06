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
                        Console.Clear();
                        Console.CursorVisible = false;
                        var wm = new Gui.WindowManager();
                        Gui.AppManager.LoadAllApps();
                        ProcessManager.AddProcess(wm).Start();
                        ProcessManager.AddProcess(wm, new Gui.SettingsService()).Start();
                        ProcessManager.AddProcess(wm, new Gui.Sound.SoundService()).Start();
                        ProcessManager.AddProcess(wm, new Gui.ShellComponents.Lock()).Start();
                        return;
                    case '2':
                        ProcessManager.AddProcess(new Shell()).Start();
                        return;
                }
            }
        }
    }
}
