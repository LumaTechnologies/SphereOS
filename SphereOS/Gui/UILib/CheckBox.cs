using Cosmos.System.Graphics;
using System;
using System.Drawing;

namespace SphereOS.Gui.UILib
{
    internal class CheckBox : Control
    {
        public CheckBox(Window parent, int x, int y, int width, int height) : base(parent, x, y, width, height)
        {
            OnClick = CheckBoxClicked;
        }

        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.Check.bmp")]
        private static byte[] checkBytes;
        private static Bitmap checkBitmap = new Bitmap(checkBytes);

        internal Action CheckBoxChecked;
        internal Action CheckBoxUnchecked;
        internal Action<bool> CheckBoxChanged;

        private const int iconSize = 16;

        private bool _checked = false;
        internal bool Checked
        {
            get
            {
                return _checked;
            }
            set
            {
                _checked = value;
                if (_checked)
                {
                    CheckBoxChecked?.Invoke();
                }
                else
                {
                    CheckBoxUnchecked?.Invoke();
                }
                CheckBoxChanged?.Invoke(_checked);
                Render();
            }
        }

        private string _text = "CheckBox";
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

        private void CheckBoxClicked(int x, int y)
        {
            Checked = !Checked;
        }

        internal override void Render()
        {
            Clear(Background);

            int iconX = 0;
            int iconY = (Height / 2) - (iconSize / 2);

            int textX = iconSize + 8;
            int textY = (Height / 2) - (16 / 2);

            DrawFilledRectangle(iconX, iconY, iconSize, iconSize, Color.LightGray);
            if (_checked)
            {
                DrawImageAlpha(checkBitmap, iconX, iconY);
            }

            DrawString(Text, _foreground, textX, textY);

            WM.Update(this);
        }
    }
}
