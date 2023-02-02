using SphereOS.Core;
using SphereOS.Gui.UILib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SphereOS.Gui.Apps.Paint
{
    internal class ToolBox : AppWindow
    {
        private Paint paintInstance;

        private Table table;

        internal Tool SelectedTool;

        internal readonly List<Tool> Tools = new List<Tool>()
        {
            new Tools.Pencil()
        };

        private void TableClicked(int x, int y)
        {
            SelectedTool = table.Cells[table.SelectedCellIndex].Tag as Tool;
        }

        internal ToolBox(Paint paint, Window canvas) : base(paint, canvas.X - 128, canvas.Y, 96, canvas.Height)
        {
            paintInstance = paint;

            Title = "Toolbox";

            table = new Table(this, 0, 0, this.Width, this.Height);
            table.AllowDeselection = false;
            table.CellHeight = 24;
            table.TextAlignment = Alignment.Middle;
            table.OnClick = TableClicked;

            foreach (Tool tool in Tools)
            {
                table.Cells.Add(new TableCell(tool.Name, tool));
            }

            table.Render();
            WM.AddWindow(table);
        }
    }
}
