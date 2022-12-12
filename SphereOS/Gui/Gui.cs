using SphereOS.Core;
using SphereOS.Gui.UILib;
using SphereOS.Logging;
using System;

namespace SphereOS.Gui
{
    internal static class Gui
    {
        internal static void StartGui()
        {
            Console.Clear();
            Console.CursorVisible = false;

            var wm = new WindowManager();

            Log.Info("Gui", "GUI starting.");

            AppManager.LoadAllApps();

            ProcessManager.AddProcess(wm);

            ProcessManager.AddProcess(wm, new SettingsService()).Start();

            wm.Start();

            ProcessManager.AddProcess(wm, new Sound.SoundService()).Start();

            ProcessManager.AddProcess(wm, new ShellComponents.Lock()).Start();

            if (Cosmos.Core.CPU.GetAmountOfRAM() < 1000)
            {
                Log.Warning("Gui", "Less than 1 GB of memory is allocated to the GUI.");

                MessageBox messageBox = new MessageBox(null, "Memory Warning", "SphereOS is running with a low\namount of memory.\n\nAt least 1 GB of system memory\nshould be allocated for optimal\nperformance and stability.");
                messageBox.Show();
            }
        }
    }
}
