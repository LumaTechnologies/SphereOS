using SphereOS.Logging;
using SphereOS.Shell;
using SphereOS.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SphereOS.Core
{
    internal static class Power
    {
        internal static void Shutdown(bool reboot)
        {
            string message = reboot ? "Rebooting..." : "Shutting down...";

            Log.Info("Power", message);
            Util.PrintTask(message);

            ProcessManager.StopAll();
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
