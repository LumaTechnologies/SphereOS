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

        AppWindow canvas;

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
            canvas = new AppWindow(this, 256, 256, 320, 256);
            wm.AddWindow(canvas);
            canvas.Title = "Paint";
            canvas.Closing = TryStop;
            canvas.OnDown = CanvasDown;

            canvas.Clear(Color.White);

            toolBox = new ToolBox(this, canvas);

            colourPicker = new ColourPicker(this, canvas);

            wm.Update(canvas);
        }

        internal override void Run()
        {
            if (down)
            {
                if (MouseManager.MouseState == MouseState.None)
                {
                    down = false;
                    return;
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
