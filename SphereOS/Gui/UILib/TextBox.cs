using Cosmos.System;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SphereOS.Gui.UILib
{
    internal class TextBox : Control
    {
        public TextBox(Window parent, int x, int y, int width, int height) : base(parent, x, y, width, height)
        {
            OnDown = TextBoxDown;
            OnKeyPressed = TextBoxKeyPressed;
            OnUnfocused = TextBoxUnfocused;
        }

        internal Action Submitted;
        internal Action Changed;

        internal string Text
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < lines.Count; i++)
                {
                    builder.Append(lines[i]);
                    if (i != lines.Count - 1)
                    {
                        builder.AppendLine();
                    }
                }
                return builder.ToString();
            }
            set
            {
                lines = value.Split('\n').ToList();

                caretLine = -1;
                caretCol = 0;

                MarkAllLines();
                Render();
            }
        }

        private string _placeholderText = string.Empty;
        internal string PlaceholderText
        {
            get
            {
                return _placeholderText;
            }
            set
            {
                _placeholderText = value;
                Render();
            }
        }

        internal bool ReadOnly { get; set; } = false;

        internal bool MultiLine { get; set; } = false;

        internal bool Shield { get; set; } = false;

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

        private Color _placeholderForeground = Color.Gray;
        internal Color PlaceholderForeground
        {
            get
            {
                return _placeholderForeground;
            }
            set
            {
                _placeholderForeground = value;
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

        private void TextBoxUnfocused()
        {
            MarkLine(caretLine);

            caretLine = -1;
            caretCol = 0;

            Render();
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
                    if (!MultiLine)
                    {
                        Submitted?.Invoke();

                        caretLine = -1;
                        caretCol = 0;

                        MarkAllLines();
                        break;
                    }

                    lines.Insert(caretLine + 1, lines[caretLine].Substring(caretCol));
                    lines[caretLine] = lines[caretLine].Substring(0, caretCol);

                    caretLine++;
                    caretCol = 0;

                    MarkLine(caretLine - 1);
                    MarkLine(caretLine);

                    Changed?.Invoke();
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

                        Changed?.Invoke();
                    }
                    else
                    {
                        lines[caretLine] = lines[caretLine].Remove(caretCol - 1, 1);
                        caretCol--;
                        MarkLine(caretLine);

                        Changed?.Invoke();
                    }
                    break;
                default:
                    lines[caretLine] = lines[caretLine].Insert(caretCol, key.KeyChar.ToString());
                    caretCol++;
                    MarkLine(caretLine);

                    Changed?.Invoke();
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

        internal void MarkAllLines()
        {
            markedLinesBegin = 0;
            markedLinesEnd = lines.Count - 1;
        }

        private List<string> lines = new List<string>() { string.Empty };

        private int markedLinesBegin = 0;
        private int markedLinesEnd = 0;

        private const int fontWidth = 8;
        private const int fontHeight = 16;

        private int caretLine = -1;
        private int caretCol = 0;

        private int scrollX = 0;
        private int scrollY = 0;

        internal override void Render()
        {
            if (Text == string.Empty)
            {
                Clear(_background);

                DrawRectangle(0, 0, Width, Height, Color.Gray);
                DrawString(PlaceholderText, PlaceholderForeground, 0, 0);

                if (caretLine == 0)
                {
                    DrawVerticalLine(fontHeight, 1, 0, Foreground);
                }

                WM.Update(this);

                return;
            }

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
                    DrawString(Shield ? new string('*', lines[i].Length) : lines[i], Foreground, -scrollX, lineY);

                    if (caretLine == i)
                    {
                        DrawVerticalLine(fontHeight, ((caretCol * fontWidth) - scrollX) + 1, (caretLine * fontHeight) - scrollY, Foreground);
                    }
                }
            }

            markedLinesBegin = -1;
            markedLinesEnd = -1;

            DrawRectangle(0, 0, Width, Height, Color.Gray);

            WM.Update(this);
        }
    }
}
