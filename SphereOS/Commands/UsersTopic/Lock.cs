using System;
using SphereOS.Shell;

namespace SphereOS.Commands.UsersTopic
{
    internal class Lock : Command
    {
        public Lock() : base("lock")
        {
            Description = "Lock your PC.";

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

                    Util.Print(ConsoleColor.Gray, "[SphereOS] ");
                    Util.Print(ConsoleColor.Cyan, "This PC is locked.");
                    Util.PrintLine(ConsoleColor.Gray, " (ESC to switch users)");

                    Util.PrintLine(ConsoleColor.Cyan, $"Enter the password for {Kernel.CurrentUser.Username}: ");

                    var password = Util.ReadPassword(cancelKey: Cosmos.System.ConsoleKeyEx.Escape);
                    if (password == null)
                    {
                        Kernel.CurrentUser = null;
                        Shell.Shell.WorkingDir = @"0:\";

                        Console.Clear();
                        Console.SetCursorPosition(0, 0);

                        return ReturnCode.Aborted;
                    }

                    authenticated = Kernel.CurrentUser.Authenticate(password);
                }
                return ReturnCode.Success;
            }
            catch
            {
                Kernel.CurrentUser = null;
                Shell.Shell.WorkingDir = @"0:\";

                Console.Clear();
                Console.SetCursorPosition(0, 0);
                Util.PrintLine(ConsoleColor.Red, "An error occurred in the lock screen. Please log back in.");
                return ReturnCode.Failure;
            }
        }
    }
}
