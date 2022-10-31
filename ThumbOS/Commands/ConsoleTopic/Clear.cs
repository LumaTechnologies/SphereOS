using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThumbOS.Commands.ConsoleTopic
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
