using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;
using Cosmos.System.Network;
using Cosmos.HAL;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.UDP.DHCP;
using Cosmos.System.Network.IPv4.UDP.DNS;
using System.IO;
using SphereOS.Commands;

namespace SphereOS
{
    public class Kernel : Sys.Kernel
    {
        public const string Version = "0.1.2 Preview";

        internal static User CurrentUser = null;

        internal static string WorkingDir = @"0:\";

        protected override void BeforeRun()
        {
            Console.Clear();

            Util.PrintTask("Initialising commands...");
            CommandManager.RegisterCommands();

            Util.PrintTask("Initialising system...");
            UserManager.AddUser("admin", "password", admin: true);

            FsManager.Initialise();

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
                Util.PrintTask($"Could not start network: {e}");
            }

            Util.PrintTask("Starting shell...");
            WelcomeMessage();
        }

        private void WelcomeMessage()
        {
            Util.Print(ConsoleColor.Cyan, "Welcome to ");
            Util.Print(ConsoleColor.Magenta, "SphereOS");
            Util.PrintLine(ConsoleColor.Gray, $" - version {Version}");
            Util.PrintLine(ConsoleColor.White, "Copyright (c) 2022. All rights reserved.");

            Util.Print(ConsoleColor.Yellow, "New in this version: ");
            Util.PrintLine(ConsoleColor.White, "New sysinfo command!");
        }

        protected override void Run()
        {
            if (CloudChat.Running)
            {
                try
                {
                    CloudChat.Loop();
                }
                catch (Exception e)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Clear();
                    Console.SetCursorPosition(0, 0);
                    Util.PrintLine(ConsoleColor.Red, "Something went wrong while running CloudChat.");
                    Util.PrintLine(ConsoleColor.White, $"Please report this to the developers. Error information: {e}");
                    Cosmos.Core.CPU.Halt();
                }
                return;
            }
            if (CurrentUser == null)
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
            else
            {
                Util.Print(ConsoleColor.Cyan, CurrentUser.Username);
                Util.Print(ConsoleColor.Gray, @$"@SphereOS [{WorkingDir}]> ");

                var input = Console.ReadLine();

                if (input.Trim() == string.Empty)
                    return;

                var args = input.Trim().Split();
                var commandName = args[0];

                Command command = CommandManager.GetCommand(commandName);
                if (command != null)
                {
                    try
                    {
                        command.Execute(args);
                    }
                    catch (Exception e)
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        Util.PrintLine(ConsoleColor.Red, $"Something went wrong while running '{commandName}'.");
                        Util.PrintLine(ConsoleColor.White, $"Please report this to the developers. Error information: {e.Message}");
                    }
                }
                else
                {
                    Util.PrintLine(ConsoleColor.Red, $"Unknown command '{commandName}'.");
                }
            }
            Cosmos.Core.Memory.Heap.Collect();
        }
    }
}
