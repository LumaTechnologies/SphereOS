using System;
using System.Collections.Generic;
using System.Drawing;
using Cosmos.System.Graphics;

namespace SphereOS.Gui.UILib
{
    internal class Table : Control
    {
        public Table(Window parent, int x, int y, int width, int height) : base(parent, x, y, width, height)
        {
            OnDown = TableDown;
        }

        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.ScrollbarUp.bmp")]
        private static byte[] scrollbarUpBytes;
        private static Bitmap scrollbarUpBitmap = new Bitmap(scrollbarUpBytes);

        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.ScrollbarDown.bmp")]
        private static byte[] scrollbarDownBytes;
        private static Bitmap scrollbarDownBitmap = new Bitmap(scrollbarDownBytes);

        internal List<TableCell> Cells { get; set; } = new List<TableCell>();

        internal Action<int> TableCellSelected { get; set; }

        internal bool AllowDeselection { get; set; } = true;

        private int scrollY = 0;

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

        private int _scrollbarThickness = 20;
        internal int ScrollbarThickness
        {
            get
            {
                return _scrollbarThickness;
            }
            set
            {
                _scrollbarThickness = value;
                Render();
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
            if ((CanScrollUp || CanScrollDown) && x >= (Width - _scrollbarThickness))
            {
                int allCellsHeight = Cells.Count * CellHeight;
                if (y < _scrollbarThickness && CanScrollUp)
                {
                    scrollY = Math.Max(0, scrollY - CellHeight);
                    Render();
                }
                if (y >= Height - _scrollbarThickness && CanScrollDown)
                {
                    scrollY = Math.Min(allCellsHeight - Height, scrollY + CellHeight);
                    Render();
                }
                return;
            }

            int scrollAdjustedY = y + scrollY;
            if (scrollAdjustedY < 0 || scrollAdjustedY > _cellHeight * Cells.Count)
            {
                if (AllowDeselection)
                {
                    SelectedCellIndex = -1;
                }
                return;
            }

            SelectedCellIndex = scrollAdjustedY / _cellHeight;
            TableCellSelected?.Invoke(_selectedCellIndex);
        }

        private bool CanScrollUp
        {
            get
            {
                return scrollY > 0;
            }
        }

        private bool CanScrollDown
        {
            get
            {
                int allCellsHeight = Cells.Count * CellHeight;
                return (scrollY < 0) || ((allCellsHeight > Height) && (scrollY < (allCellsHeight - Height)));
            }
        }

        private void RenderScrollbar()
        {
            if (CanScrollUp || CanScrollDown)
            {
                /* Background */
                DrawFilledRectangle(Width - _scrollbarThickness, 0, _scrollbarThickness, Height, _border);

                /* Track */
                int trackAvailableHeight = Height - (ScrollbarThickness * 2);
                double trackSize = (double)Height / (double)(Cells.Count * CellHeight);
                double trackProgress = (double)scrollY / (double)((Cells.Count * CellHeight) - Height);
                int trackY = (int)(_scrollbarThickness + (((double)trackAvailableHeight - ((double)trackAvailableHeight * trackSize)) * trackProgress));
                DrawFilledRectangle(Width - _scrollbarThickness, trackY, _scrollbarThickness, (int)(trackSize * trackAvailableHeight), _background);
                DrawRectangle(Width - _scrollbarThickness, 0, _scrollbarThickness, Height, _border);

                /* Up arrow */
                DrawFilledRectangle(Width - _scrollbarThickness, 0, _scrollbarThickness, _scrollbarThickness, CanScrollUp ? _background : _border);
                DrawRectangle(Width - _scrollbarThickness, 0, _scrollbarThickness, _scrollbarThickness, _border);
                DrawImageAlpha(scrollbarUpBitmap, (int)((Width - _scrollbarThickness) + ((_scrollbarThickness / 2) - (scrollbarUpBitmap.Width / 2))), (int)((_scrollbarThickness / 2) - (scrollbarUpBitmap.Height / 2)));

                /* Down arrow */
                DrawFilledRectangle(Width - _scrollbarThickness, Height - _scrollbarThickness, _scrollbarThickness, _scrollbarThickness, CanScrollDown ? _background : _border);
                DrawRectangle(Width - _scrollbarThickness, Height - _scrollbarThickness, _scrollbarThickness, _scrollbarThickness, _border);
                DrawImageAlpha(scrollbarDownBitmap, (int)((Width - _scrollbarThickness) + ((_scrollbarThickness / 2) - (scrollbarUpBitmap.Width / 2))), (int)((Height - _scrollbarThickness) + ((_scrollbarThickness / 2) - (scrollbarUpBitmap.Height / 2))));
            }
        }

        internal override void Render()
        {
            Clear(Background);

            for (int i = 0; i < Cells.Count; i++)
            {
                TableCell cell = Cells[i];
                bool selected = _selectedCellIndex == i;
                Rectangle cellRect = new Rectangle(0, (i * _cellHeight) - scrollY, Width, _cellHeight);

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

            RenderScrollbar();

            WM.Update(this);
        }
    }
}
