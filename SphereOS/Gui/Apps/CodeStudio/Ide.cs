using SphereOS.Core;
using SphereOS.Gui.UILib;
using Cosmos.System.Graphics;
using System.Drawing;
using RiverScript.VM;
using RiverScript;
using RiverScript.StandardLibrary;
using System;
using System.Collections.Generic;

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

        TextBox output;

        private const int headersHeight = 24;

        private const int outputHeight = 128;

        private static class Theme
        {
            internal static Color Background = Color.FromArgb(68, 76, 84);
            internal static Color CodeBackground = Color.FromArgb(41, 46, 51);
        }

        private void RunClicked(int x, int y)
        {
            /*Kernel.PrintDebug("1");
            output.Foreground = Color.White;
            Kernel.PrintDebug("2");
            output.Text = string.Empty;
            Kernel.PrintDebug("3");
            try
            {
                Kernel.PrintDebug("4");
                Interpreter interpreter = new Interpreter();
                Kernel.PrintDebug("5");

                StandardLibrary.LoadStandardLibrary(interpreter);
                Kernel.PrintDebug("6");

                interpreter.DefineVariable("print", new VMNativeFunction(
                new List<string> { ("string") },
                (List<VMObject> arguments) =>
                {
                    Kernel.PrintDebug("A1");
                    output.Text += arguments[0].ToString() + "\n";
                    Kernel.PrintDebug("A2");
                    return new VMNull();
                }), scope: null);
                Kernel.PrintDebug("7");

                Script script = new Script(editor.Text);
                Kernel.PrintDebug("8");
                interpreter.InterpretScript(script);
                Kernel.PrintDebug("9");
            }
            catch (Exception e)
            {
                Kernel.PrintDebug("10");
                output.Foreground = Color.Red;
                Kernel.PrintDebug("11");
                output.Text += $"Error occurred in your program: {e.Message}";
                Kernel.PrintDebug("12");
            }*/
        }

        internal void Start()
        {
            mainWindow = new AppWindow(process, 96, 96, 832, 576);
            mainWindow.Title = "CodeStudio";
            mainWindow.Clear(Color.DarkGray);
            mainWindow.Closing = process.TryStop;
            wm.AddWindow(mainWindow);

            runButton = new Button(mainWindow, 0, 0, 80, headersHeight);
            runButton.Background = Theme.Background;
            runButton.Border = Theme.Background;
            runButton.Foreground = Color.White;
            runButton.Text = "Run";
            runButton.Image = runBitmap;
            runButton.ImageLocation = Button.ButtonImageLocation.Left;
            //runButton.OnClick = RunClicked
            wm.AddWindow(runButton);

            editor = new TextBox(mainWindow, 0, headersHeight, mainWindow.Width, mainWindow.Height - outputHeight - (headersHeight * 2))
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

            mainWindow.DrawString("Output", Color.White, 0, mainWindow.Height - outputHeight - headersHeight);

            wm.Update(mainWindow);
        }
    }
}
