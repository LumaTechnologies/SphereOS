using SphereOS.Core;
using System;
using System.IO;

namespace SphereOS.Commands.ConsoleTopic
{
    internal class Rs : Command
    {
        public Rs() : base("rs")
        {
            Description = "Run a script or the RiverScript REPL.";

            Topic = "console";
        }

        internal override ReturnCode Execute(string[] args)
        {
            if (args.Length > 2)
            {
                Util.PrintLine(ConsoleColor.Red, "Invalid usage. Please either provide the name of the file to run, or to start the REPL, no arguments.");
                return ReturnCode.Invalid;
            }

            if (args.Length == 2)
            {
                string path = Path.Join(Kernel.WorkingDir, args[1]);

                if (!FileSecurity.CanAccess(Kernel.CurrentUser, path))
                {
                    Util.PrintLine(ConsoleColor.Red, "You do not have permission to access this file.");
                    return ReturnCode.Unauthorised;
                }

                if (!File.Exists(path))
                {
                    Util.PrintLine(ConsoleColor.Red, $"No such file '{Path.GetFileName(path)}'.");
                    return ReturnCode.NotFound;
                }

                string source = File.ReadAllText(path);

                try
                {
                    RiverScript.Script script = new RiverScript.Script(source);
                    script.Lex();
                    RiverScript.VM.Interpreter interpreter = new RiverScript.VM.Interpreter();
                    RiverScript.StandardLibrary.StandardLibrary.LoadStandardLibrary(interpreter);
                    interpreter.InterpretScript(script);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return ReturnCode.Failure;
                }
            }
            else
            {
                var repl = new RiverScript.Repl();
                repl.Start();
            }

            return ReturnCode.Success;
        }
    }
}
