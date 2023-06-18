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
using System.IO;

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

        TextBox problems;

        private const int headersHeight = 24;
        private const int problemsHeight = 128;

        private string? path = null;

        private bool modified = false;

        private void TextChanged()
        {
            modified = true;

            UpdateTitle();
        }

        private static class Theme
        {
            internal static Color Background = Color.FromArgb(68, 76, 84);
            internal static Color CodeBackground = Color.FromArgb(41, 46, 51);
        }

        private void UpdateTitle()
        {
            if (path == null)
            {
                mainWindow.Title = "Untitled - RiverScript CodeStudio";
                return;
            }

            if (modified)
            {
                mainWindow.Title = $"{Path.GetFileName(path)}* - RiverScript CodeStudio";
            }
            else
            {
                mainWindow.Title = $"{Path.GetFileName(path)} - RiverScript CodeStudio";
            }
        }

        internal void Open(string newPath, bool readFile = true)
        {
            if (newPath == null) return;

            if (!FileSecurity.CanAccess(Kernel.CurrentUser, path))
            {
                MessageBox messageBox = new MessageBox(process, "RiverScript CodeStudio", $"Access to '{Path.GetFileName(newPath)}' is unauthorised.");
                messageBox.Show();
            }

            if (readFile && !File.Exists(newPath))
            {
                MessageBox messageBox = new MessageBox(process, "RiverScript CodeStudio", $"No such file '{Path.GetFileName(newPath)}'.");
                messageBox.Show();
            }

            path = newPath;

            if (readFile)
            {
                editor.Text = File.ReadAllText(path);

                editor.MarkAllLines();
                editor.Render();

                modified = false;
            }

            UpdateTitle();
        }

        private void OpenFilePrompt()
        {
            PromptBox prompt = new PromptBox(process, "Open File", "Enter the path to open.", "Path", (string newPath) =>
            {
                if (!newPath.Contains(':'))
                {
                    newPath = $@"0:\{newPath}";
                }
                Open(newPath);
            });
            prompt.Show();
        }

        private void SaveAsPrompt()
        {
            PromptBox prompt = new PromptBox(process, "Save As", "Enter the path to save to.", "Path", (string newPath) =>
            {
                if (!newPath.Contains(':'))
                {
                    newPath = $@"0:\{newPath}";
                }

                Open(newPath, readFile: false);

                // Check if open succeeded.
                if (path != null)
                {
                    Save();
                }
            });
            prompt.Show();
        }

        private void Save()
        {
            if (path == null)
            {
                SaveAsPrompt();
                return;
            }

            File.WriteAllText(path, editor.Text);

            modified = false;
            UpdateTitle();
        }

        private void RunClicked(int x, int y)
        {
            try
            {
                Script script = new Script(editor.Text);
                script.Lex();
                var dialogue = new RiverScriptUIConsole(process, script, "RiverScript IDE Debugger");
                dialogue.Show();
            }
            catch (Exception e)
            {
                problems.Foreground = Color.Pink;
                problems.Text = e.Message;
            }
        }

        private void Evaluate()
        {
            try
            {
                Script script = new Script(editor.Text);
                script.Lex();

                problems.Foreground = Color.LimeGreen;
                problems.Text = "No problems";
            }
            catch (Exception e)
            {
                problems.Foreground = Color.Pink;
                problems.Text = e.Message;
            }
        }

        internal void Start()
        {
            mainWindow = new AppWindow(process, 96, 96, 800, 600);
            mainWindow.Clear(Theme.Background);
            mainWindow.Closing = process.TryStop;
            UpdateTitle();
            wm.AddWindow(mainWindow);

            runButton = new Button(mainWindow, 0, 0, 60, headersHeight);
            runButton.Background = Theme.Background;
            runButton.Border = Theme.Background;
            runButton.Foreground = Color.White;
            runButton.Text = "Run";
            runButton.Image = runBitmap;
            runButton.ImageLocation = Button.ButtonImageLocation.Left;
            runButton.OnClick = RunClicked;
            wm.AddWindow(runButton);

            editor = new TextBox(mainWindow, 0, headersHeight, mainWindow.Width, mainWindow.Height - headersHeight - problemsHeight - (headersHeight * 2))
            {
                Background = Theme.CodeBackground,
                Foreground = Color.White,
                Text = "print(\"Hello World!\")",
                Changed = TextChanged,
                MultiLine = true
            };
            wm.AddWindow(editor);

            problems = new TextBox(mainWindow, 0, headersHeight + editor.Height + headersHeight, mainWindow.Width, problemsHeight + (headersHeight * 2))
            {
                Background = Theme.CodeBackground,
                Foreground = Color.Gray,
                Text = "Click Evaluate to check your program for syntax errors.",
                ReadOnly = true,
                MultiLine = true
            };
            wm.AddWindow(problems);

            mainWindow.DrawString("Problems", Color.White, 0, headersHeight + editor.Height);

            var shortcutBar = new ShortcutBar(mainWindow, runButton.Width, 0, mainWindow.Width - runButton.Width, headersHeight)
            {
                Background = Theme.Background,
                Foreground = Color.White
            };
            shortcutBar.Cells.Add(new ShortcutBarCell("Open", OpenFilePrompt)); 
            shortcutBar.Cells.Add(new ShortcutBarCell("Save", Save));
            shortcutBar.Cells.Add(new ShortcutBarCell("Save As", SaveAsPrompt));
            shortcutBar.Cells.Add(new ShortcutBarCell("Evaluate", Evaluate));
            shortcutBar.Render();
            wm.AddWindow(shortcutBar);

            wm.Update(mainWindow);
        }
    }
}
