using Cosmos.System.Graphics;
using System;
using System.Drawing;

namespace SphereOS.Gui.UILib
{
    internal class Button : Control
    {
        public Button(Window parent, int x, int y, int width, int height) : base(parent, x, y, width, height)
        {
        }

        internal enum ButtonImageLocation
        {
            AboveText,
            Left
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

        private ButtonImageLocation _imageLocation = ButtonImageLocation.AboveText;
        internal ButtonImageLocation ImageLocation
        {
            get
            {
                return _imageLocation;
            }
            set
            {
                _imageLocation = value;
                Render();
            }
        }

        private Color _background = Color.FromArgb(48, 48, 48);
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

            if (_image != null)
            {
                switch (_imageLocation)
                {
                    case ButtonImageLocation.Left:
                        DrawImageAlpha(_image, (int)((Width / 2) - ((8 / 2) * Text.Length) - 8 - _image.Width), (int)((Height / 2) - (_image.Height / 2)));
                        DrawString(Text, Foreground, (Width / 2) - ((8 / 2) * Text.Length), (Height / 2) - (16 / 2));
                        break;
                    case ButtonImageLocation.AboveText:
                        DrawImageAlpha(_image, (int)((Width / 2) - (_image.Width / 2)), (int)((Height / 2) - (_image.Height / 2)));
                        DrawString(Text, Foreground, (Width / 2) - (4 * Text.Length), Height - 16);
                        break;
                    default:
                        throw new Exception("Unrecognised image location in button.");
                }
            }
            else
            {
                DrawString(Text, Foreground, (Width / 2) - (4 * Text.Length), (Height / 2) - 8);
            }

            DrawRectangle(0, 0, Width, Height, Border);

            WM.Update(this);
        }
    }
}
