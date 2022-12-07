using SphereOS.Core;
using SphereOS.Gui.UILib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SphereOS.Gui
{
    internal static class Gui
    {
        internal static void StartGui()
        {
            Console.Clear();
            Console.CursorVisible = false;

            var wm = new WindowManager();

            AppManager.LoadAllApps();

            ProcessManager.AddProcess(wm);

            ProcessManager.AddProcess(wm, new SettingsService()).Start();

            wm.Start();

            ProcessManager.AddProcess(wm, new Sound.SoundService()).Start();

            ProcessManager.AddProcess(wm, new ShellComponents.Lock()).Start();

            if (Cosmos.Core.CPU.GetAmountOfRAM() < 1000)
            {
                MessageBox messageBox = new MessageBox(null, "Memory Warning", "SphereOS is running with a low\namount of memory.\n\nAt least 1 GB of system memory\nshould be allocated for optimal\nperformance and stability.");
                messageBox.Show();
            }
        }
    }
}
