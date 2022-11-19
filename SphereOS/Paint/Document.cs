using Cosmos.Core;
using System;
using System.Drawing;
using System.IO;

namespace SphereOS.Paint
{
    internal class Document
    {
        internal Document(string path)
        {
            Path = path;
            if (path != null && File.Exists(path))
            {
                byte[] fileData = File.ReadAllBytes(path);
                Width = BitConverter.ToInt32(fileData[1..5]);
                Height = BitConverter.ToInt32(fileData[5..9]);
                Image = fileData[10..];
            }
            else
            {
                Width = 600;
                Height = 400;
                Image = new byte[Width * Height * 4];
                MemoryOperations.Fill(Image, 255);
            }
        }

        internal byte[] Image;

        internal Paint Paint;

        internal string Path;

        internal int Width { get; private set; }
        internal int Height { get; private set; }

        internal int ScrollX { get; set; } = 0;
        internal int ScrollY { get; set; } = 0;

        internal bool IsInBounds(int x, int y)
        {
            // Kernel.PrintDebug("BOUNDS CHECK...");
            if (x >= Width || y >= Height) return false;
            if (x < 0 || y < 0) return false;

            //Kernel.PrintDebug("BOUNDS OK!...");
            return true;
        }

        internal void Save()
        {
            Kernel.PrintDebug("1");
            byte[] data = new byte[8 + (Width * Height * 4)];
            Kernel.PrintDebug("2");
            BitConverter.GetBytes(Width).CopyTo(data, 0);
            Kernel.PrintDebug("3");
            BitConverter.GetBytes(Height).CopyTo(data, 4);
            Kernel.PrintDebug("4");
            Image.CopyTo(data, 8);
            Kernel.PrintDebug("5 - " + data.Length.ToString());
            File.WriteAllBytes(Path, data);
            Kernel.PrintDebug("6");
        }

        internal void SetPixel(int x, int y, Color color)
        {
            if (!IsInBounds(x, y)) return;
            int i = ((y * Width) + x) * 4;
            Image[i] = color.B;
            Image[i + 1] = color.G;
            Image[i + 2] = color.R;
            Image[i + 3] = color.A;
        }

        internal Color GetPixel(int x, int y)
        {
            if (!IsInBounds(x, y)) return Color.Transparent;
            int i = ((y * Width) + x) * 4;
            return Color.FromArgb(Image[i + 3], Image[i + 2], Image[i + 1], Image[i]);
        }

        internal Color AlphaBlend(Color to, Color from, byte alpha)
        {
            byte R = (byte)((to.R * alpha + from.R * (255 - alpha)) >> 8);
            byte G = (byte)((to.G * alpha + from.G * (255 - alpha)) >> 8);
            byte B = (byte)((to.B * alpha + from.B * (255 - alpha)) >> 8);
            return Color.FromArgb(R, G, B);
        }

        internal void DrawLine(Color color, int dx, int dy, int x1, int y1)
        {
            if (dx == x1 && dy == y1)
            {
                SetPixel(dx, dy, color);
                return;
            }

            int i, sdx, sdy, dxabs, dyabs, x, y, px, py;

            dxabs = Math.Abs(dx);
            dyabs = Math.Abs(dy);
            sdx = Math.Sign(dx);
            sdy = Math.Sign(dy);
            x = dyabs >> 1;
            y = dxabs >> 1;
            px = x1;
            py = y1;

            if (dxabs >= dyabs)
            {
                for (i = 0; i < dxabs; i++)
                {
                    y += dyabs;
                    if (y >= dxabs)
                    {
                        y -= dxabs;
                        py += sdy;
                    }
                    px += sdx;
                    SetPixel(px, py, color);
                }
            }
            else
            {
                for (i = 0; i < dyabs; i++)
                {
                    x += dxabs;
                    if (x >= dyabs)
                    {
                        x -= dyabs;
                        px += sdx;
                    }
                    py += sdy;
                    SetPixel(px, py, color);
                }
            }
        }

        internal void FillCircle(int x0, int y0, int radius, Color color)
        {
            /*int diameter = radius / 2;
            int x1 = x - radius;
            int y1 = y - radius;
            for (int i = x1; i < x1 + radius; i++)
            {
                for (int j = y1; j < y1 + radius; j++)
                {
                    Kernel.PrintDebug("C1.");
                    //int middleDistance = (int)Math.Sqrt(Math.Pow(i - x, 2) + Math.Pow(j - y, 2));
                    Kernel.PrintDebug("C2");
                    //double brightness = (double)middleDistance / (double)radius;
                    Kernel.PrintDebug($"C3 - {i} {j}");
                    SetPixel(i, j, color);

                    Kernel.PrintDebug("C4");
                    if (middleDistance <= radius)
                    {
                        SetPixel(i, j, color);
                    }
                }
            }*/
            int x = radius;
            int y = 0;
            int xChange = 1 - (radius << 1);
            int yChange = 0;
            int radiusError = 0;
            while (x >= y)
            {
                for (int i = x0 - x; i <= x0 + x; i++)
                {
                    SetPixel(i, y0 + y, color);
                    SetPixel(i, y0 - y, color);
                }
                for (int i = x0 - y; i <= x0 + y; i++)
                {
                    SetPixel(i, y0 + x, color);
                    SetPixel(i, y0 - x, color);
                }
                y++;
                radiusError += yChange;
                yChange += 2;
                if (((radiusError << 1) + xChange) > 0)
                {
                    x--;
                    radiusError += xChange;
                    xChange += 2;
                }
            }
        }
    }
}
