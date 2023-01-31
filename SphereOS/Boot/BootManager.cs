using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using Cosmos.System.Network.IPv4.UDP.DHCP;
using SphereOS.Commands;
using SphereOS.Core;
using SphereOS.Logging;
using SphereOS.Shell;
using SphereOS.Users;
using System;

namespace SphereOS.Boot
{
    internal static class BootManager
    {
        private static void Fail(string reason)
        {
            Log.Info("BootManager", $"Boot failed: {reason}");

            Util.PrintTask("Error: Boot failed!");
            Util.PrintLine(ConsoleColor.Red, reason);
            Util.PrintLine(ConsoleColor.Cyan, "SphereOS has failed to boot. Press any key to reboot.");

            System.Console.ReadKey(true);
            Power.Shutdown(reboot: true);
        }

        private static void Console()
        {
            System.Console.Clear();

            Log.Info("BootManager", "Loading VGA font...");

            PCScreenFont font = PCScreenFont.Default;
            VGAScreen.SetFont(font.CreateVGAFont(), font.Height);
        }

        private static void SysInit()
        {
            Util.PrintTask("Initialising system...");
            ProcessManager.AddProcess(new Core.MemService()).TryStart();

            CommandManager.RegisterCommands();

            if (!FsManager.Initialise())
            {
                // Preprocessor option to allow FsManager failure.
#if false
                Util.PrintWarning("File system initialisation failed.");
#else
                Fail("FsManager initialisation failure.");
#endif
            }

            UserManager.Load();
        }

        private static void Net()
        {
            Util.PrintTask("Starting network...");
            try
            {
                if (Cosmos.HAL.NetworkDevice.Devices.Count == 0)
                {
                    throw new Exception("No network devices are available.");
                }
                using var dhcp = new DHCPClient();
                dhcp.SendDiscoverPacket();
            }
            catch (Exception e)
            {
                Log.Warning("BootManager", $"Could not start network: {e.ToString()}");
                Util.PrintTask($"Could not start network: {e.ToString()}");
            }
        }

        internal static void StartAll()
        {
            Log.Info("BootManager", "Starting boot phase Console.");
            Console();
            Log.Info("BootManager", "Starting boot phase SysInit.");
            SysInit();
            Log.Info("BootManager", "Starting boot phase Net.");
            Net();
        }
    }
}
