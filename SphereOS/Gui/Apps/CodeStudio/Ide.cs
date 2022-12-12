using SphereOS.Core;
using SphereOS.Gui.UILib;
using Cosmos.System.Graphics;
using System.Drawing;
using RiverScript.VM;
using RiverScript;
using RiverScript.StandardLibrary;
using System;
using System.Collections.Generic;
using SphereOS.Gui.SmoothMono;

namespace SphereOS.Gui.Apps.CodeStudio
{
    internal class Ide
    {
        internal Ide(Process process, WindowManager wm)
        {
            this.process = process;
            this.wm = wm;
        }

        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.CodeStudio.Run.bmp")]
        private static byte[] _runBytes;
        private static Bitmap runBitmap = new Bitmap(_runBytes);

        Process process;

        WindowManager wm;

        AppWindow mainWindow;

        Button runButton;

        TextBox editor;

        private const int headersHeight = 24;

        private static class Theme
        {
            internal static Color Background = Color.FromArgb(68, 76, 84);
            internal static Color CodeBackground = Color.FromArgb(41, 46, 51);
        }

        private void RunClicked(int x, int y)
        {
            AppWindow outputWindow = new AppWindow(process, 320, 264, 384, 240);

            outputWindow.Title = "CodeStudio";
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

                Script script = new Script(editor.Text);
                interpreter.InterpretScript(script);
            }
            catch (Exception e)
            {
                outputWindow.Clear(Color.Black);
                outputWindow.DrawString($"Error: {e.Message}", Color.Red, 0, 0);

                wm.Update(outputWindow);
            }
        }

        internal void Start()
        {
            mainWindow = new AppWindow(process, 96, 96, 832, 576);
            mainWindow.Title = "CodeStudio - Untitled";
            mainWindow.Clear(Theme.Background);
            mainWindow.Closing = process.TryStop;
            //mainWindow.CanResize = true;
            wm.AddWindow(mainWindow);

            runButton = new Button(mainWindow, 0, 0, 80, headersHeight);
            runButton.Background = Theme.Background;
            runButton.Border = Theme.Background;
            runButton.Foreground = Color.White;
            runButton.Text = "Run";
            runButton.Image = runBitmap;
            runButton.ImageLocation = Button.ButtonImageLocation.Left;
            runButton.OnClick = RunClicked;
            wm.AddWindow(runButton);

            editor = new TextBox(mainWindow, 0, headersHeight, mainWindow.Width, mainWindow.Height - headersHeight /*- outputHeight - (headersHeight * 2)*/)
            {
                Background = Theme.CodeBackground,
                Foreground = Color.White,
                Text = "print(\"Hello World!\")"
            };
            wm.AddWindow(editor);

            /*output = new TextBox(mainWindow, 0, mainWindow.Height - (outputHeight - headersHeight), mainWindow.Width, outputHeight - headersHeight)
            {
                Background = Theme.CodeBackground,
                Foreground = Color.Gray,
                Text = "Code output will be displayed here.",
                ReadOnly = true
            };
            wm.AddWindow(output);*/

            //mainWindow.DrawString("Output", Color.White, 0, mainWindow.Height - outputHeight - headersHeight);

            wm.Update(mainWindow);
        }
    }
}
