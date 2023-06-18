using SphereOS.ConsoleApps;
using System.Text;

namespace SphereOS.Commands.GeneralTopic
{
    internal class Hist : Command
    {
        public Hist() : base("hist")
        {
            Description = "View your command history.";

            Topic = "general";
        }

        internal override ReturnCode Execute(string[] args)
        {
            var builder = new StringBuilder();

            foreach (string command in Kernel.CurrentUser.CommandHistory)
            {
                builder.AppendLine(command);
            }

            TextEditor textEditor = new(builder.ToString(), isPath: false);

            textEditor.Start();

            return ReturnCode.Success;
        }
    }
}
