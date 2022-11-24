using System.Drawing;

namespace SphereOS.Gui.UILib
{
    internal static class Extensions
    {
        internal static float GetLuminance(this Color color)
        {
            return (float)((color.R * 0.2126) + (color.G * 0.7152) + (color.B * 0.0722));
        }

        internal static Color GetForegroundColour(this Color color)
        {
            return color.GetLuminance() < 140 ? Color.White : Color.Black;
        }
    }
}
