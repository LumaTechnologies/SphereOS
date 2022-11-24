using Cosmos.System.Graphics;
using SphereOS.Gui.Apps;
using System.Collections.Generic;
using System.Drawing;

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
            private static byte[] _iconBytes_Info;
            internal static Bitmap Icon_Info = new Bitmap(_iconBytes_Info);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.AppIcons.Settings.bmp")]
            private static byte[] _iconBytes_Settings;
            internal static Bitmap Icon_Settings = new Bitmap(_iconBytes_Settings);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.AppIcons.Clock.bmp")]
            private static byte[] _iconBytes_Clock;
            internal static Bitmap Icon_Clock = new Bitmap(_iconBytes_Clock);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.AppIcons.Tasks.bmp")]
            private static byte[] _iconBytes_Tasks;
            internal static Bitmap Icon_Tasks = new Bitmap(_iconBytes_Tasks);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.AppIcons.Calculator.bmp")]
            private static byte[] _iconBytes_Calculator;
            internal static Bitmap Icon_Calculator = new Bitmap(_iconBytes_Calculator);

            /*[IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.AppIcons.CodeStudio.bmp")]
            private static byte[] _iconBytes_CodeStudio;
            internal static Bitmap Icon_CodeStudio = new Bitmap(_iconBytes_CodeStudio);*/

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.AppIcons.Notepad.bmp")]
            private static byte[] _iconBytes_Notepad;
            internal static Bitmap Icon_Notepad = new Bitmap(_iconBytes_Notepad);
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

            RegisterApp(new App("Info", () => { return new Info(); }, Icons.Icon_Info, Color.FromArgb(0, 161, 255)));
            RegisterApp(new App("Settings", () => { return new Settings(); }, Icons.Icon_Settings, Color.FromArgb(0, 115, 186)));
            RegisterApp(new App("Clock", () => { return new Clock(); }, Icons.Icon_Clock, Color.FromArgb(255, 86, 71)));
            RegisterApp(new App("Tasks", () => { return new Tasks(); }, Icons.Icon_Tasks, Color.FromArgb(204, 241, 255)));
            RegisterApp(new App("Calculator", () => { return new Calculator(); }, Icons.Icon_Calculator, Color.FromArgb(0, 157, 255)));
            //RegisterApp(new App("CodeStudio", () => { return new Apps.CodeStudio.CodeStudio(); }, Icons.Icon_CodeStudio, Color.FromArgb(127, 0, 255)));
            RegisterApp(new App("Notepad", () => { return new Notepad(); }, Icons.Icon_Notepad, Color.FromArgb(14, 59, 76)));

            appsLoaded = true;
        }
    }
}
