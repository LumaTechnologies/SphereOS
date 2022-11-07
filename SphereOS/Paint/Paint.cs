using Cosmos.Core;
using Cosmos.HAL.Drivers.PCI.Video;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.System.Graphics;
using Cosmos.System;
using System.IO;

namespace SphereOS.Paint
{
    internal class Paint
    {
        private VMWareSVGAII driver = new VMWareSVGAII();
        private Mode mode = new Mode(1024, 768, ColorDepth.ColorDepth32);
        private Document doc;
        private Button exitButton = new Button(Images.Exit);
        //private Button saveButton = new Button(Images.Save);
        private string path;
        private bool exited = false;
        private const int toolbarHeight = 32;
        private string statusText = string.Empty;

        internal Paint(string path)
        {
            this.path = path;
        }

        internal Paint()
        {
        }

        private List<Tool> tools = new List<Tool>()
        {
            new Brush(),
            new Pencil(),
            new Text()
        };

        private List<DefaultColor> defaultColors = new List<DefaultColor>()
        {
            new DefaultColor(Color.Black, "Black"),
            new DefaultColor(Color.White, "White"),
            new DefaultColor(Color.Gray, "Grey"),
            new DefaultColor(Color.Red, "Red"),
            new DefaultColor(Color.Orange, "Orange"),
            new DefaultColor(Color.Lime, "Green"),
            new DefaultColor(Color.Blue, "Blue"),
            new DefaultColor(Color.Cyan, "Cyan"),
            new DefaultColor(Color.Magenta, "Magenta")
        };

        private Tool selectedTool;
        internal Color SelectedColor = Color.Red;

        private uint lastMouseX = 0;
        private uint lastMouseY = 0;
        private MouseState lastMouseState = MouseState.None;

        private byte[] image;

        private static class Theme
        {
            internal static uint PageBackground = (uint)Color.FromArgb(33, 33, 37).ToArgb();
            internal static uint Toolbar = (uint)Color.FromArgb(48, 48, 52).ToArgb();
            internal static uint Status = (uint)Color.FromArgb(48, 48, 52).ToArgb();
        }
        
        private static class Images
        {
            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Paint.Images.cursor.bmp")]
            private static byte[] _cursor;
            internal static Bitmap Cursor = new Bitmap(_cursor);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Paint.Images.selected.bmp")]
            private static byte[] _selected;
            internal static Bitmap Selected = new Bitmap(_selected);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Paint.Images.exit.bmp")]
            private static byte[] _exit;
            internal static Bitmap Exit = new Bitmap(_exit);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Paint.Images.save.bmp")]
            private static byte[] _save;
            internal static Bitmap Save = new Bitmap(_save);
        }

        internal void SetStatusToFileName()
        {
            if (doc.Path == null)
            {
                statusText = "New File";
            }
            else
            {
                if (File.Exists(doc.Path))
                {
                    statusText = Path.GetFileName(doc.Path);
                }
                else
                {
                    statusText = $"{Path.GetFileName(doc.Path)} (New File)";
                }
            }
        }

        internal int GetPointOffset(int x, int y)
        {
            int bytesPerPixel = (int)mode.ColorDepth / 8;
            int stride = (int)mode.ColorDepth / 8;
            int pitch = mode.Columns * bytesPerPixel;

            return (x * stride) + (y * pitch);
        }

        internal void DrawImage(Image aImage, int aX, int aY)
        {
            var xWidth = (int)aImage.Width;
            var xHeight = (int)aImage.Height;

            for (int i = 0; i < xHeight; i++)
            {
                driver.VideoMemory.Copy(GetPointOffset(aX, aY + i) + (int)driver.FrameSize, aImage.rawData, (i * xWidth), xWidth);
            }
        }

        internal void DrawImage(byte[] aImage, int aX, int aY, int aWidth, int aHeight)
        {
            for (int i = 0; i < aHeight; i++)
            {
                driver.VideoMemory.Copy(GetPointOffset(aX, aY + i) + (int)driver.FrameSize, aImage, (i * aWidth), aWidth);
            }
        }

