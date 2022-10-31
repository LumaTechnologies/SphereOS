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

namespace ThumbOS
{
    public class Kernel : Sys.Kernel
    {
        public const string Version = "0.1.0";

        private User currentUser = null;

        private Address dnsAddress = new Address(8, 8, 8, 8);

        private Sys.FileSystem.CosmosVFS fs;

        private string workingDir = @"0:\";

        protected override void BeforeRun()
        {
            Console.Clear();

            Util.PrintTask("Initialising system...");
            UserManager.AddUser("admin", "password");

            Util.PrintTask("Initialising file system...");
            fs = new Sys.FileSystem.CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);

            Util.PrintTask("Starting ThumbOS network... (DHCP mode)");
            try
            {
                if (Cosmos.HAL.NetworkDevice.Devices.Count == 0)
                {
                    throw new Exception("No network devices are available.");
                }
                using var dhcp = new DHCPClient();
                dhcp.SendDiscoverPacket();
                //dhcp.Close();
            }
            catch (Exception e) 
            {
                Util.PrintTask($"Could not start network: {e}");
            }

            Util.PrintTask("Starting shell...");
            Util.Print(ConsoleColor.Cyan, "Welcome to ");
            Util.Print(ConsoleColor.Magenta, "ThumbOS");
            Util.PrintLine(ConsoleColor.Gray, $" - version {Version}");
            Util.PrintLine(ConsoleColor.White, "Copyright (c) 2022. All rights reserved.");

