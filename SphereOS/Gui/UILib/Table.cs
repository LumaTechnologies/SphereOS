using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using Cosmos.System.Graphics;

namespace SphereOS.Gui.UILib
{
    internal class Table : Control
    {
        public Table(Window parent, int x, int y, int width, int height) : base(parent, x, y, width, height)
        {
            OnDown = TableDown;
        }

        internal List<string> Cells { get; set; } = new List<string>();

        internal Action<int> TableCellSelected { get; set; }

        private int _selectedCell = -1;
        internal int SelectedCell
        {
            get
            {
                return _selectedCell;
            }
            set
            {
                if (_selectedCell != value)
                {
                    _selectedCell = value;
                    TableCellSelected?.Invoke(_selectedCell);
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

        private void TableDown(int x, int y)
        {
            if (y > _cellHeight * Cells.Count) return;
            SelectedCell = y / _cellHeight;
        }

        internal override void Render()
        {
            Clear(Background);
            for (int i = 0; i < Cells.Count; i++)
            {
                string cell = Cells[i];
                bool selected = _selectedCell == i;
                Rectangle cellRect = new Rectangle(0, i * _cellHeight, Width, _cellHeight);

                if (selected)
                {
                    DrawFilledRectangle(cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height, _selectedBackground);
                }

                DrawRectangle(cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height, selected ? SelectedBorder : Border);

                int textX = cellRect.X; //cellRect.X + (cellRect.Width / 2) - (cell.Length * 8 / 2);
                int textY = cellRect.Y + (cellRect.Height / 2) - (16 / 2);

                DrawString(cell, selected ? SelectedForeground : Foreground, textX, textY);
            }
            WM.Update(this);
        }
    }
}
