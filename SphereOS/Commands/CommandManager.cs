using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SphereOS.Commands
{
    /// <summary>
    /// Manages shell commands.
    /// </summary>
    internal static class CommandManager
    {
        /// <summary>
        /// The list of commands available on the system.
        /// </summary>
        internal static List<Command> commands = new();

        /// <summary>
        /// Find a command by its name.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <returns>The command, if it was found; otherwise, null.</returns>
        internal static Command GetCommand(string name)
        {
            foreach (Command command in commands)
            {
                if (command.Name == name)
                {
                    return command;
                }
            }
            return null;
        }

        /// <summary>
        /// Register a command.
        /// </summary>
        /// <param name="command">The command to register.</param>
        private static void RegisterCommand(Command command)
        {
            commands.Add(command);
        }

        /// <summary>
        /// Register all commands.
        /// </summary>
        internal static void RegisterCommands()
        {
            /* Console */
            RegisterCommand(new ConsoleTopic.Clear());
            RegisterCommand(new ConsoleTopic.About());
            RegisterCommand(new ConsoleTopic.Help());
            RegisterCommand(new ConsoleTopic.Sysinfo());

            /* Files */
            RegisterCommand(new FilesTopic.Cd());
            RegisterCommand(new FilesTopic.Ls());
            RegisterCommand(new FilesTopic.Fsinfo());
            RegisterCommand(new FilesTopic.Edit());

            /* Network */
            RegisterCommand(new NetworkTopic.Resolve());
            RegisterCommand(new NetworkTopic.Cloudchat());

            /* Power */
            RegisterCommand(new PowerTopic.Shutdown());
            RegisterCommand(new PowerTopic.Reboot());

            /* Time */
            RegisterCommand(new TimeTopic.Date());
            RegisterCommand(new TimeTopic.Clock());

            /* Users */
            RegisterCommand(new UsersTopic.Logout());
            RegisterCommand(new UsersTopic.Lock());
            RegisterCommand(new UsersTopic.Adduser());
            RegisterCommand(new UsersTopic.Deluser());
            RegisterCommand(new UsersTopic.Lsuser());
            RegisterCommand(new UsersTopic.Admin());
        }
    }
}
