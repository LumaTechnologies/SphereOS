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
                if (joinLine)
                {
                    canvas.DrawLine(joinX, joinY, mouseX, mouseY, paint.SelectedColor);
                }
                else
                {
                    canvas.DrawPoint(mouseX, mouseY, paint.SelectedColor);
                }
                joinLine = true;
                joinX = mouseX;
                joinY = mouseY;
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
