using RiverScript.StandardLibrary;
using RiverScript.VM;
using RiverScript;
using SphereOS.Commands.FilesTopic;
using SphereOS.Gui.UILib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SphereOS.Core;
using SphereOS.Gui.SmoothMono;

namespace SphereOS.Gui
{
    /// <summary>
    /// A dialogue that runs a RiverScript script.
    /// </summary>
    internal class RsDialogue
    {
        internal RsDialogue(Process process, Script script, string title)
        {
            this.Process = process;
            this.Script = script;
            this.Title = title;
        }

        internal Process Process { get; set; }
        internal Script Script { get; set; }
        internal string Title { get; set; }

        internal void Show()
        {
            AppWindow outputWindow = new AppWindow(Process, 320, 264, 384, 240);

            WindowManager wm = ProcessManager.GetProcess<WindowManager>();

            outputWindow.Title = Title;
            outputWindow.Clear(Color.Black);

            wm.AddWindow(outputWindow);
            wm.Update(outputWindow);

            int outputLine = 0;

            try
            {
                Interpreter interpreter = new Interpreter();

                StandardLibrary.LoadStandardLibrary(interpreter);

                interpreter.DefineVariable("print", new VMNativeFunction(
                new List<string> { ("object") },
                (List<VMObject> arguments) =>
                {
                    outputWindow.DrawString(arguments[0].ToString(), Color.White, 0, outputLine * FontData.Height);
                    wm.Update(outputWindow);

                    outputLine++;

                    /*if (outputLine > outputWindow.Height / FontData.Height)
                    {
                        outputWindow.Clear(Color.Black);
                        outputLine = 0;
                    }*/

                    return new VMNull();
                }), scope: null);

                interpreter.InterpretScript(Script);
            }
            catch (Exception e)
            {
                outputWindow.Clear(Color.Black);
                outputWindow.DrawString($"Error: {e.Message}", Color.Red, 0, 0);

                wm.Update(outputWindow);
            }
        }
    }
}
