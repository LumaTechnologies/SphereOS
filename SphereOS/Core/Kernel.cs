using SphereOS.Core;
using SphereOS.Logging;
using SphereOS.Users;
using System;
using Sys = Cosmos.System;
using SphereOS.Shell;

namespace SphereOS
{
    public class Kernel : Sys.Kernel
    {
        public const string Version = "0.1.4 Preview";
        
        internal static User CurrentUser = null;

        protected override void BeforeRun()
        {
            try
            {
                Log.Info("Kernel", "Starting SphereOS kernel.");

                Boot.BootManager.StartAll();

                Log.Info("Kernel", "SphereOS kernel started.");

                Util.PrintTask("Starting shell...");
                Shell.Shell.WelcomeMessage();
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
                Shell.Shell.Execute();
                ProcessManager.Yield();
                ProcessManager.Sweep();
            }
            catch (Exception e)
            {
                CrashScreen.ShowCrashScreen(e);
            }
        }
    }
}
