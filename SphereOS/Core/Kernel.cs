using SphereOS.Core;
using SphereOS.Logging;
using SphereOS.Users;
using System;
using Sys = Cosmos.System;

namespace SphereOS
{
    public class Kernel : Sys.Kernel
    {
        public const string Version = "0.1.6 Preview";

        internal static User CurrentUser = null;

        protected override void BeforeRun()
        {
            try
            {
                Log.Info("Kernel", "Starting SphereOS kernel.");

                Boot.BootManager.StartAll();

                Log.Info("Kernel", "SphereOS kernel started.");

                Shell.InterfaceChoice.ChooseInterface();
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
