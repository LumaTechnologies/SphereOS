using System.Drawing;

namespace SphereOS.Gui.SmoothMono
{
    public static class TextRenderer
    {
        private static uint FastBlend(uint src, uint dst, uint t)
        {
            uint s = 255 - t;
            return (
                (uint)(((((src >> 0) & 0xff) * s +
                    ((dst >> 0) & 0xff) * t) >> 8) |
                (((((src >> 8) & 0xff) * s +
                    ((dst >> 8) & 0xff) * t)) & ~0xff) |
                (((((src >> 16) & 0xff) * s +
                    ((dst >> 16) & 0xff) * t) << 8) & ~0xffff) |
                0xff000000)
            );
        }

        private static void DrawChar(char c, int color, int[] buffer, int bufferWidth, int bufferHeight, int x, int y)
        {
            byte[] bytes = FontData.Chars[c];
            if (bytes != null)
            {
                for (int i = 0; i < FontData.Width; i++)
                {
                    int finalX = x + i;
                    if (finalX < 0 || finalX >= bufferWidth) continue;

                    for (int j = 0; j < FontData.Height; j++)
                    {
                        int finalY = y + j;
                        if (finalY < 0 || finalY >= bufferHeight) continue;

                        byte invAlpha = (byte)(255 - bytes[(j * FontData.Width) + i]);
                        if (invAlpha == 255) continue;

                        int k = ((finalY * bufferWidth) + finalX);

                        buffer[k] = (int)FastBlend((uint)color, (uint)buffer[k], invAlpha);
                    }
                }
            }
        }

        public static void DrawString(string str, Color color, int[] buffer, int bufferWidth, int bufferHeight, int x, int y)
        {
            int charX = x;
            int charY = y;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '\n')
                {
                    charX = x;
                    charY += FontData.Height;
                    continue;
                }
                DrawChar(str[i], color.ToArgb(), buffer, bufferWidth, bufferHeight, charX, charY);
                charX += FontData.Width;
            }
        }
    }
}
