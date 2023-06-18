using Cosmos.Core;
using Cosmos.System;
using Cosmos.System.Graphics;
using SphereOS.Core;
using System;
using System.Drawing;

namespace SphereOS.Gui
{
    internal class Window
    {
        internal Window(Process process, int x, int y, int width, int height)
        {
            WM = ProcessManager.GetProcess<WindowManager>();

            Process = process;
            X = x;
            Y = y;

            this.width = width;
            this.height = height;
            ResizeBuffer();
        }

        internal Window(Process process, Window parent, int x, int y, int width, int height)
        {
            WM = ProcessManager.GetProcess<WindowManager>();

            Process = process;
            _relativeTo = parent;
            X = x;
            Y = y;

            this.width = width;
            this.height = height;
            ResizeBuffer();
        }

        internal int[] Buffer { get; private set; }

        private Window _relativeTo = null;

        internal Process Process { get; private set; }

        protected WindowManager WM;

        private int x;
        private int y;

        private int width;
        private int height;

        private const int bytesPerPixel = 4;

        #region Events
        internal Action<int, int> OnDown;
        internal Action<int, int> OnClick;
        internal Action<int, int> OnDoubleClick;
        internal Action<KeyEvent> OnKeyPressed;
        internal Action OnFocused;
        internal Action OnUnfocused;
        internal Action UserResized;
        #endregion

        private void ResizeBuffer()
        {
            Buffer = new int[Width * Height];
            Cosmos.Core.MemoryOperations.Fill(Buffer, Color.White.ToArgb());
        }

        internal void Move(int x, int y, bool sendWMEvent = true)
        {
            if (x != X || y != Y)
            {
                this.x = x;
                this.y = y;
                if (sendWMEvent)
                {
                    ProcessManager.GetProcess<WindowManager>().RerenderAll();
                }
            }
        }

        internal void Resize(int width, int height, bool sendWMEvent = true)
        {
            if (width != Width || height != Height)
            {
                this.width = width;
                this.height = height;
                ResizeBuffer();
                if (sendWMEvent)
                {
                    ProcessManager.GetProcess<WindowManager>().RerenderAll();
                }
            }
        }

        internal void MoveAndResize(int x, int y, int width, int height, bool sendWMEvent = true)
        {
            Move(x, y, sendWMEvent: false);
            Resize(width, height, sendWMEvent: false);
            if (sendWMEvent)
            {
                ProcessManager.GetProcess<WindowManager>().RerenderAll();
            }
        }

        #region Graphics
        private Color AlphaBlend(Color to, Color from, byte alpha)
        {
            byte R = (byte)((to.R * alpha + from.R * (255 - alpha)) >> 8);
            byte G = (byte)((to.G * alpha + from.G * (255 - alpha)) >> 8);
            byte B = (byte)((to.B * alpha + from.B * (255 - alpha)) >> 8);
            return Color.FromArgb(R, G, B);
        }

        // https://github.com/CosmosOS/Cosmos/blob/master/source/Cosmos.System2/Graphics/Canvas.cs
        private void TrimLine(ref int x1, ref int y1, ref int x2, ref int y2)
        {
            if (x1 == x2)
            {
                x1 = Math.Min(width - 1, Math.Max(0, x1));
                x2 = x1;
                y1 = Math.Min(height - 1, Math.Max(0, y1));
                y2 = Math.Min(height - 1, Math.Max(0, y2));
                return;
            }

            float x1_out = x1, y1_out = y1;
            float x2_out = x2, y2_out = y2;

            float m = (y2_out - y1_out) / (x2_out - x1_out);
            float c = y1_out - m * x1_out;

            if (x1_out < 0)
            {
                x1_out = 0;
                y1_out = c;
            }
            else if (x1_out >= width)
            {
                x1_out = width - 1;
                y1_out = (width - 1) * m + c;
            }

            if (x2_out < 0)
            {
                x2_out = 0;
                y2_out = c;
            }
            else if (x2_out >= width)
            {
                x2_out = width - 1;
                y2_out = (width - 1) * m + c;
            }

            if (y1_out < 0)
            {
                x1_out = -c / m;
                y1_out = 0;
            }
            else if (y1_out >= height)
            {
                x1_out = (height - 1 - c) / m;
                y1_out = height - 1;
            }

            if (y2_out < 0)
            {
                x2_out = -c / m;
                y2_out = 0;
            }
            else if (y2_out >= width)
            {
                x2_out = (width - 1 - c) / m;
                y2_out = width - 1;
            }

            if (x1_out < 0 || x1_out >= width || y1_out < 0 || y1_out >= height)
            {
                x1_out = 0; x2_out = 0;
                y1_out = 0; y2_out = 0;
            }

            if (x2_out < 0 || x2_out >= width || y2_out < 0 || y2_out >= height)
            {
                x1_out = 0; x2_out = 0;
                y1_out = 0; y2_out = 0;
            }

            x1 = (int)x1_out; y1 = (int)y1_out;
            x2 = (int)x2_out; y2 = (int)y2_out;
        }

