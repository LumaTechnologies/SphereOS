using Cosmos.Core;
using Cosmos.HAL;
using SphereOS.Commands;
using SphereOS.Logging;
using SphereOS.Users;
using System;
using System.IO;

namespace SphereOS.Core
{
    public static class CrashScreen
    {
        private static string messageLoading = @"
 SphereOS Crash

 Generating crash log, please wait...";

        private static string message = @"
 SphereOS Crash

 Something went wrong, and SphereOS must reboot.

 SphereOS has generated a crash log, which you can view by pressing 'V'.
 This crash log has also been saved to 0:\crash.log.

                                             [V] View Log   [Esc] Reboot";

        private static string messageNoLogDump = @"
 SphereOS Crash

 Something went wrong, and SphereOS must reboot.

 SphereOS has generated a crash log, which you can view by pressing 'V'.

                                             [V] View Log   [Esc] Reboot";

        private static bool? logDumpSuccess;

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
            log = "[SphereOS Crash]\n" +
                exception.ToString() +
                "\nDate: " + DateTime.Now.ToString("dddd, dd/MM/yyyy HH:mm:ss");
            try
            {
                File.WriteAllText(@"0:\crash.log", log);
                logDumpSuccess = true;
            }
            catch
            {
                logDumpSuccess = false;
            }
        }

        public static void ShowCrashScreen(Exception exception)
        {
            try
            {
                ProcessManager.StopAll();
                ShowMessageFullScreen(messageLoading);
                GenerateCrashLog(exception);
                while (true)
                {
                    ShowMessageFullScreen((bool)logDumpSuccess ? message : messageNoLogDump);
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
            catch
            {
                ShowMessageFullScreen("Fatal: Double fault.");
            }
        }
    }
}
