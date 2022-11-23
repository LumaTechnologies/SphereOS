
using Cosmos.System.Graphics;
using SphereOS.Core;
using SphereOS.Gui.UILib;
using System;
using System.Drawing;

namespace SphereOS.Gui.ShellComponents
{
    internal class Taskbar : Process
    {
        internal Taskbar() : base("Taskbar", ProcessType.Application) { }

        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.Start.bmp")]
        private static byte[] startBytes;
        private static Bitmap startBitmap = new Bitmap(startBytes);

        Window window;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        DateTime lastDate = DateTime.Now;

        TextBlock time;

        ImageBlock start;

        internal void SetLeftStartButton(bool left)
        {
            if (left)
            {
                start.X = 0;
            }
            else
            {
                start.X = (int)((window.Width / 2) - (startBitmap.Width / 2));
            }
        }

        internal int GetTaskbarHeight()
        {
            return window.Height;
        }

        private void UpdateTime()
        {
            string timeText = DateTime.Now.ToString("HH:mm");
            if (time.Text != timeText)
            {
                time.Text = timeText;
            }
        }

        private void StartClicked(int x, int y)
        {
            StartMenu startMenu = ProcessManager.GetProcess<StartMenu>();
            if (startMenu == null)
            {
                startMenu = (StartMenu)ProcessManager.AddProcess(this, new StartMenu());
                startMenu.Start();
            }
            startMenu.ToggleStartMenu();
        }

        #region Process
        internal override void Start()
        {
            base.Start();
            window = new Window(this, 0, 0, (int)wm.ScreenWidth, 24);
            window.Clear(Color.Black);
            wm.AddWindow(window);

            time = new TextBlock(window, window.Width - 48, 4, 40, 16);
            time.Background = Color.Black;
            time.Foreground = Color.White;
            wm.AddWindow(time);

            start = new ImageBlock(window, (int)((window.Width / 2) - startBitmap.Width / 2), 0, 24, 24);
            start.Image = startBitmap;
            start.OnClick = StartClicked;
            wm.AddWindow(start);

            UpdateTime();

            wm.Update(window);
        }

        internal override void Run()
        {
            UpdateTime();
        }
        #endregion
    }
}
