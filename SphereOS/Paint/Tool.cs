using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.System;

namespace SphereOS.Paint
{
    internal abstract class Tool
    {
        internal Tool(string name, Bitmap icon)
        {
            Name = name;
            Icon = icon;
        }

        internal string Name { get; private set; }

        internal Bitmap Icon { get; private set; }

        internal int ButtonX { get; set; }
        internal int ButtonY { get; set; }
        internal int ButtonWidth { get; set; }
        internal int ButtonHeight { get; set; }

        internal abstract void Run(MouseState mouseState,
            int mouseX,
            int mouseY,
            Document doc,
            Paint paint);
    }
}
