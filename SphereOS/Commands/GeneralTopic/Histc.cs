using SphereOS.Shell;
using System;

namespace SphereOS.Commands.GeneralTopic
{
    internal class Histc : Command
    {
        public Histc() : base("histc")
        {
            Description = "Clear the shell's command history.";

            Topic = "general";
        }

        internal override ReturnCode Execute(string[] args)
        {
            if (Kernel.CurrentUser.CommandHistory.Count > 0)
            {
                Kernel.CurrentUser.CommandHistory.Clear();

                Util.PrintLine(ConsoleColor.Green, "Command history cleared.");
            }
            else
            {
                Util.PrintLine(ConsoleColor.Gray, "No history to clear.");
            }

            return ReturnCode.Success;
        }
    }
}
