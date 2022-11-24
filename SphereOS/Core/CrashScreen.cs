using Cosmos.Core;
using Cosmos.HAL;
using SphereOS.Commands;
using SphereOS.Logging;
using SphereOS.Users;
using System;

namespace SphereOS.Core
{
    internal static class CrashScreen
    {
        private static string messageLoading = @"
 :(  SphereOS Crash

 Generating crash log, please wait...";

        private static string message = @"
 :(  SphereOS Crash

 Something went wrong, and SphereOS must reboot.

 SphereOS has generated a crash log, which you can view by pressing 'V'.

                                             [V] View Log   [Esc] Reboot";

        private static string log = string.Empty;

        private static void ShowMessageFullScreen(string text)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = false;

            Console.Write(text);
        }

        private static void ShowCrashLog()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Clear();
            Console.SetCursorPosition(0, 0);

            Console.WriteLine("[i] Press any key to go back.");
            Console.WriteLine("Crash log:");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(log);

            Console.ReadKey(true);
        }

        private static void GenerateCrashLog(Exception exception)
        {
            log += "[SysInfo]";
            log += $"RprtGen:{DateTime.Now.Ticks};";
            log += $"SysVer:{Kernel.Version};";
            log += $"Cpu:{Cosmos.Core.CPU.GetCPUBrandString()};";
            log += $"CpuUp:{Cosmos.Core.CPU.GetCPUUptime()};";
            log += $"Vendor:{Cosmos.Core.CPU.GetCPUVendorName()};";
            log += $"RamMb:{Cosmos.Core.CPU.GetAmountOfRAM()};";
            log += $"RamUsed:{GCImplementation.GetUsedRAM()};";
            log += $"RamAvail:{GCImplementation.GetAvailableRAM()};";
            log += $"LoggedIn:{Kernel.CurrentUser != null};";
            if (Kernel.CurrentUser != null)
            {
                log += $"Admin:{Kernel.CurrentUser.Admin.ToString()};";
            }
            log += $"[Excpt]{exception.ToString()};";
            log += "[ProcDump]";
            foreach (Process process in ProcessManager.Processes)
            {
                log += $"Id:{process.Id};";
                log += $"Name:{process.Name};";
                log += $"Running:{process.IsRunning};";
                log += $"Argc:{process.Args.Count};";
                log += $"Prnt:{process.Parent};";
                log += $"CrtTicks:{process.Created.Ticks};";
            }
            log += "[LogDump]";
            foreach (LogEvent logEvent in Log.Logs)
            {
                log += $"Date:{logEvent.Date.Ticks};";
                log += $"Prt:";
                switch (logEvent.Priority)
                {
                    case LogPriority.Info:
                        log += "I";
                        break;
                    case LogPriority.Warning:
                        log += "W";
                        break;
                    case LogPriority.Error:
                        log += "E";
                        break;
                }
                log += ";";
                log += $"Src:{logEvent.Source};";
                log += $"Msg:{logEvent.Message};";
            }
            log += "[Counters]";
            log += $"Users:{UserManager.Users.Count};";
            log += $"LogEvents:{Log.Logs.Count};";
            log += $"Cmds:{CommandManager.commands.Count};";
            log += "[PciDump]";
            foreach (PCIDevice device in Cosmos.HAL.PCI.Devices)
            {
                log += $"I:{device.DeviceID.ToString("X")};";
                log += $"V:{device.VendorID.ToString("X")};";
            }
        }

        internal static void ShowCrashScreen(Exception exception)
        {
            ShowMessageFullScreen(messageLoading);
            GenerateCrashLog(exception);
            while (true)
            {
                ShowMessageFullScreen(message);
                var key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.Escape:
                        Cosmos.System.Power.Reboot();
                        break;
                    case ConsoleKey.V:
                        ShowCrashLog();
                        break;
                    default:
                        break;
                }
                Cosmos.Core.Memory.Heap.Collect();
            }
        }
    }
}
