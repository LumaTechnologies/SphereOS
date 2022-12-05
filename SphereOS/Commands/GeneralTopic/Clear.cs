using System;

namespace SphereOS.Commands.GeneralTopic
{
    internal class Clear : Command
    {
        public Clear() : base("clear")
        {
            Description = "Clear the screen.";

            Topic = "general";
        }

        internal override ReturnCode Execute(string[] args)
        {
            Console.Clear();
            return ReturnCode.Success;
        }
    }
}
