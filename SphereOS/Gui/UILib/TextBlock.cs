using System.Drawing;

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

        internal override void Render()
        {
            Clear(Background);
            DrawString(Text, Foreground, 0, 0);

            WM.Update(this);
        }
    }
}
