using Cosmos.System.Graphics;
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

        private Mode _mode = new Mode(1280, 800, ColorDepth.ColorDepth32);
        internal Mode Mode
        {
            get
            {
                return _mode;
            }
            set
            {
                _mode = value;
            }
        }

        internal void Flush()
        {
            IniBuilder builder = new IniBuilder();

            /* Appearance */
            builder.BeginSection("Appearance");

            builder.AddKey("LeftHandStartButton", LeftHandStartButton);

            builder.AddKey("ScreenWidth", Mode.Width);
            builder.AddKey("ScreenHeight", Mode.Height);

            /* Date & Time */
            builder.BeginSection("DateAndTime");
            builder.AddKey("TwelveHourClock", TwelveHourClock);

            File.WriteAllText(settingsPath, builder.ToString());
        }

        #region Process
        internal override void Start()
        {
            base.Start();
            if (File.Exists(settingsPath))
            {
                IniReader reader = new IniReader(File.ReadAllText(settingsPath));

                /* Appearance */
                if (reader.TryReadBool("LeftHandStartButton", out bool value, section: "Appearance"))
                {
                    LeftHandStartButton = value;
                }
                if (reader.TryReadInt("ScreenWidth", out int width, section: "Appearance"))
                {
                    if (reader.TryReadInt("ScreenHeight", out int height, section: "Appearance"))
                    {
                        _mode = new Mode(width, height, ColorDepth.ColorDepth32);
                    }
                }

                /* Date & Time */
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
            Flush();
        }
        #endregion
    }
}
