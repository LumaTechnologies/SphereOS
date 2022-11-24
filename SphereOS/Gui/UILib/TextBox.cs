using Cosmos.System;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SphereOS.Gui.UILib
{
    internal class TextBox : Control
    {
        public TextBox(Window parent, int x, int y, int width, int height) : base(parent, x, y, width, height)
        {
            OnDown = TextBoxDown;
            KeyPressed = TextBoxKeyPressed;
        }

        internal string Text
        {
            get
            {
                return string.Join('\n', lines);
            }
            set
            {
                lines = value.Split('\n').ToList();
                MarkAllLines();
                Render();
            }
        }

        internal bool ReadOnly { get; set; } = false;

        private Color _background = Color.White;
        internal Color Background
        {
            get
            {
                return _background;
            }
            set
            {
                _background = value;
                Clear(_background);
                MarkAllLines();
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
                MarkAllLines();
                Render();
            }
        }

        private void MoveCaret(int line, int col)
        {
            if (caretLine == line && caretCol == col) return;
            MarkLine(caretLine);
            caretLine = Math.Clamp(line, 0, lines.Count - 1);
            caretCol = Math.Clamp(col, 0, lines[caretLine].Length);
            MarkLine(caretLine);
            Render();
        }

        private void TextBoxDown(int x, int y)
        {
            MoveCaret((y + scrollY) / fontHeight, ((x + scrollX) + (fontWidth / 2)) / fontWidth);
        }

        private void AutoScroll()
        {
            if (caretLine == -1) return;

            if (scrollY + Height < (caretLine + 1) * fontHeight)
            {
                // Scroll up.
                scrollY = ((caretLine + 1) * fontHeight) - Height;
                MarkAllLines();
            }
            if (caretLine * fontHeight < scrollY)
            {
                // Scroll down.
                scrollY = caretLine * fontHeight;
                MarkAllLines();
            }

            if (scrollX + Width < (caretCol + 1) * fontWidth)
            {
                // Scroll right.
                scrollX = ((caretCol + 1) * fontWidth) - Width;
                MarkAllLines();
            }
            if (caretCol * fontWidth < scrollX)
            {
                // Scroll left.
                scrollX = caretCol * fontWidth;
                MarkAllLines();
            }
        }

        private void TextBoxKeyPressed(KeyEvent key)
        {
            if (caretLine == -1 || ReadOnly) return;
            switch (key.Key)
            {
                case ConsoleKeyEx.LeftArrow:
                    if (caretCol == 0)
                    {
                        if (caretLine == 0) return;
                        caretLine--;
                        caretCol = lines[caretLine].Length;
                        MarkLine(caretLine);
                        MarkLine(caretLine + 1);
                    }
                    else
                    {
                        caretCol--;
                        MarkLine(caretLine);
                    }
                    break;
                case ConsoleKeyEx.RightArrow:
                    if (caretCol == lines[caretLine].Length)
                    {
                        if (caretLine == lines.Count - 1) return;
                        caretLine++;

                        caretCol = 0;
                        MarkLine(caretLine - 1);
                        MarkLine(caretLine);
                    }
                    else
                    {
                        caretCol++;
                        MarkLine(caretLine);
                    }
                    break;
                case ConsoleKeyEx.UpArrow:
                    if (caretLine == 0) return;

                    caretLine--;
                    caretCol = Math.Min(lines[caretLine].Length, caretCol);

                    MarkLine(caretLine);
                    MarkLine(caretLine + 1);
                    break;
                case ConsoleKeyEx.DownArrow:
                    if (caretLine == lines.Count - 1) return;

                    caretLine++;
                    caretCol = Math.Min(lines[caretLine].Length, caretCol);

                    MarkLine(caretLine - 1);
                    MarkLine(caretLine);
                    break;
                case ConsoleKeyEx.Enter:
                    lines.Insert(caretLine + 1, lines[caretLine].Substring(caretCol));
                    lines[caretLine] = lines[caretLine].Substring(0, caretCol);

                    caretLine++;
                    caretCol = 0;

                    MarkLine(caretLine - 1);
                    MarkLine(caretLine);
                    break;
                case ConsoleKeyEx.Backspace:
                    if (caretCol == 0)
                    {
                        if (caretLine == 0) return;

                        caretLine--;
                        caretCol = lines[caretLine].Length;

                        lines[caretLine] += lines[caretLine + 1];
                        lines.RemoveAt(caretLine + 1);

                        MarkLine(caretLine);
                        MarkLine(caretLine + 1);
                    }
                    else
                    {
                        lines[caretLine] = lines[caretLine].Remove(caretCol - 1, 1);
                        caretCol--;
                        MarkLine(caretLine);
                    }
                    break;
                default:
                    lines[caretLine] = lines[caretLine].Insert(caretCol, key.KeyChar.ToString());
                    caretCol++;
                    MarkLine(caretLine);
                    break;
            }
            Render();
        }

        private void MarkLine(int lineNum)
        {
            if (markedLinesBegin == -1)
            {
                markedLinesBegin = lineNum;
            }
            else
            {
                markedLinesBegin = Math.Min(markedLinesBegin, lineNum);
            }
            if (markedLinesEnd == -1)
            {
                markedLinesEnd = lineNum;
            }
            else
            {
                markedLinesEnd = Math.Max(markedLinesEnd, lineNum);
            }
        }

        private void MarkAllLines()
        {
            markedLinesBegin = 0;
            markedLinesEnd = lines.Count - 1;
        }

        private List<string> lines = new List<string>() { string.Empty };

        private int markedLinesBegin = -1;
        private int markedLinesEnd = -1;

        private const int fontWidth = 8;
        private const int fontHeight = 16;

        private int caretLine = -1;
        private int caretCol = 0;

        private int scrollX = 0;
        private int scrollY = 0;

        internal override void Render()
        {
            if (markedLinesBegin == -1 || markedLinesEnd == -1) return;

            AutoScroll();

            for (int i = markedLinesBegin; i <= markedLinesEnd; i++)
            {
                int lineY = (i * fontHeight) - scrollY;

                if (lineY < 0) continue;
                if (lineY > Height) break;

                DrawFilledRectangle(0, lineY, Width, fontHeight, Background);

                if (i < lines.Count)
                {
                    DrawString(lines[i], Foreground, -scrollX, lineY);

                    if (caretLine == i)
                    {
                        DrawVerticalLine(fontHeight, (caretCol * fontWidth) - scrollX, (caretLine * fontHeight) - scrollY, Foreground);
                    }
                }
            }

            markedLinesBegin = -1;
            markedLinesEnd = -1;

            WM.Update(this);
        }
    }
}
