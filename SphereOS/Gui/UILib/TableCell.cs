using Cosmos.System.Graphics;
using System.Drawing;

namespace SphereOS.Gui.UILib
{
    internal class TableCell
    {
        internal TableCell(string text)
        {
            Text = text;
        }

        internal TableCell(string text, object tag)
        {
            Text = text;
            Tag = tag;
        }

        internal TableCell(Bitmap image, string text)
        {
            Image = image;
            Text = text;
        }

        internal TableCell(Bitmap image, string text, object tag)
        {
            Image = image;
            Text = text;
            Tag = tag;
        }

        internal Bitmap Image { get; set; }

        internal string Text { get; set; } = string.Empty;

        internal object Tag { get; set; }

        internal Color? BackgroundColourOverride { get; set; } = null;

        internal Color? ForegroundColourOverride { get; set; } = null;
    }
}
