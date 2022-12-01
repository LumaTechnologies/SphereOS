using SphereOS.Commands;
using SphereOS.Core;
using System;

namespace SphereOS.Shell
{
    internal class Shell : Process
    {
        internal Shell() : base("Shell", ProcessType.Application)
        {
            Critical = true;
        }

        internal static Shell CurrentShell
        {
            get
            {
                return ProcessManager.GetProcess<Shell>();
            }
        }

        internal string WorkingDir { get; set; } = @"0:\";

        internal bool CommandRunning { get; private set; } = false;

        internal void WelcomeMessage()
        {
            Util.Print(ConsoleColor.Cyan, "Welcome to ");
            Util.Print(ConsoleColor.Magenta, "SphereOS");
            Util.PrintLine(ConsoleColor.Gray, $" - version {Kernel.Version}");
            Util.PrintLine(ConsoleColor.White, "Copyright (c) 2022. All rights reserved.");

            /*Util.Print(ConsoleColor.Yellow, "New in this version: ");
            Util.PrintLine(ConsoleColor.White, "New GUI!");*/
        }

        #region Process
        internal override void Start()
        {
            base.Start();
            WelcomeMessage();
        }

        internal override void Run()
        {
            if (CommandRunning) return;

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
                    CommandRunning = true;
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
                    CommandRunning = false;
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
        #endregion
    }
}
