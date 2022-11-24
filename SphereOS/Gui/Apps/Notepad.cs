using SphereOS.Core;
using SphereOS.Gui.UILib;

namespace SphereOS.Gui.Apps
{
    internal class Notepad : Process
    {
        internal Notepad() : base("Notepad", ProcessType.Application) { }

        AppWindow window;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        TextBox textBox;

        internal override void Start()
        {
            base.Start();
            window = new AppWindow(this, 320, 264, 384, 240);
            wm.AddWindow(window);
            window.Title = "Notepad";
            window.Closing = TryStop;

            textBox = new TextBox(window, 0, 0, window.Width, window.Height);
            wm.AddWindow(textBox);

            wm.Update(window);
        }

        internal override void Run()
        {

        }
    }
}
