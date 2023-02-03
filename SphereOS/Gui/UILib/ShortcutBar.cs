using Cosmos.System;
using Cosmos.System.Graphics;
using SphereOS.Commands.GeneralTopic;
using SphereOS.Gui.SmoothMono;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace SphereOS.Gui.UILib
{
    internal class ShortcutBar : Control
    {
        public ShortcutBar(Window parent, int x, int y, int width, int height) : base(parent, x, y, width, height)
        {
            OnClick = ShortcutBarClick;
        }

        internal List<ShortcutBarCell> Cells { get; set; } = new List<ShortcutBarCell>();

        private Color _background = Color.LightGray;
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

        private int _cellPadding = 10;
        internal int CellPadding
        {
            get
            {
                return _cellPadding;
            }
            set
            {
                _cellPadding = value;
                Render();
            }
        }

        private void ShortcutBarClick(int x, int y)
        {
            int cellEndX = 0;
            foreach (var cell in Cells)
            {
                cellEndX += (_cellPadding * 2) + (FontData.Width * cell.Text.Length);
                if (x < cellEndX)
                {
                    cell.OnClick?.Invoke();
                    return;
                }
            }
        }

        internal override void Render()
        {
            Clear(Background);

            int cellX = 0;
            for (int i = 0; i < Cells.Count; i++)
            {
                ShortcutBarCell cell = Cells[i];
                Rectangle cellRect = new Rectangle(cellX, 0, Width, Height);

                int textX = cellRect.X + _cellPadding;
                int textY = cellRect.Y + (cellRect.Height / 2) - (FontData.Height / 2);

                DrawString(cell.Text, Foreground, textX, textY);

                cellX += (_cellPadding * 2) + (FontData.Width * cell.Text.Length);
            }

            WM.Update(this);
        }
    }
}
