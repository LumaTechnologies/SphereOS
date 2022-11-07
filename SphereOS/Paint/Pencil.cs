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
    internal class Pencil : Tool
    {
        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Paint.Images.pencil.bmp")]
        private static byte[] _icon;

        internal Pencil() : base("Pencil", new Bitmap(_icon))
        {

        }

        private bool joinLine = false;
        private int joinX = 0;
        private int joinY = 0;

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
                int x = mouseX;
                int y = mouseY;
                if (joinLine)
                {
                    doc.DrawLine(paint.SelectedColor, joinX - x, joinY - y, x, y);
                }
                else
                {
                    doc.SetPixel(x, y, paint.SelectedColor);
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
