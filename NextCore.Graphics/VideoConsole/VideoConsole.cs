using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System.Drawing;

namespace NextCore.Graphics.VideoConsole
{
    public static class VideoConsole
    {
        private struct Cell
        {
            public Cell(char? @char, Color background, Color foreground)
            {
                Char = @char;
                Background = background;
                Foreground = foreground;
            }

            public char? Char;
            public Color Background;
            public Color Foreground;
        }

        private static VBECanvas canvas;

        public static bool Initialised { get; private set; } = false;

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
        private const bool verticalCaret = false;

        public static int Column { get; set; } = 0;
        public static int Row { get; set; } = 0;

        // todo: update when changed
        public static bool CursorVisible = true;

        private static int? oldCaretColumn;
        private static int? oldCaretRow;

        public static int Columns { get; private set; }
        public static int Rows { get; private set; }

        public static ConsoleColor BackgroundColor = ConsoleColor.Black;
        public static ConsoleColor ForegroundColor = ConsoleColor.White;

        private static Cell[][] buffer;

        private static Color caretColor = Color.White;

        private static Font font = PCScreenFont.Default; //Fonts.VideoConsoleFont;

        private static ushort screenWidth = 1280;
        private static ushort screenHeight = 720;

        public static void Initialise()
        {
            canvas = new VBECanvas(new Mode(screenWidth, screenHeight, ColorDepth.ColorDepth32));

            Columns = screenWidth / font.Width;
            Rows = screenHeight / font.Height;

            buffer = new Cell[Rows][];
            for (int r = 0; r < Rows; r++)
            {
                buffer[r] = new Cell[Columns];
                for (int c = 0; c < Columns; c++)
                {
                    buffer[r][c].Foreground = colors[(int)ForegroundColor];
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
                    DrawCell(r, c);
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
                newRow[c].Background = colors[(int)BackgroundColor];
            }

            //DrawAll();
            canvas.ScrollUp(font.Height);
        }

        private static void DrawCaret()
        {
            if (!CursorVisible) return;

            // If there's already a caret on screen,
            // redraw its cell.
            if (oldCaretColumn != null && oldCaretRow != null)
            {
                DrawCell((int)oldCaretRow, (int)oldCaretColumn);
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
                canvas.DrawFilledRectangle(caretColor, caretColumn * font.Width, caretRow * font.Height, caretThickness, font.Height);
            }
            else
            {
                canvas.DrawFilledRectangle(caretColor, caretColumn * font.Width, (caretRow * font.Height) + (font.Height - caretThickness), font.Width, caretThickness);
            }

            oldCaretColumn = caretColumn;
            oldCaretRow = caretRow;
        }

        private static void DrawCell(int r, int c)
        {
            int x = c * font.Width;
            int y = r * font.Height;
            Cell cell = buffer[r][c];

            canvas.DrawFilledRectangle(cell.Background, x, y, font.Width, font.Height);

            if (cell.Char != null)
            {
                canvas.DrawChar((char)cell.Char, font, cell.Foreground, x, y);
            }
        }

        public static void Clear()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    Cell cell = buffer[r][c];
                    cell.Background = colors[(int)BackgroundColor];
                    cell.Foreground = colors[(int)ForegroundColor];
                    cell.Char = null;
                }
            }

            DrawAll();
        }

        private static void Write(string value, bool display = true)
        {
            if (!Initialised) return;

            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                switch (c)
                {
                    case (char)127:
                        buffer[Row][Column].Char = null;
                        DrawCell(Row, Column);
                        break;
                    case (char)12 or '\b':
                        if (!(Row == 0 && Column == 0))
                        {
                            buffer[Row][Column].Char = null;
                            DrawCell(Row, Column);
                            Column--;
                            if (Column < 0)
                            {
                                Row--;
                                Column = Columns - 1;
                            }
                        }
                        DrawCaret();
                        canvas.Display();
                        break;
                    case '\r':
                        Column = 0;
                        break;
                    case '\n':
                        Column = 0;
                        Row++;
                        break;
                    default:
                        if (Column >= Columns)
                        {
                            Column = 0;
                            Row++;
                        }
                        buffer[Row][Column] = new Cell(c, colors[(int)BackgroundColor], colors[(int)ForegroundColor]);
                        DrawCell(Row, Column);
                        Column++;
                        break;
                }
                if (Row >= Rows)
                {
                    ShiftBufferUp();
                }
            }
            DrawCaret();
            if (display)
            {
                canvas.Display();
            }
        }

        public static void Write(string value)
        {
            Write(value, display: true);
        }

        private static string KeyToString(ConsoleKeyInfo key)
        {
            return key.Key switch
            {
                ConsoleKey.Enter => "\n",
                ConsoleKey.Backspace => "\b",
                _ => key.KeyChar.ToString(),
            };
        }

        public static ConsoleKeyInfo ReadKey()
        {
            return ReadKey(false);
        }

        public static string ReadLine()
        {
            string input = string.Empty;
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
                        if (input.Length > 0)
                        {
                            Write("\b", display: false);
                            input = input.Substring(0, input.Length - 1);
                        }
                    }
                    else
                    {
                        input += KeyToString(key);
                    }
                }
            }
        }

        public static ConsoleKeyInfo ReadKey(bool intercept)
        {
            var key = KeyboardManager.ReadKey();

            bool shift = (key.Modifiers & ConsoleModifiers.Shift) == ConsoleModifiers.Shift;
            bool alt = (key.Modifiers & ConsoleModifiers.Alt) == ConsoleModifiers.Alt;
            bool control = (key.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control;

            var info = new ConsoleKeyInfo(key.KeyChar, key.Key.ToConsoleKey(), shift, alt, control);
            if (!intercept &&
                key.Key != ConsoleKeyEx.Backspace &&
                key.Key != ConsoleKeyEx.Enter)
            {
                Write(KeyToString(info));
            }

            return info;
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
