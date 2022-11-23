using SphereOS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SphereOS.Gui
{
    internal class SettingsService : Process
    {
        public SettingsService() : base("SettingsService", ProcessType.Service)
        {
        }

        internal bool twelveHourTime { get; set; }
        internal bool leftHandStartButton { get; set; }

        internal override void Start()
        {
            base.Start();
        }

        internal override void Run()
        {
        }

        internal override void Stop()
        {
            base.Stop();
        }
    }
}
