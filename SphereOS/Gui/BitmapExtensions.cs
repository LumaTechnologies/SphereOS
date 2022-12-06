using Cosmos.System.Graphics;
using System;

namespace SphereOS.Gui
{
    internal static class BitmapExtensions
    {
        internal static Bitmap Resize(this Bitmap bmp, uint width, uint height)
        {
            if (bmp.Width == width && bmp.Height == height)
            {
                return bmp;
            }

            if (bmp.Depth != ColorDepth.ColorDepth32)
            {
                throw new Exception("Resize can only resize images with a colour depth of 32.");
            }

            Bitmap res = new Bitmap(width, height, ColorDepth.ColorDepth32);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double xDouble = (double)x / (double)width;
                    double yDouble = (double)y / (double)height;

                    uint origX = (uint)((double)bmp.Width * xDouble);
                    uint origY = (uint)((double)bmp.Height * yDouble);

                    res.rawData[y * width + x] = bmp.rawData[(origY * bmp.Width) + origX];
                }
            }

            return res;
        }

        internal static Bitmap ResizeWidthKeepRatio(this Bitmap bmp, uint width)
        {
            return Resize(bmp, width, (uint)((double)bmp.Height * ((double)width / (double)bmp.Width)));
        }

        internal static Bitmap ResizeHeightKeepRatio(this Bitmap bmp, uint height)
        {
            return Resize(bmp, (uint)((double)bmp.Width * ((double)height / (double)bmp.Height)), height);
        }
    }
}
