using Cosmos.System.Graphics;
using SphereOS.Gui.UILib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SphereOS.Gui.ShellComponents.Dock
{
    internal class StartMenuDockIcon : BaseDockIcon
    {
        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.Dock.StartMenu.bmp")]
        private static byte[] _iconBytes_StartMenu;
        internal static Bitmap Icon_StartMenu = new Bitmap(_iconBytes_StartMenu);

        internal StartMenuDockIcon() : base(
            image: Icon_StartMenu,
            doAnimation: false)
        {
        }

        internal override void Clicked()
        {
            StartMenu.CurrentStartMenu.ToggleStartMenu();
        }
    }
}
