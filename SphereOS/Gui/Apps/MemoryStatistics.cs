using SphereOS.Core;
using SphereOS.Core.Memory;
using SphereOS.Gui.SmoothMono;
using SphereOS.Gui.UILib;
using System;
using System.Drawing;

namespace SphereOS.Gui.Apps
{
    internal class MemoryStatistics : Process
    {
        internal MemoryStatistics() : base("MemoryStatistics", ProcessType.Application) { }

        AppWindow window;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        private int lastSecond;

        private static int padding = 12;
        private static int barHeight = 12;
        private static Color barColour = Color.FromArgb(0, 155, 254);

        private void Update()
        {
            window.Clear(Color.LightGray);

            var statistics = MemoryStatisticsProvider.GetMemoryStatistics();

            window.DrawString("Memory Statistics", Color.DarkBlue, padding, padding);

            window.DrawString(string.Format(@"Total: {0} MB
Unavailable: {1} MB
Used: {2:d1} MB
Free: {3} MB
Percentage Used: {4:d1}%",
            statistics.TotalMB,
            statistics.UnavailableMB,
            statistics.UsedMB,
            statistics.FreeMB,
            statistics.PercentUsed), Color.Black, padding, padding + FontData.Height + padding);

            window.DrawFilledRectangle(0, window.Height - barHeight, window.Width, barHeight, Color.Black);
            window.DrawFilledRectangle(0, window.Height - barHeight, (int)(window.Width * ((float)statistics.PercentUsed / 100f)), barHeight, barColour);

            wm.Update(window);
        }

        internal override void Start()
        {
            base.Start();
            window = new AppWindow(this, 256, 256, 256, 192);
            wm.AddWindow(window);
            window.Title = "Memory Statistics";
            window.Icon = AppManager.GetAppMetadata("Memory Statistics").Icon;
            window.Closing = TryStop;

            Update();
        }

        internal override void Run()
        {
            int second = DateTime.Now.Second;
            if (lastSecond != second)
            {
                lastSecond = second;
                Update();
            }
        }
    }
}
