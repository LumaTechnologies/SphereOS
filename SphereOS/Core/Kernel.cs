using SphereOS.Core;
using SphereOS.Logging;
using SphereOS.Shell;
using SphereOS.Users;
using System;
using Sys = Cosmos.System;

namespace SphereOS
{
    public class Kernel : Sys.Kernel
    {
        public const string Version = "0.2.3";

        internal static User CurrentUser = null;

        internal static Random Random { get; } = new Random();

        protected override void BeforeRun()
        {
            try
            {
                BootMode bootMode = BootMenu.ChooseBootMode();

                Log.Info("Kernel", "Starting SphereOS.");

                Boot.BootManager.StartAll();

                Log.Info("Kernel", "SphereOS started.");

                switch (bootMode)
                {
                    case BootMode.Gui:
                        Gui.Gui.StartGui();
                        break;
                    case BootMode.Console:
                        ProcessManager.AddProcess(new Shell.Shell()).Start();
                        break;
                    default:
                        CrashScreen.ShowCrashScreen(new Exception("Unexpected boot mode."));
                        break;
                }
            }
            catch (Exception e)
            {
                CrashScreen.ShowCrashScreen(e);
            }
        }

        protected override void Run()
        {
            try
            {
                ProcessManager.Yield();
            }
            catch (Exception e)
            {
                CrashScreen.ShowCrashScreen(e);
            }
        }
    }
}
