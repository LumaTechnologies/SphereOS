using SphereOS.Shell;
using System;

namespace SphereOS.Commands.FilesTopic
{
    internal class Paint : Command
    {
        public Paint() : base("paint")
        {
            Description = "Paint an image.";

            Topic = "files";
        }

        internal override ReturnCode Execute(string[] args)
        {
            /*if (args.Length != 2)
            {
                Util.PrintLine(ConsoleColor.Red, "Invalid usage. Please provide the name of the file to paint.");
                return ReturnCode.Invalid;
            }

            string paintPath = Path.Join(Shell.Shell.CurrentShell.WorkingDir, args[1]);

            if (Path.GetExtension(paintPath) != ".pnt")
            {
                Util.PrintLine(ConsoleColor.Red, "Paint can only open .pnt (SphereOS Paint) files.");
                return ReturnCode.Invalid;
            }*/

            Util.PrintLine(ConsoleColor.Cyan, "Note: Saving is currently not possible due to a bug.");
            Util.PrintLine(ConsoleColor.Gray, "Press a key to start SphereOS Paint.");
            Console.ReadKey();

            Apps.Paint.Paint paint = new SphereOS.Apps.Paint.Paint();
            paint.Run();

            return ReturnCode.Success;
        }
    }
}
