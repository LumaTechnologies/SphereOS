using SphereOS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SphereOS
{
    internal static class Shell
    {
        internal static void Execute()
        {
            Kernel.CurrentUser.FlushMessages();

            Util.Print(ConsoleColor.Cyan, Kernel.CurrentUser.Username);
            Util.Print(ConsoleColor.Gray, @$"@SphereOS [{Kernel.WorkingDir}]> ");

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
    }
}
