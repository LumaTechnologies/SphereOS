using SphereOS.Core;
using SphereOS.Gui.UILib;
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

        TextBox textBox;

        string path;

        private void WindowResized()
        {
            textBox.Resize(window.Width, window.Height);

            textBox.MarkAllLines();
            textBox.Render();
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

            textBox = new TextBox(window, 0, 0, window.Width, window.Height);
            textBox.MultiLine = true;
            if (path != null)
            {
                if (FileSecurity.CanAccess(user: null, path))
                {
                    textBox.Text = File.ReadAllText(path);
                }
                else
                {
                    MessageBox messageBox = new MessageBox(this, "Notepad", $"Access to {Path.GetFileName(path)} is unauthorised.");
                    messageBox.Show();
                }
            }
            wm.AddWindow(textBox);

            wm.Update(window);
        }

        internal override void Run()
        {
        }
    }
}