        // https://github.com/CosmosOS/Cosmos/blob/master/source/Cosmos.System2/Graphics/Canvas.cs
        internal void DrawHorizontalLine(int dx, int x1, int y1, Color color)
        {
            int i;

            if (dx > 0)
            {
                for (i = 0; i < dx; i++)
                {
                    DrawPoint(x1 + i, y1, color);
                }
            }
            else
            {
                for (i = 0; i > dx; i--)
                {
                    DrawPoint(x1 + i, y1, color);
                }
            }
        }

        // https://github.com/CosmosOS/Cosmos/blob/master/source/Cosmos.System2/Graphics/Canvas.cs
        internal void DrawVerticalLine(int dy, int x1, int y1, Color color)
        {
            int i;

            if (dy > 0)
            {
                for (i = 0; i < dy; i++)
                {
                    DrawPoint(x1, y1 + i, color);
                }
            }
            else
            {
                for (i = 0; i > dy; i--)
                {
                    DrawPoint(x1, y1 + i, color);
                }
            }
        }

        // https://github.com/CosmosOS/Cosmos/blob/master/source/Cosmos.System2/Graphics/Canvas.cs
        private void DrawDiagonalLine(int dx, int dy, int x1, int y1, Color color)
        {
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
                    DrawPoint(px, py, color);
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
                    DrawPoint(px, py, color);
                }
            }
        }

        public void Clear(Color color)
        {
            MemoryOperations.Fill(Buffer, color.ToArgb());
        }

        // To-do: Optimise.
        public void DrawFilledRectangle(int x, int y, int width, int height, Color color)
        {
            int argb = color.ToArgb();
            for (int i = Math.Max(0, x); i < Math.Min(Width, x + width); i++)
            {
                for (int j = Math.Max(0, y); j < Math.Min(Height, y + height); j++)
                {
                    Buffer[(j * Width) + i] = argb;
                }
            }
        }

        public void DrawRectangle(int x, int y, int width, int height, Color color)
        {
            DrawHorizontalLine(width, x, y, color);
            DrawHorizontalLine(width, x, y + height - 1, color);
            DrawVerticalLine(height - 1, x, y + 1, color);
            DrawVerticalLine(height - 1, x + width - 1, y + 1, color);
        }

        public void DrawPoint(int x, int y, Color color)
        {
            if (x < 0 || x >= width) return;
            if (y < 0 || y >= height) return;

            int index = x + (y * width);

            Buffer[index] = color.ToArgb();
        }

        public Color GetPixel(int x, int y)
        {
            if (x < 0 || x >= width) return Color.Transparent;
            if (y < 0 || y >= height) return Color.Transparent;

            int index = x + (y * width);

            return Color.FromArgb(Buffer[index]);
        }

        // https://github.com/CosmosOS/Cosmos/blob/master/source/Cosmos.System2/Graphics/Canvas.cs
        public void DrawCircle(int x, int y, int radius, Color color)
        {
            int i = radius;
            int j = 0;
            int e = 0;

            while (i >= j)
            {
                DrawPoint(x + i, y + j, color);
                DrawPoint(x + j, y + i, color);
                DrawPoint(x - j, y + i, color);
                DrawPoint(x - i, y + j, color);
                DrawPoint(x - i, y - j, color);
                DrawPoint(x - j, y - i, color);
                DrawPoint(x + j, y - i, color);
                DrawPoint(x + i, y - j, color);

                j++;
                if (e <= 0)
                {
                    e += 2 * j + 1;
                }
                if (e > 0)
                {
                    i--;
                    e -= 2 * i + 1;
                }
            }
        }

        // https://github.com/CosmosOS/Cosmos/blob/master/source/Cosmos.System2/Graphics/Canvas.cs
        public void DrawLine(int x1, int y1, int x2, int y2, Color color)
        {
            TrimLine(ref x1, ref y1, ref x2, ref y2);

            int dx, dy;

            dx = x2 - x1;
            dy = y2 - y1;

            if (dy == 0)
            {
                DrawHorizontalLine(dx, x1, y1, color);
                return;
            }

            if (dx == 0)
            {
                DrawVerticalLine(dy, x1, y1, color);
                return;
            }

            DrawDiagonalLine(dx, dy, x1, y1, color);
        }

        public void DrawString(string str, Color color, int x, int y)
        {
            //Asc16.DrawAsciiString(str, color, x, y, Buffer, width, height);
            SmoothMono.TextRenderer.DrawString(str, color, Buffer, width, height, x, y);
        }

        public unsafe void DrawImage(Bitmap bitmap, int x, int y)
        {
            /*if (bitmap.rawData.Length != bitmap.Width * bitmap.Height)
            {
                throw new Exception("Invalid bitmap.");
            }
            for (int i = 0; i < y; i++)
            {
                int destOffset = ((i + y) * width) + x;
                int srcOffset = (int)(i * bitmap.Width);
                fixed (int* destPtr = &Buffer[destOffset])
                fixed (int* sourcePtr = &bitmap.rawData[srcOffset])
                {
                    MemoryOperations.Copy(destPtr, sourcePtr, (int)bitmap.Width);
                }
            }*/
            for (int i = 0; i < bitmap.Height; i++)
            {
                for (int j = 0; j < bitmap.Width; j++)
                {
                    DrawPoint(x + j, y + i, Color.FromArgb(bitmap.RawData[(i * bitmap.Width) + j]));
                }
            }
        }

        public unsafe void DrawImageAlpha(Bitmap bitmap, int x, int y)
        {
            /*if (bitmap.rawData.Length != bitmap.Width * bitmap.Height)
            {
                throw new Exception("Invalid bitmap.");
            }
            for (int i = 0; i < y; i++)
            {
                int destOffset = ((i + y) * width) + x;
                int srcOffset = (int)(i * bitmap.Width);
                fixed (int* destPtr = &Buffer[destOffset])
                fixed (int* sourcePtr = &bitmap.rawData[srcOffset])
                {
                    MemoryOperations.Copy(destPtr, sourcePtr, (int)bitmap.Width);
                }
            }*/
            for (int i = 0; i < bitmap.Height; i++)
            {
                for (int j = 0; j < bitmap.Width; j++)
                {
                    Color from = GetPixel(x + j, y + i);
                    Color to = Color.FromArgb(bitmap.RawData[(i * bitmap.Width) + j]);
                    DrawPoint(x + j, y + i, AlphaBlend(to, from, to.A));
                }
            }
        }
        #endregion Graphics

        public int X
        {
            get
            {
                return x;
            }
            set
            {
                if (value != x)
                {
                    x = value;
                    ProcessManager.GetProcess<WindowManager>().RerenderAll();
                }
            }
        }

        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                if (value != y)
                {
                    y = value;
                    ProcessManager.GetProcess<WindowManager>().RerenderAll();
                }
            }
        }

        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                if (value != width)
                {
                    if (value < width)
                    {
                        ProcessManager.GetProcess<WindowManager>().RerenderAll();
                    }
                    width = value;
                    ResizeBuffer();
                }
            }
        }
        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                if (value != height)
                {
                    if (value < height)
                    {
                        ProcessManager.GetProcess<WindowManager>().RerenderAll();
                    }
                    height = value;
                    ResizeBuffer();
                }
            }
        }

        public int BytesPerPixel
        {
            get
            {
                return bytesPerPixel;
            }
        }

        public int ScreenX
        {
            get
            {
                if (_relativeTo != null)
                {
                    return x + _relativeTo.ScreenX;
                }
                else
                {
                    return x;
                }
            }
        }

        public int ScreenY
        {
            get
            {
                if (_relativeTo != null)
                {
                    return y + _relativeTo.ScreenY;
                }
                else
                {
                    return y;
                }
            }
        }

        public Window RelativeTo
        {
            get
            {
                return _relativeTo;
            }
            set
            {
                _relativeTo = value;
            }
        }
    }
}
