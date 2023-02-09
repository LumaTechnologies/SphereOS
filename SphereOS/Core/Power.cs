using SphereOS.Logging;
using SphereOS.Shell;
using SphereOS.Users;
using System.IO;

namespace SphereOS.Core
{
    internal static class Power
    {
        internal static void Shutdown(bool reboot)
        {
            string message = reboot ? "Rebooting..." : "Shutting down...";

            Log.Info("Power", message);
            Util.PrintSystem(message);

            if (SysCfg.BootLock)
            {
                if (File.Exists(@"0:\etc\bootlock.tmp"))
                {
                    File.Delete(@"0:\etc\bootlock.tmp");
                    Log.Info("Power", "bootlock cleared.");
                }
                else
                {
                    Log.Warning("Power", "bootlock is missing.");
                }
            }

            ProcessManager.StopAll();
            SysCfg.Flush();
            UserManager.Flush();

            if (reboot)
            {
                Cosmos.System.Power.Reboot();
            }
            else
            {
                Cosmos.System.Power.Shutdown();
            }
        }
    }
}
