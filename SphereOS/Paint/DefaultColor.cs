using System.Drawing;

namespace SphereOS.Paint
{
    internal class DefaultColor
    {
        internal DefaultColor(Color color, string name)
        {
            Color = color;
            Name = name;
        }

        internal Color Color { get; set; }

        internal string Name { get; set; }

        internal int ButtonX { get; set; }
        internal int ButtonY { get; set; }
        internal int ButtonWidth { get; set; }
        internal int ButtonHeight { get; set; }
    }
}
