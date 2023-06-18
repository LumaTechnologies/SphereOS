using NextCore.Graphics.VideoConsole;
using System;
using Sys = Cosmos.System;

namespace NextCore
{
    public class Kernel : Sys.Kernel
    {
        protected override void BeforeRun()
        {
            Kernel.PrintDebug("1");
            Kernel.PrintDebug("Starting video console...");
            Kernel.PrintDebug("2");
            VideoConsole.Initialise();
            Kernel.PrintDebug("3");
            Console.SetCursorPosition(Console.GetCursorPosition().Top + 1, 0);
            Console.WriteLine("Hello from the System.Console plug!\nWrite something to get it echoed back.");
            Kernel.PrintDebug("4");
        }

        protected override void Run()
        {
            try
            {
                //Console.WriteLine(Console.ReadLine());
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
