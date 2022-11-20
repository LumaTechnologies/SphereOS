using Cosmos.System.Graphics;

namespace SphereOS.Apps.Paint
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
