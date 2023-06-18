using SphereOS.Logging;
using System.Collections.Generic;

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
        internal static List<Command> Commands = new();

        /// <summary>
        /// Find a command by its name.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <returns>The command, if it was found; otherwise, null.</returns>
        internal static Command GetCommand(string name)
        {
            foreach (Command command in Commands)
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
            Commands.Add(command);
        }

        /// <summary>
        /// Register all commands.
        /// </summary>
        internal static void RegisterCommands()
        {
            /* General (formerly Console) */
            RegisterCommand(new GeneralTopic.About());
            RegisterCommand(new GeneralTopic.Asyscfg());
            RegisterCommand(new GeneralTopic.Clear());
            RegisterCommand(new GeneralTopic.Crash());
            RegisterCommand(new GeneralTopic.Echo());
            RegisterCommand(new GeneralTopic.Help());
            RegisterCommand(new GeneralTopic.Hist());
            RegisterCommand(new GeneralTopic.Histc());
            RegisterCommand(new GeneralTopic.License());
            RegisterCommand(new GeneralTopic.Logs());
            RegisterCommand(new GeneralTopic.Lsproc());
            RegisterCommand(new GeneralTopic.Name());
            RegisterCommand(new GeneralTopic.Pci());
            RegisterCommand(new GeneralTopic.Rs());
            RegisterCommand(new GeneralTopic.Sysinfo());
            RegisterCommand(new GeneralTopic.Wd());

            /* Files */
            RegisterCommand(new FilesTopic.Al());
            RegisterCommand(new FilesTopic.Audit());
            RegisterCommand(new FilesTopic.Cat());
            RegisterCommand(new FilesTopic.Cd());
            RegisterCommand(new FilesTopic.Copy());
            RegisterCommand(new FilesTopic.Del());
            RegisterCommand(new FilesTopic.Edit());
            RegisterCommand(new FilesTopic.Ff());
            RegisterCommand(new FilesTopic.Fsinfo());
            RegisterCommand(new FilesTopic.Ls());
            RegisterCommand(new FilesTopic.Mkdir());
            RegisterCommand(new FilesTopic.Mkhome());
            RegisterCommand(new FilesTopic.Mv());
            RegisterCommand(new FilesTopic.Rmdir());
            RegisterCommand(new FilesTopic.Touch());
            //RegisterCommand(new FilesTopic.Perm());
            //RegisterCommand(new FilesTopic.Setperm());

            /* Games */
            RegisterCommand(new GamesTopic.Hangman());

            /* Network */
            //RegisterCommand(new NetworkTopic.Cloudchat());
            RegisterCommand(new NetworkTopic.Ip());
            RegisterCommand(new NetworkTopic.Ping());
            RegisterCommand(new NetworkTopic.Resolve());
            //RegisterCommand(new NetworkTopic.Httpserver());

            /* Power */
            RegisterCommand(new PowerTopic.Reboot());
            RegisterCommand(new PowerTopic.Shutdown());

            /* Time */
            RegisterCommand(new TimeTopic.Clock());
            RegisterCommand(new TimeTopic.Date());

            /* Users */
            RegisterCommand(new UsersTopic.Adduser());
            RegisterCommand(new UsersTopic.Admin());
            RegisterCommand(new UsersTopic.Broadcast());
            RegisterCommand(new UsersTopic.Deluser());
            RegisterCommand(new UsersTopic.Expire());
            RegisterCommand(new UsersTopic.Lock());
            RegisterCommand(new UsersTopic.Logout());
            RegisterCommand(new UsersTopic.Lsuser());
            RegisterCommand(new UsersTopic.Pass());
            RegisterCommand(new UsersTopic.Send());
            RegisterCommand(new UsersTopic.Su());

            Log.Info("CommandManager", $"{Commands.Count} commands were registered.");
        }
    }
}
