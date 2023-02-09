using Cosmos.System.Graphics;
using SphereOS.Core;
using SphereOS.Gui.UILib;
using System;
using System.Drawing;

namespace SphereOS.Gui.Apps
{
    internal class Clock : Process
    {
        internal Clock() : base("Clock", ProcessType.Application) { }

        AppWindow window;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        private int lastSecond = DateTime.Now.Second;

        private SettingsService settingsService;

        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.Clock.ClockBackground.bmp")]
        private static byte[] clockBackgroundBytes;
        private static Bitmap clockBackgroundBitmap = new Bitmap(clockBackgroundBytes);

        private void RenderHand(int originX, int originY, int handLength, double radians, Color color)
        {
            int x = originX + (int)(handLength * Math.Sin(radians));
            int y = originY - (int)(handLength * Math.Cos(radians));
            window.DrawLine(originX, originY, x, y, color);
        }

        private void RenderClock()
        {
            if (settingsService == null)
            {
                settingsService = ProcessManager.GetProcess<SettingsService>();
            }

            DateTime now = DateTime.Now;

            string timeText;
            if (settingsService.TwelveHourClock)
            {
                timeText = DateTime.Now.ToString("h:mm:ss tt");
            }
            else
            {
                timeText = DateTime.Now.ToString("HH:mm:ss");
            }

            int originX = window.Width / 2;
            int originY = window.Height / 2;
            int diameter = (int)(Math.Min(window.Width, window.Height) * 0.75f);
            int radius = diameter / 2;

            /* Background */
            if (window.Width == 192 && window.Height == 192)
            {
                window.DrawImage(clockBackgroundBitmap, 0, 0);
            }
            else
            {
                window.Clear(Color.White);
                window.DrawCircle(originX, originY, radius, Color.Black);

                for (int i = 1; i <= 12; i++)
                {
                    int numX = (int)(originX + (Math.Sin(i * Math.PI / 6) * radius * 0.8)) - 4;
                    int numY = (int)(originY - Math.Cos(i * Math.PI / 6) * radius * 0.8) - 8;
                    window.DrawString(i.ToString(), Color.Black, numX, numY);
                }
            }

            window.DrawString(timeText, Color.Black, (window.Width / 2) - ((timeText.Length * 8) / 2), 4);

            /* Second hand */
            double second = now.Second;
            double secondRad = second * Math.PI / 30;
            RenderHand(originX, originY, radius, secondRad, Color.Red);

            /* Minute hand*/
            double minute = now.Minute + (second / 60);
            double minuteRad = minute * Math.PI / 30;
            RenderHand(originX, originY, (int)(radius * 0.75f), minuteRad, Color.Black);

            /* Hour hand */
            double hour = now.Hour + (minute / 60);
            double hourRad = hour * Math.PI / 6;
            RenderHand(originX, originY, (int)(radius * 0.5f), hourRad, Color.Black);

            wm.Update(window);
        }

        #region Process
        internal override void Start()
        {
            base.Start();
            window = new AppWindow(this, 256, 256, 192, 192);
            window.Icon = AppManager.GetAppMetadata("Clock").Icon;
            window.CanResize = true;
            window.UserResized = RenderClock;
            window.Closing = TryStop;
            wm.AddWindow(window);

            window.Title = "Clock";

            RenderClock();
        }


        internal override void Run()
        {
            DateTime now = DateTime.Now;
            if (lastSecond != now.Second)
            {
                RenderClock();
                lastSecond = now.Second;
            }
        }
        #endregion Process
    }
}
