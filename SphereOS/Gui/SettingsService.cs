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

        private bool _twelveHourTime = false;
        internal bool TwelveHourTime
        {
            get
            {
                return _twelveHourTime;
            }
            set
            {
                _twelveHourTime = value;
                ApplySettings();
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
                ApplySettings();
            }
        }

        private void ApplySettings()
        {
            ProcessManager.GetProcess<Taskbar>().SetLeftHandStartButton(LeftHandStartButton);
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

            File.WriteAllText(settingsPath, builder.ToString());
        }
    }
}
