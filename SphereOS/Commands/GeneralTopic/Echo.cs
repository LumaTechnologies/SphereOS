using System;

namespace SphereOS.Commands.GeneralTopic
{
    internal class Echo : Command
    {
        public Echo() : base("echo")
        {
            Description = "Print to the console.";

            Topic = "general";
        }

        internal override ReturnCode Execute(string[] args)
        {
            string text = string.Empty;
            for (int i = 1; i < args.Length; i++)
            {
                text += args[i];
                if (i != args.Length - 1)
                {   
                    text += " ";
                }
            }

            Console.WriteLine(text);

            return ReturnCode.Success;
        }
    }
}
