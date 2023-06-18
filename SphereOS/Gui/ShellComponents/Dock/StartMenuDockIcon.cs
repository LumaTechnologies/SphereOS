using Cosmos.System.Graphics;

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
