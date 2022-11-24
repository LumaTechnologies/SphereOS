namespace SphereOS.Gui.UILib
{
    internal abstract class Control : Window
    {
        internal Control(Window parent, int x, int y, int width, int height) : base(parent.Process, x, y, width, height)
        {
            RelativeTo = parent;

            Render();
        }

        internal abstract void Render();
    }
}
