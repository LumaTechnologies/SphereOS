using Cosmos.HAL.Drivers.PCI.Video;
using Cosmos.System;
using Cosmos.System.Graphics;
using System.Drawing;

namespace SphereOS.Paint
{
    internal class Text : Tool
    {
        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Paint.Images.text.bmp")]
        private static byte[] _icon;

        internal Text() : base("Text", new Bitmap(_icon))
        {

        }

        private bool writing = false;
        private string text = string.Empty;
        private int x = 0;
        private int y = 0;
        private MouseState lastMouseState = MouseState.None;
        private Document document;

        private void RenderTextToDocument(Color foreground, Document doc)
        {
            for (int i = 0; i < text.Length; i++)
            {
                byte[] bytes = FontData.Chars[text[i]];
                if (bytes != null)
                {
                    for (int j = 0; j < FontData.Width; j++)
                    {
                        for (int k = 0; k < FontData.Height; k++)
                        {
                            byte alpha = bytes[(k * FontData.Width) + j];
                            if (alpha == 0) continue;

                            int finalX = x + j + FontData.Width * i;
                            int finalY = y + k;

                            Color color = doc.AlphaBlend(foreground, doc.GetPixel(finalX, finalY), alpha);

                            doc.SetPixel(finalX, finalY, color);
                        }
                    }
                }
            }
        }

        internal override void Run(MouseState mouseState,
            int mouseX,
            int mouseY,
            Document doc,
            Paint paint)
        {
            document = doc;

            if (writing)
            {
                if (doc.IsInBounds(mouseX, mouseY) && mouseState == MouseState.Left && lastMouseState == MouseState.None)
                {
                    RenderTextToDocument(paint.SelectedColor, doc);
                    x = mouseX;
                    y = mouseY;
                    text = string.Empty;
                }
                if (KeyboardManager.TryReadKey(out var key))
                {
                    switch (key.Key)
                    {
                        case ConsoleKeyEx.Backspace:
                            if (text.Length > 0)
                            {
                                text = text.Substring(0, text.Length - 1);
                            }
                            break;
                        case ConsoleKeyEx.Escape or ConsoleKeyEx.Enter:
                            RenderTextToDocument(paint.SelectedColor, doc);
                            text = string.Empty;
                            writing = false;
                            break;
                        default:
                            text += key.KeyChar;
                            break;
                    }
                }
            }
            else
            {
                if (doc.IsInBounds(mouseX, mouseY) && mouseState == MouseState.Left && lastMouseState == MouseState.None)
                {
                    x = mouseX;
                    y = mouseY;
                    writing = true;
                }
            }

            lastMouseState = mouseState;
        }

        internal override void RenderOverlay(Paint paint, VMWareSVGAII driver)
        {
            if (writing)
            {
                TextRenderer.Render(text, x + document.ScrollX, y + document.ScrollY, driver, Color.Transparent, paint.SelectedColor);

                paint.DrawFilledRectangle((uint)Color.Gray.ToArgb(), x + document.ScrollX + FontData.Width * text.Length, y + document.ScrollY, 1, FontData.Height);
            }
        }

        internal override void Deselected()
        {
            writing = false;
            text = string.Empty;
        }
    }
}