        private void DrawDocument(Document document)
        {
            for (int i = 0; i < document.Height; i++)
            {
                driver.VideoMemory.Copy(GetPointOffset(document.ScrollX, document.ScrollY + i) + (int)driver.FrameSize, document.Image, i * document.Width * 4, document.Width * 4);
            }
        }

        internal Color AlphaBlend(Color to, Color from, byte alpha)
        {
            byte R = (byte)((to.R * alpha + from.R * (255 - alpha)) >> 8);
            byte G = (byte)((to.G * alpha + from.G * (255 - alpha)) >> 8);
            byte B = (byte)((to.B * alpha + from.B * (255 - alpha)) >> 8);
            return Color.FromArgb(R, G, B);
        }

        internal void DrawImageAlpha(Image image, int x, int y)
        {
            var xWidth = (int)image.Width;
            var xHeight = (int)image.Height;

            for (int _x = 0; _x < xWidth; _x++)
            {
                for (int _y = 0; _y < xHeight; _y++)
                {
                    Color from = Color.FromArgb((int)driver.GetPixel((uint)(x + _x), (uint)(y + _y)));
                    Color to = Color.FromArgb(image.rawData[_x + _y * image.Width]);
                    Color color = AlphaBlend(to, from, to.A);
                    driver.SetPixel((uint)(x + _x), (uint)(y + _y), (uint)color.ToArgb());
                }
            }
        }

        internal void DrawFilledRectangle(uint color, int x, int y, int width, int height)
        {
            for (int i = y; i < y + height; i++)
            {
                driver.VideoMemory.Fill((uint)(GetPointOffset(x, i) + driver.FrameSize), (uint)width, color);
            }
        }

        private void DrawCursor()
        {
            DrawImageAlpha(Images.Cursor, (int)MouseManager.X, (int)MouseManager.Y);
        }

        private void DrawToolbar()
        {
            DrawFilledRectangle(Theme.Toolbar, 0, 0, mode.Columns, toolbarHeight);
            for (int i = 0; i < tools.Count; i++)
            {
                var tool = tools[i];
                if (selectedTool == tool)
                {
                    DrawImage(Images.Selected, tool.ButtonX, tool.ButtonY);
                }
                DrawImageAlpha(tool.Icon, tool.ButtonX, tool.ButtonY);
            }
            for (int i = 0; i < defaultColors.Count; i++)
            {
                var color = defaultColors[i];
                if (SelectedColor == color.Color)
                {
                    DrawFilledRectangle((uint)Color.White.ToArgb(), color.ButtonX + 1, color.ButtonY + 1, color.ButtonWidth - 2, color.ButtonHeight - 2);
                }
                DrawFilledRectangle((uint)color.Color.ToArgb(), color.ButtonX + 2, color.ButtonY + 2, color.ButtonWidth - 4, color.ButtonHeight - 4);
            }
            DrawImage(Images.Exit, exitButton.ButtonX, exitButton.ButtonY);
            //DrawImage(Images.Save, saveButton.ButtonX, saveButton.ButtonY);
        }

        private void LayoutToolbar()
        {
            int x = 0;
            for (int i = 0; i < tools.Count; i++)
            {
                var tool = tools[i];
                tool.ButtonX = x;
                tool.ButtonY = 0;
                tool.ButtonWidth = toolbarHeight;
                tool.ButtonHeight = toolbarHeight;
                x += toolbarHeight;
                //Kernel.PrintDebug(x.ToString());
            }
            for (int i = 0; i < defaultColors.Count; i++)
            {
                var color = defaultColors[i];
                color.ButtonX = x;
                color.ButtonY = 0;
                color.ButtonWidth = toolbarHeight;
                color.ButtonHeight = toolbarHeight;
                x += toolbarHeight;
            }
            exitButton.ButtonWidth = (int)Images.Exit.Width;
            exitButton.ButtonHeight = (int)Images.Exit.Height;
            exitButton.ButtonX = mode.Columns - exitButton.ButtonWidth;
            exitButton.ButtonY = 0;

            /*saveButton.ButtonWidth = (int)Images.Save.Width;
            saveButton.ButtonHeight = (int)Images.Save.Height;
            saveButton.ButtonX = exitButton.ButtonX - saveButton.ButtonWidth;
            saveButton.ButtonY = 0;*/
        }