            Util.Print(ConsoleColor.Yellow, "New in this version: ");
            Util.PrintLine(ConsoleColor.White, "CloudChat! Run cloudchat to start.");
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
                    Console.Clear();
                    Console.SetCursorPosition(0, 0);
                    Util.PrintLine(ConsoleColor.Red, "Something went wrong while running CloudChat.");
                    Util.PrintLine(ConsoleColor.White, $"Please report this to the developers. Error information: {e}");
                    Cosmos.Core.CPU.Halt();
                }
                return;
            }
            if (currentUser == null)
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
                        currentUser = user;
                        Console.WriteLine();
                        Util.PrintLine(ConsoleColor.Green, $"Welcome to ThumbOS!");
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
                Util.Print(ConsoleColor.Cyan, currentUser.Username);
                Util.Print(ConsoleColor.Gray, @$"@thumbos [{workingDir}]> ");
                var input = Console.ReadLine();
                if (input.Trim() == string.Empty)
                    return;
                var args = input.Trim().Split();
                var command = args[0];
                try
                {
                    switch (command)
                    {
                        case "about":
                            Util.Print(ConsoleColor.Magenta, "ThumbOS");
                            Util.PrintLine(ConsoleColor.Gray, $" - version {Version}");
                            Util.PrintLine(ConsoleColor.White, "Copyright (c) 2022. All rights reserved.");
                            break;
                        case "help":
                            Help.Main(args);
                            break;
                        case "date":
                            Util.PrintLine(ConsoleColor.White, DateTime.Now.ToString("dddd, dd/MM/yyyy HH:mm:ss"));
                            break;
                        case "logout":
                            Util.PrintLine(ConsoleColor.Green, "Goodbye!");
                            currentUser = null;
                            break;
                        case "shutdown":
                            Util.PrintLine(ConsoleColor.Green, "Goodbye!");
                            Util.PrintTask("Shutting down...");
                            Sys.Power.Shutdown();
                            break;
                        case "reboot":
                            Util.PrintLine(ConsoleColor.Green, "Goodbye!");
                            Util.PrintTask("Rebooting...");
                            Sys.Power.Reboot();
                            break;
                        case "resolve":
                            if (args.Length < 2)
                            {
                                Util.PrintLine(ConsoleColor.Red, "Invalid usage. Please provide a domain.");
                                return;
                            }
                            var domain = args[1];
                            Util.PrintTask($"Resolving {domain}...");
                            using (var dns = new DnsClient())
                            {
                                dns.Connect(dnsAddress);
                                dns.SendAsk(domain);
                                Address destination = dns.Receive();
                                if (destination != null)
                                {
                                    Util.PrintLine(ConsoleColor.Green, $"{domain} is located at: {destination.ToString()}");
                                }
                                else
                                {
                                    Util.PrintLine(ConsoleColor.Red, $"Unable to resolve {domain}.");
                                }
                            }
                            break;
                        case "sysinfo":
                            Util.PrintLine(ConsoleColor.Green, $"System information as of {DateTime.Now.ToString("HH:mm")}");
                            Util.Print(ConsoleColor.Cyan, PCI.Count);
                            Util.PrintLine(ConsoleColor.White, " PCI devices");
                            Util.PrintLine(ConsoleColor.Cyan, $"{Cosmos.Core.CPU.GetCPUBrandString()} ({Cosmos.Core.CPU.GetAmountOfRAM()} MB memory)");
                            break;
                        case "clear":
                            Console.Clear();
                            break;
                        case "lock":
                            bool authenticated = false;
                            while (!authenticated)
                            {
                                Console.Clear();
                                Util.Print(ConsoleColor.Gray, "[ThumbOS] ");
                                Util.PrintLine(ConsoleColor.Cyan, "This PC is locked.");
                                Util.PrintLine(ConsoleColor.Cyan, $"Enter the password for {currentUser.Username}: ");
                                var password = Util.ReadPassword();
                                authenticated = currentUser.Authenticate(password);
                            }
                            break;
                        case "clock":
                            Clock.Main(args);
                            break;
                        case "adduser":
                            if (args.Length < 2)
                            {
                                Util.PrintLine(ConsoleColor.Red, "Invalid usage. Please provide a new username.");
                                return;
                            }
                            var username = args[1].Trim();
                            if (UserManager.GetUser(username) != null)
                            {
                                Util.PrintLine(ConsoleColor.Red, $"A user named {username} already exists.");
                                return;
                            }
                            Util.Print(ConsoleColor.Cyan, $"New password for {username}: ");
                            var newPassword = Util.ReadPassword();
                            UserManager.AddUser(username, newPassword);
                            Util.PrintLine(ConsoleColor.Green, $"Successfully created user {username}.");
                            break;
                        case "cloudchat":
                            CloudChat.Init();
                            break;
                        case "cd":
                            if (args.Length < 2)
                            {
                                Util.PrintLine(ConsoleColor.Red, "Invalid usage. Please provide the path to navigate to.");
                                return;
                            }
                            workingDir = Path.Join(workingDir, args[1]);
                            break;
                        case "ls":
                            foreach (var file in Directory.GetFiles(workingDir))
                            {
                                Util.Print(ConsoleColor.Cyan, Path.GetFileName(file) + " ");
                            }
                            foreach (var dir in Directory.GetDirectories(workingDir))
                            {
                                Util.Print(ConsoleColor.Green, Path.GetFileName(dir) + " ");
                            }
                            Console.WriteLine();
                            break;
                        case "fsinfo":
                            var root = Directory.GetDirectoryRoot(workingDir);
                            Util.PrintLine(ConsoleColor.Green, $"Volume information for {root}");
                            Util.Print(ConsoleColor.Cyan, "Label: ");
                            Util.PrintLine(ConsoleColor.White, fs.GetFileSystemLabel(root));
                            Util.Print(ConsoleColor.Cyan, "Total size: ");
                            Util.PrintLine(ConsoleColor.White, fs.GetTotalSize(root));
                            Util.Print(ConsoleColor.Cyan, "Available free space: ");
                            Util.PrintLine(ConsoleColor.White, fs.GetAvailableFreeSpace(root));
                            Util.Print(ConsoleColor.Cyan, "File system: ");
                            Util.PrintLine(ConsoleColor.White, fs.GetFileSystemType(root));
                            break;
                        case "edit":
                            if (args.Length < 2)
                            {
                                Util.PrintLine(ConsoleColor.Red, "Invalid usage. Please provide the name of the file to edit.");
                                return;
                            }
                            string editPath = Path.Join(workingDir, args[1]);
                            TextEditor textEditor = new TextEditor(editPath);
                            textEditor.Start();
                            break;
                        default:
                            Util.PrintLine(ConsoleColor.Red, $"Unknown command '{command}'.");
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Clear();
                    Console.SetCursorPosition(0, 0);
                    Util.PrintLine(ConsoleColor.Red, $"Something went wrong while running '{command}'.");
                    Util.PrintLine(ConsoleColor.White, $"Please report this to the developers. Error information: {e.Message}");
                }
            }
            Cosmos.Core.Memory.Heap.Collect();
        }
    }
}
