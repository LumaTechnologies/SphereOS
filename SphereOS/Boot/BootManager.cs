using Cosmos.System.Graphics.Fonts;
using Cosmos.System.Graphics;
using SphereOS.Core;
using SphereOS.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Cosmos.System.Network.IPv4.UDP.DHCP;
using SphereOS.Shell;
using SphereOS.Users;
using SphereOS.Commands;

namespace SphereOS.Boot
{
    internal static class BootManager
    {
        private static void Console()
        {
            System.Console.Clear();

            Log.Info("BootManager", "Loading font...");

            PCScreenFont font = PCScreenFont.Default;
            VGAScreen.SetFont(font.CreateVGAFont(), font.Height);
        }

        private static void SysInit()
        {
            ProcessManager.AddProcess(new Core.MemService()).TryStart();

            Util.PrintTask("Initialising commands...");
            CommandManager.RegisterCommands();

            FsManager.Initialise();

            Util.PrintTask("Initialising system...");
            UserManager.Load();
        }

        private static void Net()
        {
            Util.PrintTask("Starting SphereOS network...");
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