        private void ExitClicked()
        {
            exited = true;
        }

        private void SaveClicked()
        {
            doc.Save();
            statusText = $"Saved to {path}";
        }

        private bool UIInput()
        {
            if (MouseManager.Y < toolbarHeight)
            {
                if (MouseManager.MouseState == MouseState.Left && MouseManager.LastMouseState == MouseState.None)
                {
                    foreach (var tool in tools)
                    {
                        if (MouseManager.X >= tool.ButtonX
                            && MouseManager.X >= tool.ButtonY
                            && MouseManager.X < tool.ButtonX + tool.ButtonWidth
                            && MouseManager.Y < tool.ButtonY + tool.ButtonHeight)
                        {
                            selectedTool.Deselected();
                            selectedTool = tool;
                            tool.Selected();
                            statusText = tool.Name;
                        }
                    }
                    foreach (var color in defaultColors)
                    {
                        if (MouseManager.X >= color.ButtonX
                            && MouseManager.X >= color.ButtonY
                            && MouseManager.X < color.ButtonX + color.ButtonWidth
                            && MouseManager.Y < color.ButtonY + color.ButtonHeight)
                        {
                            SelectedColor = color.Color;
                            statusText = $"Colour: {color.Name}";
                        }
                    }
                    if (MouseManager.X >= exitButton.ButtonX
                            && MouseManager.X >= exitButton.ButtonY
                            && MouseManager.X < exitButton.ButtonX + exitButton.ButtonWidth
                            && MouseManager.Y < exitButton.ButtonY + exitButton.ButtonHeight)
                    {
                        ExitClicked();
                    }
                    /*if (MouseManager.X >= saveButton.ButtonX
                            && MouseManager.X >= saveButton.ButtonY
                            && MouseManager.X < saveButton.ButtonX + saveButton.ButtonWidth
                            && MouseManager.Y < saveButton.ButtonY + saveButton.ButtonHeight)
                    {
                        SaveClicked();
                    }*/
                }
                return true;
            }
            return false;
        }

        private void DrawStatus()
        {
            DrawFilledRectangle(Theme.Status, 0, mode.Rows - TextRenderer.FontHeight, mode.Columns, TextRenderer.FontHeight);
            TextRenderer.Render(statusText, 0, mode.Rows - TextRenderer.FontHeight, driver, Color.FromArgb((int)Theme.Status), Color.White);
        }

        internal void Run()
        {
            Util.PrintLine(ConsoleColor.Cyan, "[Paint] Starting SphereOS Paint...");

            doc = new Document(path);

            doc.ScrollX = (mode.Columns / 2) - (doc.Width / 2);
            doc.ScrollY = (mode.Rows / 2) - (doc.Height / 2);
            selectedTool = tools[0];
            LayoutToolbar();
            SetStatusToFileName();

            MouseManager.ScreenWidth = (uint)mode.Columns;
            MouseManager.ScreenHeight = (uint)mode.Rows;
            MouseManager.X = MouseManager.ScreenWidth / 2;
            MouseManager.Y = MouseManager.ScreenHeight / 2;
            lastMouseX = MouseManager.X;
            lastMouseY = MouseManager.Y;
            lastMouseState = MouseManager.MouseState;

            TextRenderer.Initialize();
            driver.SetMode((uint)mode.Columns, (uint)mode.Rows, (uint)mode.ColorDepth);

            while (!exited)
            {
                if (!UIInput())
                {
                    selectedTool.Run(
                        MouseManager.MouseState,
                        (int)MouseManager.X - doc.ScrollX,
                        (int)MouseManager.Y - doc.ScrollY,
                        doc,
                        this
                    );
                    lastMouseX = MouseManager.X;
                    lastMouseY = MouseManager.Y;
                    lastMouseState = MouseManager.MouseState;
                }

                driver.Clear(Theme.PageBackground);

                DrawDocument(doc);
                selectedTool.RenderOverlay(this, driver);
                DrawStatus();
                DrawToolbar();
                DrawCursor();

                driver.DoubleBufferUpdate();

                Cosmos.Core.Memory.Heap.Collect();
            }

            driver.Disable();
        }
    }
}
