using Cosmos.System.Graphics;
using System.Drawing;

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

        private bool _alpha = false;
        internal bool Alpha
        {
            get
            {
                return _alpha;
            }
            set
            {
                _alpha = value;
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

            if (_alpha)
            {
                DrawImageAlpha(_image, 0, 0);
            }
            else
            {
                DrawImage(_image, 0, 0);
            }

            WM.Update(this);
        }
    }
}
