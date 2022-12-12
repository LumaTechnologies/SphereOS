using SphereOS.Core;
using SphereOS.Gui.UILib;
using System;
using System.Drawing;

namespace SphereOS.Gui.Apps.Demos
{
    internal class Mandelbrot : Process
    {
        internal Mandelbrot() : base("Mandelbrot", ProcessType.Application) { }

        AppWindow window;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        private Color GetColor(double v)
        {
            int red = Math.Clamp((int)(255 * v), 0, 255);
            int green = 0;
            int blue = Math.Clamp((int)(255 * (1 - v)), 0, 255);

            return Color.FromArgb(red, green, blue);
        }

        private void RenderMandelbrot()
        {
            window.Clear(Color.Black);
            wm.Update(window);

            int width = window.Width;
            int height = window.Height;

            const int max = 20;
            const double bail = 2.0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double zx = 0;
                    double zy = 0;
                    double cx = (x - width / 2.0) / (width / 4.0);
                    double cy = (y - height / 2.0) / (height / 4.0);

                    int iteration = 0;

                    while (zx * zx + zy * zy < bail && iteration < max)
                    {
                        double zxNew = zx * zx - zy * zy + cx;
                        zy = 2 * zx * zy + cy;
                        zx = zxNew;
                        iteration++;
                    }

                    double smooth = iteration + 1 - Math.Log(Math.Log(Math.Sqrt(zx * zx + zy * zy)) / Math.Log(bail)) / Math.Log(2);
                    window.DrawPoint(x, y, GetColor(smooth / max));

                    if (x % 32 == 0)
                    {
                        ProcessManager.Yield();
                    }
                }

                if (y % 8 == 0)
                {
                    wm.Update(window);
                }
            }

            wm.Update(window);
        }

        internal override void Start()
        {
            base.Start();
            window = new AppWindow(this, 256, 256, 256, 256);
            wm.AddWindow(window);
            window.Title = "Mandelbrot";
            window.CanResize = true;
            window.Closing = TryStop;
            window.UserResized = RenderMandelbrot;

            RenderMandelbrot();
        }

        internal override void Run()
        {

        }
    }
}
