using SphereOS.Commands;
using System;

namespace SphereOS.Shell
{
    internal static class Shell
    {
        internal static string WorkingDir = @"0:\";

        internal static void WelcomeMessage()
        {
            Util.Print(ConsoleColor.Cyan, "Welcome to ");
            Util.Print(ConsoleColor.Magenta, "SphereOS");
            Util.PrintLine(ConsoleColor.Gray, $" - version {Kernel.Version}");
            Util.PrintLine(ConsoleColor.White, "Copyright (c) 2022. All rights reserved.");

            Util.Print(ConsoleColor.Yellow, "New in this version: ");
            Util.PrintLine(ConsoleColor.White, "New security, scripting, and more!");
        }

        internal static void Execute()
        {
            if (Kernel.CurrentUser != null)
            {
                Kernel.CurrentUser.FlushMessages();

                Util.Print(ConsoleColor.Cyan, Kernel.CurrentUser.Username);
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
                        Util.PrintLine(ConsoleColor.White, $"Error information: {e.ToString()}");
                    }
                }
                else
                {
                    Util.PrintLine(ConsoleColor.Red, $"Unknown command '{commandName}'.");
                }
            }
            else
            {
                LoginPrompt.PromptLogin();
            }
        }
    }
}
