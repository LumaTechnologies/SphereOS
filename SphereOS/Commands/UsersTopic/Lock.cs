using SphereOS.Shell;
using System;

namespace SphereOS.Commands.UsersTopic
{
    internal class Lock : Command
    {
        public Lock() : base("lock")
        {
            Description = "Lock this PC.";

            Topic = "users";
        }

        internal override ReturnCode Execute(string[] args)
        {
            try
            {
                bool authenticated = false;
                while (!authenticated)
                {
                    Console.Clear();

                    Util.Print(ConsoleColor.Gray, "SphereOS - ");
                    Util.Print(ConsoleColor.Cyan, "This PC is locked.");
                    Util.PrintLine(ConsoleColor.Gray, " (ESC to switch users)");

                    Util.PrintLine(ConsoleColor.Cyan, $"Enter the password for {Kernel.CurrentUser.Username}: ");

                    ReadLineExResult result = Util.ReadLineEx(cancelKeys: new Cosmos.System.ConsoleKeyEx[] { Cosmos.System.ConsoleKeyEx.Escape }, mask: true);
                    if (result.CancelKey == Cosmos.System.ConsoleKeyEx.Escape)
                    {
                        Kernel.CurrentUser = null;
                        Shell.Shell.CurrentShell.WorkingDir = @"0:\";

                        Console.Clear();
                        Console.SetCursorPosition(0, 0);

                        return ReturnCode.Aborted;
                    }

                    authenticated = Kernel.CurrentUser.Authenticate(result.Input);
                }
                return ReturnCode.Success;
            }
            catch
            {
                Kernel.CurrentUser = null;
                Shell.Shell.CurrentShell.WorkingDir = @"0:\";

                Console.Clear();
                Console.SetCursorPosition(0, 0);
                Util.PrintLine(ConsoleColor.Red, "An error occurred in the lock screen. Please log back in.");
                return ReturnCode.Failure;
            }
        }
    }
}
