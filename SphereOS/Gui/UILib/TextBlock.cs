using System.Drawing;
using System;

namespace SphereOS.Gui.UILib
{
    internal class TextBlock : Control
    {
        public TextBlock(Window parent, int x, int y, int width, int height) : base(parent, x, y, width, height)
        {
        }

        private string _text = "TextBlock";
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

        private Color _background = Color.White;
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

        private Color _foreground = Color.Black;
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

        private Alignment _horizontalAlignment = Alignment.Start;
        internal Alignment HorizontalAlignment
        {
            get
            {
                return _horizontalAlignment;
            }
            set
            {
                _horizontalAlignment = value;
                Render();
            }
        }

        private Alignment _verticalAlignment = Alignment.Start;
        internal Alignment VerticalAlignment
        {
            get
            {
                return _verticalAlignment;
            }
            set
            {
                _verticalAlignment = value;
                Render();
            }
        }

        internal override void Render()
        {
            Clear(Background);

            int textX;
            int textY;

            switch (HorizontalAlignment)
            {
                case Alignment.Start:
                    textX = 0;
                    break;
                case Alignment.Middle:
                    textX = (Width / 2) - (8 * Text.Length / 2);
                    break;
                case Alignment.End:
                    textX = Width - (8 * Text.Length);
                    break;
                default:
                    throw new Exception("Invalid horizontal alignment.");
            }

            switch (VerticalAlignment)
            {
                case Alignment.Start:
                    textY = 0;
                    break;
                case Alignment.Middle:
                    textY = (Height / 2) - (16 / 2);
                    break;
                case Alignment.End:
                    textY = Height - 16;
                    break;
                default:
                    throw new Exception("Invalid vertical alignment.");
            }

            DrawString(Text, Foreground, textX, textY);

            WM.Update(this);
        }
    }
}
