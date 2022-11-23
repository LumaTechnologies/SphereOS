using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using Cosmos.System.Graphics;

namespace SphereOS.Gui.UILib
{
    internal class Button : Control
    {
        public Button(Window parent, int x, int y, int width, int height) : base(parent, x, y, width, height)
        {
        }

        private string _text = "Button";
        internal string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                Render();
            }
        }

        private Color _background = Color.FromArgb(32,32,32);
        internal Color Background
        {
            get
            {
                return _background;
            }
            set
            {
                _background = value;
                Render();
            }
        }

        private Color _foreground = Color.White;
        internal Color Foreground
        {
            get
            {
                return _foreground;
            }
            set
            {
                _foreground = value;
                Render();
            }
        }

        private Color _border = Color.Black;
        internal Color Border
        {
            get
            {
                return _border;
            }
            set
            {
                _border = value;
                Render();
            }
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
            Clear(Background);
            DrawRectangle(0, 0, Width, Height, Border);
            if (_image != null)
            {
                DrawImageAlpha(_image, (int)((Width / 2) - (_image.Width / 2)), (int)((Height / 2) - (_image.Height / 2)));
                DrawString(Text, Foreground, (Width / 2) - (4 * Text.Length), Height - 16);
            }
            else
            {
                DrawString(Text, Foreground, (Width / 2) - (4 * Text.Length), (Height / 2) - 8);
            }
            WM.Update(this);
        }
    }
}
