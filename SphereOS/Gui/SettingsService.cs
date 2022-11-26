using SphereOS.Core;
using SphereOS.Gui.ShellComponents;
using SphereOS.Text;
using System.IO;

namespace SphereOS.Gui
{
    internal class SettingsService : Process
    {
        public SettingsService() : base("SettingsService", ProcessType.Service)
        {
        }

        private const string settingsPath = @"0:\settings.ini";

        private bool _twelveHourClock = false;
        internal bool TwelveHourClock
        {
            get
            {
                return _twelveHourClock;
            }
            set
            {
                _twelveHourClock = value;
                ProcessManager.GetProcess<Taskbar>()?.UpdateTime();
            }
        }

        private bool _leftHandStartButton = false;
        internal bool LeftHandStartButton
        {
            get
            {
                return _leftHandStartButton;
            }
            set
            {
                _leftHandStartButton = value;
                ProcessManager.GetProcess<Taskbar>()?.SetLeftHandStartButton(LeftHandStartButton);
            }
        }

        internal override void Start()
        {
            base.Start();
            if (File.Exists(settingsPath))
            {
                IniReader reader = new IniReader(File.ReadAllText(settingsPath));
                if (reader.TryReadBool("LeftHandStartButton", out bool value, section: "Appearance"))
                {
                    LeftHandStartButton = value;
                }
                if (reader.TryReadBool("TwelveHourClock", out bool value1, section: "DateAndTime"))
                {
                    TwelveHourClock = value1;
                }
            }
        }

        internal override void Run()
        {
        }

        internal override void Stop()
        {
            base.Stop();
            IniBuilder builder = new IniBuilder();

            /* Appearance */
            builder.BeginSection("Appearance");
            builder.AddKey("LeftHandStartButton", LeftHandStartButton);

            /* Date & Time */
            builder.BeginSection("DateAndTime");
            builder.AddKey("TwelveHourClock", TwelveHourClock);

            File.WriteAllText(settingsPath, builder.ToString());
        }
    }
}
