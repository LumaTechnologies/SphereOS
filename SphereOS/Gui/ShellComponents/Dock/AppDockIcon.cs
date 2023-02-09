using Cosmos.System.Graphics;
using SphereOS.Core;
using SphereOS.Gui.UILib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SphereOS.Gui.ShellComponents.Dock
{
    internal class AppDockIcon : BaseDockIcon
    {
        internal AppDockIcon(AppWindow appWindow) : base(
            image: appWindow.Icon,
            doAnimation: true)
        {
            AppWindow = appWindow;
        }

        internal AppWindow AppWindow { get; init; }

        internal override void Clicked()
        {
            ProcessManager.GetProcess<WindowManager>().Focus = AppWindow;
        }
    }
}
