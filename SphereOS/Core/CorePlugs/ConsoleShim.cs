using SphereOS.VideoConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SphereOS
{
    public class Console
    {
        public static ConsoleColor ForegroundColor
        {
            get
            {
                return VideoConsole.VideoConsole.ForegroundColor;
            }
            set
            {
                VideoConsole.VideoConsole.ForegroundColor = value;
            }
        }

        public static ConsoleColor BackgroundColor
        {
            get
            {
                return VideoConsole.VideoConsole.BackgroundColor;
            }
            set
            {
                VideoConsole.VideoConsole.BackgroundColor = value;
            }
        }

        public static void Clear()
        {
            VideoConsole.VideoConsole.Clear();
        }

        public static void Write(string value)
        {
            VideoConsole.VideoConsole.Write(value);
        }

        public static void Write(object value)
        {
            VideoConsole.VideoConsole.Write(value.ToString());
        }

        public static void WriteLine(string value)
        {
            VideoConsole.VideoConsole.WriteLine(value);
        }

        public static void WriteLine(object value)
        {
            VideoConsole.VideoConsole.WriteLine(value.ToString());
        }

        public static void WriteLine()
        {
            VideoConsole.VideoConsole.WriteLine();
        }

        public static ConsoleKeyInfo ReadKey()
        {
            return VideoConsole.VideoConsole.ReadKey();
        }

        public static ConsoleKeyInfo ReadKey(bool intercept)
        {
            return VideoConsole.VideoConsole.ReadKey(intercept);
        }

        public static string ReadLine()
        {
            return VideoConsole.VideoConsole.ReadLine();
        }

        public static bool CursorVisible
        {
            get
            {
                return VideoConsole.VideoConsole.CursorVisible;
            }
            set
            {
                VideoConsole.VideoConsole.CursorVisible = value;
            }
        }

        public static (int Left, int Top) GetCursorPosition()
        {
            return (VideoConsole.VideoConsole.Column, VideoConsole.VideoConsole.Row);
        }

        public static void SetCursorPosition(int left, int top)
        {
            if (left < 0 || top < 0 || left >= VideoConsole.VideoConsole.Columns || top >= VideoConsole.VideoConsole.Rows)
                throw new Exception("Attempt to set cursor position outside the bounds of the console.");

            VideoConsole.VideoConsole.Column = left;
            VideoConsole.VideoConsole.Row = top;
        }

        public static int WindowWidth
        {
            get
            {
                return VideoConsole.VideoConsole.Columns;
            }
        }

        public static int WindowHeight
        {
            get
            {
                return VideoConsole.VideoConsole.Rows;
            }
        }
    }
}
