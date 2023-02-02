using Cosmos.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SphereOS.Gui.Apps.Paint.Tools
{
    internal class Pencil : Tool
    {
        public Pencil() : base("Pencil")
        {
        }

        private bool joinLine;
        private int joinX;
        private int joinY;

        internal override void Run(Paint paint, Window canvas, MouseState mouseState, int mouseX, int mouseY)
        {
            if (!paint.IsInBounds(mouseX, mouseY))
            {
                joinLine = false;
                return;
            }

            if (mouseState == MouseState.Left)
            {
                int x = mouseX;
                int y = mouseY;
                if (joinLine)
                {
                    canvas.DrawLine(joinX - x, joinY - y, x, y, paint.SelectedColor);
                }
                else
                {
                    canvas.DrawPoint(x, y, paint.SelectedColor);
                }
                joinLine = true;
                joinX = x;
                joinY = y;
            }
            else
            {
                joinLine = false;
            }
        }

        internal override void Deselected()
        {
            joinLine = false;
        }
    }
}
