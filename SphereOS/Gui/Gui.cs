using SphereOS.Core;
using SphereOS.Gui.UILib;
using SphereOS.Logging;
using SphereOS.Shell;
using SphereOS.Users;
using System;

namespace SphereOS.Gui
{
    internal static class Gui
    {
        internal static bool StartGui()
        {
            Console.Clear();
            Console.CursorVisible = false;

            var wm = new WindowManager();

            Log.Info("Gui", "GUI starting.");

            if (Cosmos.Core.CPU.GetAmountOfRAM() < 1000)
            {
                Util.PrintWarning("Not enough system memory is available to run the GUI. At least 1 GB should be allocated.");
                Util.PrintSystem("Continue anyway? [y/N]");

                if (Console.ReadKey(true).Key != ConsoleKey.Y)
                {
                    return false;
                }
            }

            AppManager.LoadAllApps();

            ProcessManager.AddProcess(wm);

            ProcessManager.AddProcess(wm, new SettingsService()).Start();

            wm.Start();

            ProcessManager.AddProcess(wm, new Sound.SoundService()).Start();

            ProcessManager.AddProcess(wm, new ShellComponents.Lock()).Start();

            return true;
        }
    }
}
