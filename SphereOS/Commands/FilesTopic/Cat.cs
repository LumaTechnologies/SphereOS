using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SphereOS.Commands.FilesTopic
{
    internal class Cat : Command
    {
        public Cat() : base("cat")
        {
            Description = "Read a file.";

            Topic = "files";
        }

        internal override ReturnCode Execute(string[] args)
        {
            if (args.Length != 2)
            {
                Util.PrintLine(ConsoleColor.Red, "Invalid usage. Please provide the name of the file to read.");
                return ReturnCode.Invalid;
            }

            string path = Path.Join(Kernel.WorkingDir, args[1]);

            if (File.Exists(path))
            {
                foreach (var line in File.ReadAllLines(path))
                {
                    Console.WriteLine(line);
                }
            }
            else
            {
                Util.PrintLine(ConsoleColor.Red, $"No such file '{Path.GetFileName(path)}'.");
                return ReturnCode.NotFound;
            }

            return ReturnCode.Success;
        }
    }
}
