using SphereOS.Core;
using SphereOS.Gui.UILib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SphereOS.Gui.Apps.Paint
{
    internal class ColourPicker : Window
    {
        private Paint paintInstance;

        private Table table;

        internal readonly List<Color> Colours = new List<Color>()
        {
            Color.Black,
            Color.White,
            Color.Red,
            Color.Blue,
            Color.Orange,
            Color.Green,
            Color.Pink,
            Color.Gray,
            Color.Purple,
            Color.DarkGoldenrod,
            Color.DarkGray,
            Color.DarkGreen,
            Color.DarkCyan,
            Color.Cyan,
            Color.BlueViolet,
            Color.AliceBlue,
            Color.Brown,
            Color.CornflowerBlue,
            Color.Azure,
            Color.Beige,
            Color.DarkBlue,
            Color.DarkSlateBlue,
            Color.SeaGreen
        };

        private void TableClicked(int x, int y)
        {
            // Clear 'Selected' text on previously selected colour.
            foreach (var cell in table.Cells)
            {
                cell.Text = string.Empty;
            }

            var selectedCell = table.Cells[table.SelectedCellIndex];
            Color color = (Color)selectedCell.Tag;

            paintInstance.SelectedColor = color;

            selectedCell.Text = "Selected";

            table.Render();
        }

        internal ColourPicker(Paint paint, int x, int y, int width, int height) : base(paint, x, y, width, height)
        {
            paintInstance = paint;

            Clear(Color.FromArgb(107, 107, 107));
            DrawString("Colours", Color.White, 8, 8);

            table = new Table(this, 0, 32, Width, Height - 32);
            table.AllowDeselection = false;
            table.CellHeight = 20;
            table.TextAlignment = Alignment.Middle;
            table.OnClick = TableClicked;

            foreach (Color colour in Colours)
            {
                TableCell cell = new(string.Empty, tag: colour);
                cell.BackgroundColourOverride = colour;
                cell.ForegroundColourOverride = colour.GetForegroundColour();
                if (colour == paint.SelectedColor)
                {
                    cell.Text = "Selected";
                }
                table.Cells.Add(cell);
            }

            table.Render();

            WM.AddWindow(this);
            WM.AddWindow(table);
        }
    }
}
