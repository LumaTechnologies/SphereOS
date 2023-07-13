using SphereOS.Core;
using SphereOS.Shell;
using System;

namespace SphereOS.Commands.GeneralTopic
{
    internal class Gui : Command
    {
        public Gui() : base("gui")
        {
            Description = "Start the GUI.";

            Topic = "general";
        }

        internal override ReturnCode Execute(string[] args)
        {
            bool result = SphereOS.Gui.Gui.StartGui();

            if (result)
            {
                Shell.Shell.CurrentShell.Stop();

                return ReturnCode.Success;
            }
            else
            {
                Util.PrintWarning("Failed to start the GUI.");

                return ReturnCode.Failure;
            }
        }
    }
}
