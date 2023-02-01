using SphereOS.Commands;
using SphereOS.Core;
using SphereOS.Users;
using System;
using System.Collections.Generic;
using System.IO;

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

        internal bool Backgrounded { get; private set; } = false;

        internal void WelcomeMessage()
        {
            //Util.PrintLine(ConsoleColor.Cyan, banner);
            Util.Print(ConsoleColor.Cyan, "Welcome to ");
            Util.Print(ConsoleColor.Magenta, "SphereOS");
            Util.PrintLine(ConsoleColor.Gray, $" - {Kernel.Version}");
            Util.PrintLine(ConsoleColor.White, "Copyright (c) 2023 Sphere Systems. All rights reserved.");

            /*Util.Print(ConsoleColor.Yellow, "New in this version: ");
            Util.PrintLine(ConsoleColor.White, "New GUI!");*/
        }

        #region Process
        internal override void Start()
        {
            base.Start();
            WelcomeMessage();
        }

        internal ReturnCode Execute(string text)
        {
            text = text.Trim();

            if (text == string.Empty)
                return ReturnCode.Success;

            // Parse arguments.
            var args = new List<string>();
            bool inQuotes = false;
            string buffer = string.Empty;
            for (int i = 0; i < text.Length; i++)
            {
                switch (text[i])
                {
                    case '"':
                        inQuotes = !inQuotes;
                        break;
                    case ' ':
                        if (inQuotes)
                        {
                            buffer += ' ';
                        }
                        else
                        {
                            args.Add(buffer);
                            buffer = string.Empty;
                        }
                        break;
                    default:
                        buffer += text[i];
                        break;
                }
            }
            if (buffer.Length > 0)
            {
                args.Add(buffer);
            }

            var name = args[0];

            Command command = CommandManager.GetCommand(name);
            if (command != null)
            {
                try
                {
                    return command.Execute(args.ToArray());
                }
                catch (Exception e)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Util.PrintLine(ConsoleColor.Red, $"An error occurred while running '{name}'.");
                    Util.PrintLine(ConsoleColor.White, e.ToString());

                    return ReturnCode.Failure;
                }
            }
            else
            {
                string path = PathUtil.JoinPaths(WorkingDir, name);
                if (FileSecurity.CanAccess(Kernel.CurrentUser, path) && File.Exists(path))
                {
                    return RunShellScript(path);
                }
                else
                {
                    Util.PrintLine(ConsoleColor.Red, $"Command or shell script '{name}' does not exist.");
                    return ReturnCode.NotFound;
                }
            }
        }

        internal ReturnCode RunShellScript(string path)
        {
            User user = Kernel.CurrentUser;

            Console.CursorVisible = false;

            string[] lines = File.ReadAllLines(path);

            foreach (string line in lines)
            {
                // If the logged in user has changed during
                // script execution, abort the script. This
                // is to prevent attacks where a user could
                // execute arbitrary code as another user.
                if (Kernel.CurrentUser != user)
                {
                    return ReturnCode.Aborted;
                }

                ReturnCode returnCode = Execute(line);

                if (returnCode != ReturnCode.Success)
                {
                    Util.Print(ConsoleColor.Red, "Script aborted. ");
                    Util.PrintLine(ConsoleColor.Gray, $"(code {((int)returnCode).ToString()})");

                    return returnCode;
                }
            }

            Console.CursorVisible = true;

            return ReturnCode.Success;
        }

        internal override void Run()
        {
            if (Backgrounded) return;

            if (Kernel.CurrentUser != null)
            {
                Kernel.CurrentUser.FlushMessages();

                Util.Print(ConsoleColor.Green, Kernel.CurrentUser.Username);

                string dirDisplay = WorkingDir;
                if (WorkingDir == $@"0:\users\{Kernel.CurrentUser.Username}")
                {
                    dirDisplay = "~";
                }
                dirDisplay.Replace($@"0:\users\{Kernel.CurrentUser.Username}\", string.Empty);
                Util.Print(ConsoleColor.Blue, @$"@{SysCfg.Name} ");
                Util.Print(ConsoleColor.White, @$"{dirDisplay}");
                Util.Print(ConsoleColor.Blue, Kernel.CurrentUser.Admin ? "# " : "$ ");

                var input = Console.ReadLine();

                Backgrounded = true;

                Execute(input);

                Backgrounded = false;
            }
            else
            {
                LoginPrompt.PromptLogin();
            }
        }
        #endregion
    }
}
