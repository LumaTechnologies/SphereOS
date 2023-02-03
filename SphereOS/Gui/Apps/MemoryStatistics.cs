using SphereOS.Core;
using SphereOS.Core.Memory;
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
        
        private void Update()
        {
            window.Clear(Color.LightGray);

            var statistics = MemoryStatisticsProvider.GetMemoryStatistics();

            window.DrawString(string.Format(@$"
Total: {0} MB
Unavailable: {1} MB
Used: {2} MB
Free: {3} MB
Percentage Used: {4}%
", statistics.TotalMB, statistics.UnavailableMB, statistics.UsedMB, statistics.FreeMB, statistics.UsedMB), Color.Black, 12, 12);

            wm.Update(window);
        }

        internal override void Start()
        {
            base.Start();
            window = new AppWindow(this, 256, 256, 256, 192);
            wm.AddWindow(window);
            window.Title = "Memory Statistics";
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
