
using Cosmos.System.Graphics;
using SphereOS.Core;
using SphereOS.Gui.UILib;
using System;
using System.Drawing;

namespace SphereOS.Gui.ShellComponents
{
    internal class Taskbar : Process
    {
        internal Taskbar() : base("Taskbar", ProcessType.Application)
        {
            Critical = true;
        }

        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.Start.bmp")]
        private static byte[] startBytes;
        private static Bitmap startBitmap = new Bitmap(startBytes);

        Window window;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        DateTime lastDate = DateTime.Now;

        TextBlock time;

        ImageBlock start;

        SettingsService settingsService;

        private bool miniCalendarOpen = false;
        private Calendar miniCalendar;

        private int timeUpdateTicks = 0;

        internal void SetLeftHandStartButton(bool left)
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

        internal void UpdateTime()
        {
            if (settingsService == null)
            {
                settingsService = ProcessManager.GetProcess<SettingsService>();
            }

            string timeText;
            if (settingsService.TwelveHourClock)
            {
                timeText = DateTime.Now.ToString("h:mm tt");
            }
            else
            {
                timeText = DateTime.Now.ToString("HH:mm");
            }
            if (time.Text != timeText)
            {   
                time.Text = timeText;
            }
        }

        private void StartClicked(int x, int y)
        {
            StartMenu.CurrentStartMenu.ToggleStartMenu();
        }

        private void TimeClicked(int x, int y)
        {
            miniCalendarOpen = !miniCalendarOpen;
            if (miniCalendarOpen)
            {
                miniCalendar = new Calendar(window, window.Width - 256, window.Height, 256, 256);
                miniCalendar.Background = Color.FromArgb(56, 56, 71);
                miniCalendar.TodayBackground = Color.FromArgb(77, 77, 91);
                miniCalendar.Foreground = Color.White;
                miniCalendar.WeekendForeground = Color.LightPink;
                wm.AddWindow(miniCalendar);
                wm.Update(miniCalendar);
            }
            else
            {
                wm.RemoveWindow(miniCalendar);
            }
        }

        #region Process
        internal override void Start()
        {
            base.Start();
            window = new Window(this, 0, 0, (int)wm.ScreenWidth, 24);
            window.Clear(Color.Black);
            wm.AddWindow(window);

            time = new TextBlock(window, window.Width - 72, 0, 64, window.Height);
            time.Background = Color.Black;
            time.Foreground = Color.White;
            time.HorizontalAlignment = Alignment.End;
            time.VerticalAlignment = Alignment.Middle;
            time.OnClick = TimeClicked;
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
            timeUpdateTicks++;
            if (timeUpdateTicks % 100 == 0)
            {
                UpdateTime();
            }
        }
        #endregion
    }
}
