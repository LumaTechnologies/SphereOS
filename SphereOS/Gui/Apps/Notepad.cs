using Microsoft.VisualBasic;
using SphereOS.Core;
using SphereOS.Gui.SmoothMono;
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

        private bool modified = false;

        private void TextChanged()
        {
            modified = true;

            UpdateTitle();
        }

        private void WindowResized()
        {
            textBox.Resize(window.Width, window.Height - 20);
            shortcutBar.Resize(window.Width, 20);

            shortcutBar.Render();

            textBox.MarkAllLines();
            textBox.Render();
        }

        private void UpdateTitle()
        {
            if (path == null)
            {
                window.Title = "Untitled - Notepad";
            }
            if (modified)
            {
                window.Title = $"{Path.GetFileName(path)}* - Notepad";
            }
            else
            {
                window.Title = $"{Path.GetFileName(path)} - Notepad";
            }
        }

        internal void Open(string newPath, bool readFile = true)
        {
            if (newPath == null) return;

            if (!FileSecurity.CanAccess(Kernel.CurrentUser, path))
            {
                MessageBox messageBox = new MessageBox(this, "Notepad", $"Access to '{Path.GetFileName(newPath)}' is unauthorised.");
                messageBox.Show();
            }

            if (readFile && !File.Exists(newPath))
            {
                MessageBox messageBox = new MessageBox(this, "Notepad", $"No such file '{Path.GetFileName(newPath)}'.");
                messageBox.Show();
            }

            path = newPath;

            if (readFile)
            {
                textBox.Text = File.ReadAllText(path);

                textBox.MarkAllLines();
                textBox.Render();

                modified = false;
            }

            UpdateTitle();
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

            modified = false;
            UpdateTitle();
        }

        private void ApplyTheme()
        {
            if (settingsService.DarkNotepad)
            {
                textBox.Background = Color.FromArgb(24, 24, 30);
                textBox.Foreground = Color.White;

                shortcutBar.Background = Color.FromArgb(56, 56, 71);
                shortcutBar.Foreground = Color.White;
            }
            else
            {
                textBox.Background = Color.White;
                textBox.Foreground = Color.Black;

                shortcutBar.Background = Color.LightGray;
                shortcutBar.Foreground = Color.Black;
            }

            textBox.MarkAllLines();
            textBox.Render();
        }

        private void OpenViewSettings()
        {
            AppWindow settingsWindow = new AppWindow(this, 320, 264, 256, 192);
            settingsWindow.DrawString("Notepad Settings", Color.DarkBlue, 12, 12);
            settingsWindow.DrawString($"Notepad v{Kernel.Version}", Color.DarkGray, 12, settingsWindow.Height - 12 - FontData.Height);
            wm.AddWindow(settingsWindow);
            settingsWindow.Title = "Notepad";

            Switch darkSwitch = new Switch(settingsWindow, 12, 40, settingsWindow.Width - 16, 20);
            darkSwitch.Text = "Dark theme";
            darkSwitch.Checked = settingsService.DarkNotepad;
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
            UpdateTitle();
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
            textBox.Changed = TextChanged;
            wm.AddWindow(textBox);

            ApplyTheme();

            Open(path);

            wm.Update(window);
        }

        internal override void Run()
        {
        }
    }
}
