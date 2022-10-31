using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThumbOS
{
    internal static class Help
    {
        internal static void Main(string[] args)
        {
            Util.PrintLine(ConsoleColor.Green, "ThumbOS Help");
            Util.Print(ConsoleColor.Cyan, "Select a help topic: ");
            Util.PrintLine(ConsoleColor.White, "console, time, network, system, users, files");
            var topic = Console.ReadLine();
            switch (topic)
            {
                case "console":
                    Util.PrintLine(ConsoleColor.White, "- about: Show information about ThumbOS.");
                    Util.PrintLine(ConsoleColor.White, "- help: Show a list of commands.");
                    Util.PrintLine(ConsoleColor.White, "- sysinfo: Show system information.");
                    Util.PrintLine(ConsoleColor.White, "- clear: Clear the console.");
                    break;
                case "time":
                    Util.PrintLine(ConsoleColor.White, "- date: Show the current date and time.");
                    Util.PrintLine(ConsoleColor.White, "- clock: Show a live clock.");
                    break;
                case "network":
                    Util.PrintLine(ConsoleColor.White, "- resolve: Find the IP of a domain.");
                    Util.PrintLine(ConsoleColor.White, "- cloudchat: Start CloudChat!");
                    break;
                case "system":
                    Util.PrintLine(ConsoleColor.White, "- shutdown: Shut down your PC");
                    Util.PrintLine(ConsoleColor.White, "- reboot: Reboot your PC.");
                    break;
                case "users":
                    Util.PrintLine(ConsoleColor.White, "- logout: Log out of ThumbOS.");
                    Util.PrintLine(ConsoleColor.White, "- lock: Lock your PC.");
                    Util.PrintLine(ConsoleColor.White, "- adduser: Add a new user.");
                    break;
                case "files":
                    Util.PrintLine(ConsoleColor.White, "- fsinfo: Show volume information.");
                    Util.PrintLine(ConsoleColor.White, "- ls: List files and directories.");
                    Util.PrintLine(ConsoleColor.White, "- cd: Change the working directory.");
                    Util.PrintLine(ConsoleColor.White, "- edit: Edit a file.");
                    break;
                default:
                    Util.PrintLine(ConsoleColor.Red, "Unknown help topic.");
                    break;
            }
        }
    }
}