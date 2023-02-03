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
    internal class RiverScriptUIConsole
    {
        internal RiverScriptUIConsole(Process process, Script script, string title)
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

            int line = 0;
            int col = 0;
            int width = outputWindow.Width / FontData.Width;
            int height = outputWindow.Height / FontData.Height;
            char[] outputBuffer = new char[width * height];

            try
            {
                Interpreter interpreter = new Interpreter();

                StandardLibrary.LoadStandardLibrary(interpreter);

                interpreter.DefineVariable("print", new VMNativeFunction(
                new List<string> { ("object") },
                (List<VMObject> arguments) =>
                {
                    string str = arguments[0].ToString();
                    for (int i = 0; i < str.Length; i++)
                    {
                        outputWindow.DrawString(str[i].ToString(), Color.White, col * FontData.Width, line * FontData.Height);

                        outputBuffer[(line * width) + col] = str[i];

                        col++;
                        if (col >= width)
                        {
                            line++;
                            col = 0;
                        }
                    }
                    wm.Update(outputWindow);

                    line++;
                    col = 0;

                    if (line > outputWindow.Height / FontData.Height)
                    {
                        // Scroll up.
                        char[] newBuffer = new char[width * height];
                        for (int y = 1; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                newBuffer[((y - 1) * width) + x] = outputBuffer[(y * width) + x];
                            }
                        }
                        outputBuffer = newBuffer;

                        outputWindow.Clear(Color.Black);
                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                outputWindow.DrawString(outputBuffer[(y * width) + x].ToString(), Color.White, x * FontData.Width, y * FontData.Height);
                            }
                        }
                    }

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
