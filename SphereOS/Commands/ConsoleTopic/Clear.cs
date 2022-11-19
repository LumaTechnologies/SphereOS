using System;

namespace SphereOS.Commands.ConsoleTopic
{
    internal class Clear : Command
    {
        public Clear() : base("clear")
        {
            Description = "Clear the screen.";

            Topic = "console";
        }

        internal override ReturnCode Execute(string[] args)
        {
            Console.Clear();
            return ReturnCode.Success;
        }
    }
}
