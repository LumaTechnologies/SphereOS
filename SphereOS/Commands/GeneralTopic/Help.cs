using SphereOS.Shell;
using System;

namespace SphereOS.Commands.GeneralTopic
{
    internal class Help : Command
    {
        public Help() : base("help")
        {
            Description = "Show a list of commands.";

            Topic = "general";
        }

        internal override ReturnCode Execute(string[] args)
        {
            Util.PrintLine(ConsoleColor.Green, "SphereOS Help");
            Util.Print(ConsoleColor.Cyan, "Select a help topic: ");
            Util.PrintLine(ConsoleColor.White, "general, files, games (NEW), network, power, time, users");

            var topic = Console.ReadLine();

            if (topic.Trim() == string.Empty)
            {
                return ReturnCode.Aborted;
            }

            bool found = false;

            foreach (Command command in CommandManager.commands)
            {
                if (command.Topic == topic)
                {
                    Util.PrintLine(ConsoleColor.White, $"- {command.Name}: {command.Description}");
                    found = true;
                }
            }

            if (!found)
            {
                Util.PrintLine(ConsoleColor.Red, "Unknown help topic.");
                return ReturnCode.Failure;
            }

            return ReturnCode.Success;
        }
    }
}
