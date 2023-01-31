using System;

namespace SphereOS.Commands
{
    /// <summary>
    /// A command for the shell.
    /// </summary>
    internal abstract class Command
    {
        /// <summary>
        /// Initialise a new command.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        internal Command(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="args">The arguments of the command, including the command name.</param>
        /// <returns>A return code, indicating success or failure.</returns>
        /// <exception cref="NotImplementedException">Thrown when the command is not implemented.</exception>
        internal abstract ReturnCode Execute(string[] args);

        /// <summary>
        /// The name of the command.
        /// </summary>
        internal string Name { get; private set; }

        /// <summary>
        /// The description of the command.
        /// </summary>
        internal string Description { get; set; }

        /// <summary>
        /// The usage of the command.
        /// </summary>
        internal string Usage { get; set; }

        /// <summary>
        /// The topic of the command, to categorise help pages.
        /// </summary>
        internal string Topic { get; set; }
    }
}
