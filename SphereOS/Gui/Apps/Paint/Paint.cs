using Cosmos.System;
using SphereOS.Core;
using SphereOS.Gui.UILib;
using System.Collections.Generic;
using System.Drawing;

namespace SphereOS.Gui.Apps.Paint
{
    internal class Paint : Process
    {
        internal Paint() : base("Paint", ProcessType.Application)
        {
        }

        AppWindow window;

        Window canvas;

        ToolBox toolBox;

        ColourPicker colourPicker;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        private bool down = false;

        internal Color SelectedColor { get; set; } = Color.Black;

        internal bool IsInBounds(int x, int y)
        {
            if (x >= canvas.Width || y >= canvas.Height) return false;
            if (x < 0 || y < 0) return false;

            return true;
        }

        private void CanvasDown(int x, int y)
        {
            down = true;
        }

        internal override void Start()
        {
            base.Start();
            window = new AppWindow(this, 256, 256, 768, 448);
            window.Title = "Paint";
            window.Closing = TryStop;
            window.OnDown = CanvasDown;
            window.Clear(Color.FromArgb(73, 73, 73));
            wm.AddWindow(window);

            int canvasWidth = 384;
            int canvasHeight = 256;
            canvas = new Window(this, (window.Width / 2) - (canvasWidth / 2), (window.Height / 2) - (canvasHeight / 2), canvasWidth, canvasHeight);
            canvas.RelativeTo = window;
            canvas.Clear(Color.White);
            wm.AddWindow(canvas);

            toolBox = new ToolBox(this, 0, 0, 128, window.Height);
            toolBox.RelativeTo = window;
            colourPicker = new ColourPicker(this, window.Width - 128, 0, 128, window.Height);
            colourPicker.RelativeTo = window;

            wm.Update(window);
        }
        
        internal override void Run()
        {
            if (down)
            {
                if (MouseManager.MouseState == MouseState.None)
                {
                    down = false;
                }

                toolBox.SelectedTool.Run(
                   this,
                   canvas,
                   MouseManager.MouseState,
                   (int)(MouseManager.X - canvas.ScreenX),
                   (int)(MouseManager.Y - canvas.ScreenY)
                );

                wm.Update(canvas);
            }
        }
    }
}
