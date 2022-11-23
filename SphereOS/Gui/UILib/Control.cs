using SphereOS.Core;
using System;

namespace SphereOS.Gui.UILib
{
    internal abstract class Control : Window
    {
        internal Control(Window parent, int x, int y, int width, int height) : base(parent.Process, x, y, width, height)
        {
            WM = ProcessManager.GetProcess<WindowManager>();

            RelativeTo = parent;

            Render();
        }

        protected WindowManager WM;

        internal abstract void Render();
    }
}
