using Cosmos.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SphereOS.Gui.Apps.Paint
{
    internal abstract class Tool
    {
        internal Tool(string name)
        {
            Name = name;
        }

        internal abstract void Run(Paint paint, Window canvas, MouseState mouseState, int mouseX, int mouseY);

        internal virtual void Selected()
        {
        }

        internal virtual void Deselected()
        {
        }

        internal string Name { get; init; }
    }
}
