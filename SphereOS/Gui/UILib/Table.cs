using System;
using System.Collections.Generic;
using System.Drawing;

namespace SphereOS.Gui.UILib
{
    internal class Table : Control
    {
        public Table(Window parent, int x, int y, int width, int height) : base(parent, x, y, width, height)
        {
            OnDown = TableDown;
        }

        internal List<TableCell> Cells { get; set; } = new List<TableCell>();

        internal Action<int> TableCellSelected { get; set; }

        internal bool AllowDeselection { get; set; } = true;

        private int _selectedCellIndex = -1;
        internal int SelectedCellIndex
        {
            get
            {
                return _selectedCellIndex;
            }
            set
            {
                if (_selectedCellIndex != value)
                {
                    _selectedCellIndex = value;
                    Render();
                }
            }
        }

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

        private Color _border = Color.Gray;
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

        private Color _selectedBackground = Color.FromArgb(221, 246, 255);
        internal Color SelectedBackground
        {
            get
            {
                return _selectedBackground;
            }
            set
            {
                _selectedBackground = value;
                Render();
            }
        }

        private Color _selectedForeground = Color.Black;
        internal Color SelectedForeground
        {
            get
            {
                return _selectedForeground;
            }
            set
            {
                _selectedForeground = value;
                Render();
            }
        }

        private Color _selectedBorder = Color.FromArgb(126, 205, 234);
        internal Color SelectedBorder
        {
            get
            {
                return _selectedBorder;
            }
            set
            {
                _selectedBorder = value;
                Render();
            }
        }

        private int _cellHeight = 20;
        internal int CellHeight
        {
            get
            {
                return _cellHeight;
            }
            set
            {
                _cellHeight = value;
                Render();
            }
        }

        private void TableDown(int x, int y)
        {
            if (y > _cellHeight * Cells.Count)
            {
                if (AllowDeselection)
                {
                    SelectedCellIndex = -1;
                }
                return;
            }
            SelectedCellIndex = y / _cellHeight;
            TableCellSelected?.Invoke(_selectedCellIndex);
        }

        internal override void Render()
        {
            Clear(Background);

            for (int i = 0; i < Cells.Count; i++)
            {
                TableCell cell = Cells[i];
                bool selected = _selectedCellIndex == i;
                Rectangle cellRect = new Rectangle(0, i * _cellHeight, Width, _cellHeight);

                if (selected)
                {
                    DrawFilledRectangle(cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height, _selectedBackground);
                }

                int textX = cellRect.X; //cellRect.X + (cellRect.Width / 2) - (cell.Length * 8 / 2);
                int textY = cellRect.Y + (cellRect.Height / 2) - (16 / 2);

                if (cell.Image != null)
                {
                    textX += (int)cell.Image.Width;
                    DrawImageAlpha(cell.Image, cellRect.X, (int)(cellRect.Y + (cellRect.Height / 2) - (cell.Image.Height / 2)));
                }
                DrawString(cell.Text, selected ? SelectedForeground : Foreground, textX, textY);

                DrawRectangle(cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height, selected ? SelectedBorder : Border);
            }

            WM.Update(this);
        }
    }
}
