﻿using RiverScript;
using RiverScript.StandardLibrary;
using RiverScript.VM;
using SphereOS.Core;
using SphereOS.Gui.SmoothMono;
using SphereOS.Gui.UILib;
using System;
using System.Collections.Generic;
using System.Drawing;

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

                            if (line > outputWindow.Height / FontData.Height)
                            {
                                line = 0;
                                col = 0;

                                outputWindow.Clear(Color.Black);
                            }
                        }
                    }
                    wm.Update(outputWindow);

                    line++;
                    col = 0;

                    if (line > outputWindow.Height / FontData.Height)
                    {
                        line = 0;
                        col = 0;

                        outputWindow.Clear(Color.Black);
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
