using Cosmos.System;

namespace SphereOS.Gui.Apps.Paint.Tools
{
    internal class CircleBrush : Tool
    {
        public CircleBrush() : base("Circle brush")
        {
        }

        internal override void Run(Paint paint, Window canvas, MouseState mouseState, int mouseX, int mouseY)
        {
            if (mouseState == MouseState.Left)
            {
                canvas.DrawCircle(mouseX, mouseY, 5, paint.SelectedColor);
            }
        }
    }
}
