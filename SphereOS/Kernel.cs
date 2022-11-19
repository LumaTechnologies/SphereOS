using Cosmos.System.Network.IPv4.UDP.DHCP;
using SphereOS.Commands;
using SphereOS.Core;
using SphereOS.Logging;
using SphereOS.Users;
using System;
using Sys = Cosmos.System;

namespace SphereOS
{
    public class Kernel : Sys.Kernel
    {
        public const string Version = "0.1.4 Preview";

        internal static User CurrentUser = null;

        internal static string WorkingDir = @"0:\";

        protected override void BeforeRun()
        {
            try
            {
                Log.Info("Kernel", "Starting SphereOS kernel.");

                Console.Clear();

                ProcessManager.AddProcess(new Core.MemService()).TryStart();

                Util.PrintTask("Initialising commands...");
                CommandManager.RegisterCommands();

                FsManager.Initialise();

                Util.PrintTask("Initialising system...");
                UserManager.Load();

                Util.PrintTask("Starting SphereOS network... (DHCP mode)");
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
                    Log.Warning("Kernel", $"Could not start network: {e.Message}");
                    Util.PrintTask($"Could not start network: {e.Message}");
                }

                Log.Info("Kernel", "SphereOS kernel started.");

                Util.PrintTask("Starting shell...");
                WelcomeMessage();
            }
            catch (Exception e)
            {
                CrashScreen.ShowCrashScreen(e);
            }
        }

        private void WelcomeMessage()
        {
            Util.Print(ConsoleColor.Cyan, "Welcome to ");
            Util.Print(ConsoleColor.Magenta, "SphereOS");
            Util.PrintLine(ConsoleColor.Gray, $" - version {Version}");
            Util.PrintLine(ConsoleColor.White, "Copyright (c) 2022. All rights reserved.");

            Util.Print(ConsoleColor.Yellow, "New in this version: ");
            Util.PrintLine(ConsoleColor.White, "New security, scripting, and more!");
        }

        private void PromptLogin()
        {
            Util.Print(ConsoleColor.Cyan, "Username: ");
            var username = Console.ReadLine().Trim();
            User user = UserManager.GetUser(username);
            if (user != null)
            {
                Util.Print(ConsoleColor.Cyan, $"Password for {username}: ");
                var password = Util.ReadPassword();
                if (user.Authenticate(password))
                {
                    CurrentUser = user;
                    Log.Info("Kernel", $"{user.Username} logged on.");
                    Console.WriteLine();
                    Util.PrintLine(ConsoleColor.Green, $"Welcome to SphereOS!");
                }
                else
                {
                    Util.PrintLine(ConsoleColor.Red, "Incorrect password.");
                    return;
                }
            }
            else
            {
                Util.PrintLine(ConsoleColor.Red, "Unknown user.");
                return;
            }
        }

        protected override void Run()
        {
            try
            {
                if (CurrentUser == null)
                {
                    PromptLogin();
                }
                else
                {
                    Shell.Execute();
                }
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
