using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SphereOS.Paint
{
    internal class Button
    {
        internal Button(Image image)
        {
            Image = image;
        }

        internal Image Image { get; set; }

        internal int ButtonX { get; set; }
        internal int ButtonY { get; set; }
        internal int ButtonWidth { get; set; }
        internal int ButtonHeight { get; set; }
    }
}
