/*using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SphereOS.VideoConsole
{
    public static class VideoConsole
    {
        private struct Cell
        {
            public Cell(char? @char, ConsoleColor background, ConsoleColor foreground)
            {
                Char = @char;
                Background = background;
                Foreground = foreground;
            }

            public char? Char;
            public ConsoleColor Background;
            public ConsoleColor Foreground;
        }

        internal static Canvas Canvas;

        internal static bool Initialised = false;

        private static Pen[] pens = new Pen[]
        {
            new Pen(Color.Black),
            new Pen(Color.DarkBlue),
            new Pen(Color.DarkGreen),
            new Pen(Color.DarkCyan),
            new Pen(Color.DarkRed),
            new Pen(Color.DarkMagenta),
            new Pen(Color.Olive),
            new Pen(Color.Gray),
            new Pen(Color.DarkGray),
            new Pen(Color.Blue),
            new Pen(Color.Green),
            new Pen(Color.Cyan),
            new Pen(Color.Red),
            new Pen(Color.Yellow),
            new Pen(Color.Magenta),
            new Pen(Color.White)
        };

        private static Color[] colors = new Color[]
        {
            Color.Black,
            Color.DarkBlue,
            Color.DarkGreen,
            Color.DarkCyan,
            Color.DarkRed,
            Color.DarkMagenta,
            Color.Olive,
            Color.Gray,
            Color.DarkGray,
            Color.Blue,
            Color.Green,
            Color.Cyan,
            Color.Red,
            Color.Yellow,
            Color.Magenta,
            Color.White
        };

        private const int caretThickness = 2;
        private const bool verticalCaret = true;

        internal static int Column = 0;
        internal static int Row = 0;

        private static bool waitingToBreak = false;

        // todo: update when changed
        internal static bool CursorVisible = true;

        private static int? oldCaretColumn;
        private static int? oldCaretRow;

        public static int Columns;
        public static int Rows;

        public static ConsoleColor BackgroundColor = ConsoleColor.Black;
        public static ConsoleColor ForegroundColor = ConsoleColor.White;

        private static Cell[][] buffer;

        private static Pen pen = new Pen(Color.Black);
        private static Pen caretPen = new Pen(Color.Gray);

        internal static void Initialise()
        {
            Canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(1024, 768, ColorDepth.ColorDepth32));

            Columns = Canvas.Mode.Columns / FontData.Width;
            Rows = Canvas.Mode.Rows / FontData.Height;

            buffer = new Cell[Rows][];
            for (int r = 0; r < Rows; r++)
            {
                buffer[r] = new Cell[Columns];
                for (int c = 0; c < Columns; c++)
                {
                    buffer[r][c].Foreground = ForegroundColor;
                }
            }

            Initialised = true;
        }

        private static void DrawAll()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    DrawBuffer(r, c);
                }
            }
        }

        private static void ShiftBufferUp()
        {
            Row--;

            Array.Copy(buffer, 1, buffer, 0, buffer.Length - 1);

            Cell[] newRow = new Cell[Columns];
            buffer[Rows - 1] = newRow;
            for (int c = 0; c < Columns; c++)
            {
                newRow[c].Foreground = ForegroundColor;
            }

            DrawAll();
        }

        private static void DrawCaret()
        {
            if (!CursorVisible) return;

            if (oldCaretColumn != null && oldCaretRow != null)
            {
                DrawBuffer((int)oldCaretRow, (int)oldCaretColumn);
            }

            int caretColumn = Column;
            int caretRow = Row;
            if (caretColumn >= Columns)
            {
                caretColumn = 0;
                caretRow++;
                if (caretRow >= Rows)
                {
                    ShiftBufferUp();
                    caretRow--;
                }
            }

            Cell cell = buffer[caretRow][caretColumn];
            if (verticalCaret)
            {
                Canvas.DrawFilledRectangle(caretPen, (caretColumn * FontData.Width) + caretThickness, caretRow * FontData.Height, caretThickness, FontData.Height);
            }
            else
            {
                Canvas.DrawFilledRectangle(caretPen, caretColumn * FontData.Width, (caretRow * FontData.Height) + (FontData.Height - caretThickness), FontData.Width, caretThickness);
            }

            oldCaretColumn = caretColumn;
            oldCaretRow = caretRow;
        }

        private static void DrawBuffer(int r, int c)
        {
            int x = c * FontData.Width;
            int y = r * FontData.Height;
            Cell cell = buffer[r][c];

            Canvas.DrawFilledRectangle(pens[(int)cell.Background], x, y, FontData.Width, FontData.Height);
            if (cell.Char != null)
            {
                int chari = (int)cell.Char;
                if (chari >= FontData.Chars.Length) return;
                byte[] bytes = FontData.Chars[chari];
                if (bytes == null) return;
                for (int i = 0; i < FontData.Width; i++)
                {
                    for (int j = 0; j < FontData.Height; j++)
                    {
                        byte alpha = bytes[(j * FontData.Width) + i];
                        if (alpha == 0) continue;

                        Color color = Color.FromArgb(alpha, colors[(int)ForegroundColor]);
                        pen.Color = color;

                        Canvas.DrawPoint(pen, x + i, y + j);
                    }
                }
            }
        }

        public static void Clear()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    Cell cell = buffer[r][c];
                    cell.Background = BackgroundColor;
                    cell.Foreground = ForegroundColor;
                    cell.Char = null;
                }
            }

            DrawAll();
        }

        public static void Write(string value)
        {
            if (!Initialised) return;

            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                switch (c)
                {
                    case (char)127:
                        buffer[Row][Column].Char = null;
                        DrawBuffer(Row, Column);
                        break;
                    case (char)12 or '\b':
                        if (!(Row == 0 && Column == 0))
                        {
                            Column--;
                            if (Column < 0)
                            {
                                Row--;
                                Column = Columns - 1;
                            }
                        }
                        DrawCaret();
                        Canvas.Display();
                        break;
                    case '\r':
                        Column = 0;
                        break;
                    case '\n':
                        if (waitingToBreak)
                        {
                            Column = 0;
                            Row++;
                        }
                        else
                        {
                            waitingToBreak = true;
                        }
                        break;
                    default:
                        if (waitingToBreak)
                        {
                            waitingToBreak = false;
                            Column = 0;
                            Row++;
                        }
                        if (Column >= Columns)
                        {
                            Column = 0;
                            Row++;
                        }
                        if (Row >= Rows)
                        {
                            ShiftBufferUp();
                        }
                        buffer[Row][Column] = new Cell(c, BackgroundColor, ForegroundColor);
                        DrawBuffer(Row, Column);
                        Column++;
                        break;
                }
            }
            DrawCaret();
            Canvas.Display();
        }

        private static string KeyToString(ConsoleKeyInfo key)
        {
            return key.Key switch
            {
                ConsoleKey.Enter => "\n",
                ConsoleKey.Backspace => ((char)127).ToString(),
                _ => key.KeyChar.ToString(),
            };
        }

        public static ConsoleKeyInfo ReadKey()
        {
            return ReadKey(false);
        }

        public static ConsoleKeyInfo ReadKey(bool intercept)
        {
            Cosmos.System.KeyEvent keyEvent = KeyboardManager.ReadKey();
            ConsoleKeyInfo key = new ConsoleKeyInfo(keyEvent.KeyChar,
                                                        keyEvent.Key.ToConsoleKey(),
                                                        keyEvent.Modifiers.HasFlag(ConsoleModifiers.Shift), keyEvent
                                                        .Modifiers.HasFlag(ConsoleModifiers.Alt), keyEvent
                                                        .Modifiers.HasFlag(ConsoleModifiers.Control));
            if (!intercept) Write(KeyToString(key));
            return key;
        }

        public static string ReadLine()
        {
            string input = "";
            while (true)
            {
                ConsoleKeyInfo key = ReadKey();
                if (key.Key == ConsoleKey.Enter)
                {
                    Write("\n");
                    return input;
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace)
                    {
                        input = input.Substring(0, input.Length - 1);
                    }
                    else
                    {
                        input += KeyToString(key);
                    }
                }
            }
        }

        public static void WriteLine(string value)
        {
            Write(value + "\n");
        }

        public static void WriteLine()
        {
            Write("\n");
        }
    }
}
*/