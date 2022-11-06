using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.Core.IOGroup;
using Cosmos.System;
using Cosmos.System.Graphics;

namespace SphereOS.Paint
{
    internal class Brush : Tool
    {
        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Paint.Images.brush.bmp")]
        private static byte[] _icon;

        internal Brush() : base("Brush", new Bitmap(_icon))
        {

        }

        private bool joinLine = false;
        private int joinX = 0;
        private int joinY = 0;

        internal int Radius = 4;

        internal override void Run(MouseState mouseState,
            int mouseX,
            int mouseY,
            Document doc,
            Paint paint)
        {
            if (!doc.IsInBounds(mouseX, mouseY))
            {
                joinLine = false;
                return;
            }

            if (mouseState == MouseState.Left)
            {
                //Kernel.PrintDebug(lastMouseState == MouseState.Left ? "Left" : "Misc");
                int x1 = mouseX;
                int y1 = mouseY;
                if (joinLine)
                {
                    int x2 = joinX;
                    int y2 = joinY;
                    int diameter = 5;
                    int length = (int)Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
                    int steps = Math.Max(length / (diameter / 2), 1);
                    for (int i = 0; i < steps; i++)
                    {
                        int x = x1 + (x2 - x1) * i / steps;
                        int y = y1 + (y2 - y1) * i / steps;
                        //Kernel.PrintDebug("A");
                        doc.FillCircle(x, y, Radius, paint.SelectedColor);
                        //Kernel.PrintDebug("A2");
                    }
                }
                else
                {
                    //Kernel.PrintDebug("B");
                    doc.FillCircle(x1, y1, Radius, paint.SelectedColor);
                    //Kernel.PrintDebug("B2");
                }
                joinLine = true;
                joinX = x1;
                joinY = y1;
            }
            else
            {
                joinLine = false;
            }
        }
    }
}
