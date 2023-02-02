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
    internal class ColourPicker : AppWindow
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
            Color.AliceBlue
        };

        private void TableClicked(int x, int y)
        {
            // Clear 'Selected' text on previously selected colour.
            table.Cells[table.SelectedCellIndex].Text = string.Empty;

            var cell = table.Cells[table.SelectedCellIndex];
            Color color = (Color)cell.Tag;

            paintInstance.SelectedColor = color;

            cell.Text = "Colour";
            table.Foreground = color.GetForegroundColour();

            table.Render();
        }

        internal ColourPicker(Paint paint, Window canvas) : base(paint, canvas.X + canvas.Width + 16, canvas.Y, 128, 128)
        {
            paintInstance = paint;

            Title = "Colours";
            table.AllowDeselection = false;
            table.CellHeight = 20;
            table.TextAlignment = Alignment.Middle;
            table.OnClick = TableClicked;

            foreach (Color colour in Colours)
            {
                TableCell cell = new(string.Empty, tag: colour);
                cell.BackgroundColourOverride = colour;
                if (colour == paint.SelectedColor)
                {
                    cell.Text = "Selected";
                    table.Foreground = colour.GetForegroundColour();
                }
            }

            table.Render();

            WM.AddWindow(table);
        }
    }
}
