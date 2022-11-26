using SphereOS.Core;
using SphereOS.Gui.UILib;
using System;
using System.Drawing;

namespace SphereOS.Gui.Apps
{
    internal class Calendar : Process
    {
        internal Calendar() : base("Calendar", ProcessType.Application) { }

        AppWindow window;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        UILib.Calendar cal;

        Button nextButton;

        Button prevButton;


        private void PrevClicked(int x, int y)
        {
            if (cal.Month == 1)
            {
                cal.Month = 12;
                cal.Year--;
            }
            else
            {
                cal.Month--;
            }
        }

        private void NextClicked(int x, int y)
        {
            if (cal.Month == 12)
            {
                cal.Month = 1;
                cal.Year++;
            }
            else
            {
                cal.Month++;
            }
        }

        private void WindowResized()
        {
            cal.Resize(window.Width, window.Height);

            cal.Render();
        }

        internal override void Start()
        {
            base.Start();
            window = new AppWindow(this, 320, 256, 384, 288);
            wm.AddWindow(window);
            window.Title = "Calendar";
            window.CanResize = true;
            window.UserResized = WindowResized;
            window.Closing = TryStop;

            cal = new UILib.Calendar(window, 0, 0, window.Width, window.Height);
            wm.AddWindow(cal);

            DateTime now = DateTime.Now;
            cal.Year = now.Year;
            cal.Month = now.Month;

            prevButton = new Button(window, 8, 8, 24, 24);
            prevButton.Text = "<";
            prevButton.OnClick = PrevClicked;
            wm.AddWindow(prevButton);

            nextButton = new Button(window, 40, 8, 24, 24);
            nextButton.Text = ">";
            nextButton.OnClick = NextClicked;
            wm.AddWindow(nextButton);

            wm.Update(window);
        }

        internal override void Run()
        {
        }
    }
}
