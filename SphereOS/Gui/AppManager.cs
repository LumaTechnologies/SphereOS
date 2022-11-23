using Cosmos.System.Graphics;
using SphereOS.Core;
using SphereOS.Gui.Apps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SphereOS.Gui
{
    internal static class AppManager
    {
        internal static List<App> Apps { get; private set; } = new List<App>();

        private static bool appsLoaded = false;

        private static class Icons
        {
            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.AppIcons.Default.bmp")]
            private static byte[] _iconBytes_Default;
            internal static Bitmap Icon_Default = new Bitmap(_iconBytes_Default);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.AppIcons.Info.bmp")]
            private static byte[] _iconBytes_TestApp;
            internal static Bitmap Icon_TestApp = new Bitmap(_iconBytes_TestApp);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.AppIcons.Settings.bmp")]
            private static byte[] _iconBytes_Settings;
            internal static Bitmap Icon_Settings = new Bitmap(_iconBytes_Settings);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.AppIcons.Clock.bmp")]
            private static byte[] _iconBytes_Clock;
            internal static Bitmap Icon_Clock = new Bitmap(_iconBytes_Clock);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.AppIcons.Tasks.bmp")]
            private static byte[] _iconBytes_Tasks;
            internal static Bitmap Icon_Tasks = new Bitmap(_iconBytes_Tasks);
        }

        internal static void RegisterApp(App app)
        {
            Apps.Add(app);
        }

        internal static void LoadAllApps()
        {
            if (appsLoaded)
            {
                return;
            }

            RegisterApp(new App("Info", () => { return new Info(); }, Icons.Icon_TestApp));
            RegisterApp(new App("Settings", () => { return new Settings(); }, Icons.Icon_Settings));
            RegisterApp(new App("Clock", () => { return new Clock(); }, Icons.Icon_Clock));
            RegisterApp(new App("Tasks", () => { return new Tasks(); }, Icons.Icon_Tasks));
            RegisterApp(new App("Calculator", () => { return new Calculator(); }, Icons.Icon_Default));

            appsLoaded = true;
        }
    }
}
