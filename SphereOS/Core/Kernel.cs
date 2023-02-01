using SphereOS.Core;
using SphereOS.Logging;
using SphereOS.Shell;
using SphereOS.Users;
using System;
using System.Collections.Generic;
using Sys = Cosmos.System;

namespace SphereOS
{
    public class Kernel : Sys.Kernel
    {
        public const string Version = "0.2.0";

        internal static User CurrentUser = null;

        internal static Random Random { get; } = new Random();

        protected override void BeforeRun()
        {
            try
            {
                Log.Info("Kernel", "Starting SphereOS.");

                Boot.BootManager.StartAll();

                Log.Info("Kernel", "SphereOS started.");

                InterfaceChoice.ChooseInterface();
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
