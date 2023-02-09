using SphereOS.Core;
using SphereOS.Gui.UILib;
using System.Drawing;

namespace SphereOS.Gui.Apps
{
    internal class Info : Process
    {
        internal Info() : base("Info", ProcessType.Application) { }

        AppWindow window;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        internal override void Start()
        {
            base.Start();
            window = new AppWindow(this, 256, 256, 320, 256);
            wm.AddWindow(window);
            window.Title = "Info";
            window.Icon = AppManager.GetAppMetadata("Info").Icon;
            window.Closing = TryStop;

            window.Clear(Color.LightGray);
            window.DrawFilledRectangle(0, 0, window.Width, 40, Color.Black);
            window.DrawString("SphereOS", System.Drawing.Color.White, 12, 12);

            window.DrawString($"OS: SphereOS {Kernel.Version}", Color.Black, 12, 52);
            window.DrawString($"Memory: {Cosmos.Core.CPU.GetAmountOfRAM()} MB", Color.Black, 12, 80);

            window.DrawString("Credits", Color.DarkBlue, 12, 108);
            window.DrawString("Cosmos Team - OS tooling", Color.Black, 12, 132);
            window.DrawString("Microsoft - .NET Runtime", Color.Black, 12, 156);
            window.DrawString("Google Fonts - Font", Color.Black, 12, 180);

            Button button = new Button(window, window.Width - 80 - 12, window.Height - 20 - 12, 80, 20);
            button.Text = "OK";
            button.OnClick = (int x, int y) =>
            {
                wm.RemoveWindow(window);
            };
            wm.AddWindow(button);

            wm.Update(window);
        }

        internal override void Run()
        {

        }
    }
}
