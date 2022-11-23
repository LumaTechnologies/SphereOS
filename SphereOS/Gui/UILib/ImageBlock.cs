using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using Cosmos.System.Graphics;

namespace SphereOS.Gui.UILib
{
    internal class ImageBlock : Control
    {
        public ImageBlock(Window parent, int x, int y, int width, int height) : base(parent, x, y, width, height)
        {
        }

        private Bitmap _image;
        internal Bitmap Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                Render();
            }
        }

        internal override void Render()
        {
            if (_image == null)
            {
                Clear(Color.Gray);
                WM.Update(this);
                return;
            }
            DrawImage(_image, 0, 0);

            WM.Update(this);
        }
    }
}
