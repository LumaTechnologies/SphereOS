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
    internal class ToolBox : Window
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
            Tool tool = table.Cells[table.SelectedCellIndex].Tag as Tool;

            if (tool != SelectedTool)
            {
                SelectedTool.Deselected();
                SelectedTool = tool;
            }
        }

        internal ToolBox(Paint paint, int x, int y, int width, int height) : base(paint, x, y, width, height)
        {
            paintInstance = paint;


            Clear(Color.FromArgb(107, 107, 107));
            DrawString("Toolbox", Color.White, 8, 8);

            table = new Table(this, 0, 32, Width, Height - 32);
            table.AllowDeselection = false;
            table.CellHeight = 24;
            table.TextAlignment = Alignment.Middle;
            table.OnClick = TableClicked;

            foreach (Tool tool in Tools)
            {
                table.Cells.Add(new TableCell(tool.Name, tool));
            }

            SelectedTool = Tools[0];
            table.SelectedCellIndex = 0;

            table.Render();

            WM.AddWindow(this);
            WM.AddWindow(table);
        }
    }
}
