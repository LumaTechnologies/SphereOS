using Cosmos.HAL.Drivers.PCI.Video;
using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SphereOS.Paint
{
    internal static class TextRenderer
    {
        /// <summary>
        /// The width of the system font.
        /// </summary>
        internal static int FontWidth;

        /// <summary>
        /// The height of the system font.
        /// </summary>
        internal static int FontHeight;

        /// <summary>
        /// Load the system fonts.
        /// </summary>
        internal static void Initialize()
        {
            FontWidth = FontData.Width;
            FontHeight = FontData.Height;
            //Font = PCScreenFont.Default;
        }

        private static Color AlphaBlend(Color to, Color from, byte alpha)
        {
            byte R = (byte)((to.R * alpha + from.R * (255 - alpha)) >> 8);
            byte G = (byte)((to.G * alpha + from.G * (255 - alpha)) >> 8);
            byte B = (byte)((to.B * alpha + from.B * (255 - alpha)) >> 8);
            return Color.FromArgb(R, G, B);
        }

        private static void DrawSmoothChar(char c, int x, int y, VMWareSVGAII driver, Color background, Color foreground)
        {
            byte[] bytes = FontData.Chars[c];
            if (bytes != null)
            {
                for (int i = 0; i < FontData.Width; i++)
                {
                    for (int j = 0; j < FontData.Height; j++)
                    {
                        byte alpha = bytes[(j * FontData.Width) + i];
                        if (alpha == 0) continue;

                        int finalX = x + i;
                        int finalY = y + j;

                        Color color;
                        if (background == Color.Transparent)
                        {
                            color = AlphaBlend(foreground, Color.FromArgb((int)driver.GetPixel((uint)finalX, (uint)finalY)), alpha);
                        }
                        else
                        {
                            color = AlphaBlend(foreground, background, alpha);
                        }

                        driver.SetPixel((uint)finalX, (uint)finalY, (uint)color.ToArgb());
                    }
                }
            }
        }

        internal static void Render(string text, int x, int y, VMWareSVGAII driver, Color background, Color foreground)
        {
            for (int i = 0; i < text.Length; i++)
            {
                DrawSmoothChar(text[i], x + (FontWidth * i), y, driver, background, foreground);
            }
        }
    }
}
