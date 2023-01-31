using SphereOS.Shell;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace SphereOS.Commands.GeneralTopic
{
    internal class Help : Command
    {
        public Help() : base("help")
        {
            Description = "View help for SphereOS.";

            Usage = "<topic/command/all>";

            Topic = "general";
        }

        internal override ReturnCode Execute(string[] args)
        {
            if (args.Length == 2)
            {
                var query = args[1].ToLower();

                if (query.Trim() == string.Empty)
                {
                    return ReturnCode.Failure;
                }

                if (query == "all")
                {
                    // Find all topics
                    List<string> topics = new();
                    foreach (Command command in CommandManager.Commands)
                    {
                        // Prevent duplicates
                        if (topics.Contains(command.Topic)) continue;

                        topics.Add(command.Topic);
                    }

                    List<List<string>> columns = new();
                    foreach (string topic in topics)
                    {
                        List<string> column = new() { topic };

                        foreach (Command command in CommandManager.Commands)
                        {
                            if (command.Topic == topic)
                            {
                                column.Add(command.Name);
                            }
                        }

                        columns.Add(column);
                    }

                    Util.PrintTable(columns, ConsoleColor.White, ConsoleColor.Cyan);

                    Util.PrintLine(ConsoleColor.Gray, $"{CommandManager.Commands.Count} commands, {topics.Count} topics");

                    return ReturnCode.Success;
                }

                // List of commands in the topic
                List<Command> commands = new();

                // Longest command name
                int maxLength = 0;

                foreach (Command command in CommandManager.Commands)
                {
                    // Command name match
                    if (command.Name == query)
                    {
                        Util.Print(ConsoleColor.Cyan, command.Name);
                        Util.PrintLine(ConsoleColor.Gray, $" (topic: {command.Topic})");
                        Util.PrintLine(ConsoleColor.White, command.Description);

                        if (command.Usage != null)
                        {
                            Util.Print(ConsoleColor.Gray, "Usage: ");
                            Util.PrintLine(ConsoleColor.White, $"{command.Name} {command.Usage}");
                        }

                        return ReturnCode.Success;
                    }

                    // Topic match (add to list of commands in the topic)
                    if (command.Topic == query)
                    {
                        commands.Add(command);
                        maxLength = Math.Max(maxLength, command.Name.Length);
                    }
                }

                if (commands.Count > 0)
                {
                    foreach (Command command in commands)
                    {
                        Util.PrintLine(ConsoleColor.White, $"{command.Name}{new string(' ', maxLength - command.Name.Length)} - {command.Description}");
                    }
                }
                else
                {
                    Util.PrintLine(ConsoleColor.Red, "Unknown command or help topic.");
                    return ReturnCode.Failure;
                }

                return ReturnCode.Success;
            }
            else
            {
                Util.PrintLine(ConsoleColor.White, $"Usage: {Name} {Usage}");
                Util.Print(ConsoleColor.Cyan, "Available help topics: ");
                Util.PrintLine(ConsoleColor.White, "general, files, network, power, time, users");

                return ReturnCode.Invalid;
            }
        }
    }
}
