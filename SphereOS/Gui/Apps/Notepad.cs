using Microsoft.VisualBasic;
using SphereOS.Core;
using SphereOS.Gui.UILib;
using System.Drawing;
using System.IO;

namespace SphereOS.Gui.Apps
{
    internal class Notepad : Process
    {
        internal Notepad() : base("Notepad", ProcessType.Application) { }

        internal Notepad(string path) : base("Notepad", ProcessType.Application)
        {
            this.path = path;
        }

        AppWindow window;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();
        
        SettingsService settingsService = ProcessManager.GetProcess<SettingsService>();

        TextBox textBox;

        ShortcutBar shortcutBar;

        private string? path;

        private void WindowResized()
        {
            textBox.Resize(window.Width, window.Height - 20);
            shortcutBar.Resize(window.Width, 20);

            textBox.MarkAllLines();
            textBox.Render();
        }

        internal void Open(string newPath, bool readFile = true)
        {
            if (newPath == null) return;

            if (!FileSecurity.CanAccess(Kernel.CurrentUser, path))
            {
                MessageBox messageBox = new MessageBox(this, "Notepad", $"Access to '{Path.GetFileName(newPath)}' is unauthorised.");
                messageBox.Show();
            }

            if (!File.Exists(newPath))
            {
                MessageBox messageBox = new MessageBox(this, "Notepad", $"No such file '{Path.GetFileName(newPath)}'.");
                messageBox.Show();
            }

            path = newPath;
            window.Title = $"{Path.GetFileName(newPath)} - Notepad";

            if (readFile)
            {
                textBox.Text = File.ReadAllText(path);
            }
        }

        private void OpenFilePrompt()
        {
            PromptBox prompt = new PromptBox(this, "Open File", "Enter the path to open.", "Path", (string newPath) =>
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
            PromptBox prompt = new PromptBox(this, "Save As", "Enter the path to save to.", "Path", (string newPath) =>
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
            
            File.WriteAllText(path, textBox.Text);
        }

        private void ApplyTheme()
        {
            if (settingsService.DarkNotepad)
            {
                textBox.Background = Color.Black;
                textBox.Foreground = Color.White;
            }
            else
            {
                textBox.Background = Color.White;
                textBox.Foreground = Color.Black;
            }
        }

        private void OpenViewSettings()
        {
            AppWindow settingsWindow = new AppWindow(this, 320, 264, 144, 36);
            wm.AddWindow(settingsWindow);
            settingsWindow.Title = "Settings";

            Switch darkSwitch = new Switch(settingsWindow, 8, 8, settingsWindow.Width - 16, settingsWindow.Height - 16);
            darkSwitch.Text = "Dark theme";
            darkSwitch.CheckBoxChanged = (bool @checked) => {
                settingsService.DarkNotepad = @checked;
                ApplyTheme();
            };
            wm.AddWindow(darkSwitch);

            wm.Update(settingsWindow);
        }

        internal override void Start()
        {
            base.Start();
            window = new AppWindow(this, 320, 264, 384, 240);
            wm.AddWindow(window);
            window.Title = "Notepad";
            window.Closing = TryStop;
            window.CanResize = true;
            window.UserResized = WindowResized;

            shortcutBar = new ShortcutBar(window, 0, 0, window.Width, 20);
            shortcutBar.Cells.Add(new ShortcutBarCell("Open", OpenFilePrompt));
            shortcutBar.Cells.Add(new ShortcutBarCell("Save", Save));
            shortcutBar.Cells.Add(new ShortcutBarCell("Save As", SaveAsPrompt));
            shortcutBar.Cells.Add(new ShortcutBarCell("View", OpenViewSettings));
            shortcutBar.Render();
            wm.AddWindow(shortcutBar);

            textBox = new TextBox(window, 0, 20, window.Width, window.Height - 20);
            textBox.MultiLine = true;
            wm.AddWindow(textBox);

            Open(path);

            wm.Update(window);
        }

        internal override void Run()
        {
        }
    }
}
