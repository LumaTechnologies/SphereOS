using IL2CPU.API.Attribs;
using NextCore.Graphics.VideoConsole;

namespace NextCore.Plugs.System
{
    /// <summary>
    /// Provides a plug that connects the console to the <see cref="NextCore.Graphics.VideoConsole.VideoConsole"/>.
    /// </summary>
    [Plug(Target = typeof(global::System.Console))]
    public static class ConsoleImpl
    {
        public static ConsoleColor get_BackgroundColor()
        {
            return VideoConsole.BackgroundColor;
        }

        public static void set_BackgroundColor(ConsoleColor value)
        {
            VideoConsole.BackgroundColor = value;
        }

        public static int get_BufferHeight()
        {
            return VideoConsole.Rows;
        }

        public static void set_BufferHeight(int aHeight)
        {
            throw new NotImplementedException("Not implemented: set_BufferHeight");
        }

        public static int get_BufferWidth()
        {
            return VideoConsole.Columns;
        }

        public static void set_BufferWidth(int aWidth)
        {
            throw new NotImplementedException("Not implemented: set_BufferWidth");
        }

        public static bool get_CapsLock()
        {
            return Cosmos.System.Global.CapsLock;
        }

        public static int get_CursorLeft()
        {
            return VideoConsole.Column;
        }

        public static void set_CursorLeft(int x)
        {
            VideoConsole.Column = x;
        }

        public static int get_CursorSize()
        {
            throw new NotImplementedException("Not implemented: get_CursorSize");
        }

        public static void set_CursorSize(int aSize)
        {
            throw new NotImplementedException("Not implemented: set_CursorSize");
        }

        public static int get_CursorTop()
        {
            return VideoConsole.Row;
        }

        public static void set_CursorTop(int y)
        {
            VideoConsole.Row = y;
        }

        public static bool get_CursorVisible()
        {
            return VideoConsole.CursorVisible;
        }

        public static void set_CursorVisible(bool value)
        {
            VideoConsole.CursorVisible = value;
        }

        public static ConsoleColor get_ForegroundColor()
        {
            return VideoConsole.ForegroundColor;
        }

        public static void set_ForegroundColor(ConsoleColor value)
        {
            VideoConsole.ForegroundColor = value;
        }

        public static bool get_KeyAvailable()
        {
            return Cosmos.System.KeyboardManager.KeyAvailable;
        }

        public static int get_LargestWindowHeight()
        {
            throw new NotImplementedException("Not implemented: get_LargestWindowHeight");
        }

        public static int get_LargestWindowWidth()
        {
            throw new NotImplementedException("Not implemented: get_LargestWindowWidth");
        }

        public static bool get_NumberLock()
        {
            return Cosmos.System.Global.NumLock;
        }

        public static string get_Title()
        {
            throw new NotImplementedException("Not implemented: get_Title");
        }

        public static void set_Title(string value)
        {
            throw new NotImplementedException("Not implemented: set_Title");
        }

        public static bool get_TreatControlCAsInput()
        {
            throw new NotImplementedException("Not implemented: get_TreatControlCAsInput");
        }

        public static void set_TreatControlCAsInput(bool value)
        {
            throw new NotImplementedException("Not implemented: set_TreatControlCAsInput");
        }

        public static int get_WindowHeight()
        {
            return VideoConsole.Rows;
        }

        public static void set_WindowHeight(int value)
        {
            throw new NotImplementedException("Not implemented: set_WindowHeight");
        }

        public static int get_WindowLeft()
        {
            throw new NotImplementedException("Not implemented: get_WindowLeft");
        }

        public static void set_WindowLeft(int value)
        {
            throw new NotImplementedException("Not implemented: set_WindowLeft");
        }

        public static int get_WindowTop()
        {
            throw new NotImplementedException("Not implemented: get_WindowTop");
        }

        public static void set_WindowTop(int value)
        {
            throw new NotImplementedException("Not implemented: set_WindowTop");
        }

        public static int get_WindowWidth()
        {
            return VideoConsole.Columns;
        }

        public static void set_WindowWidth(int value)
        {
            throw new NotImplementedException("Not implemented: set_WindowWidth");
        }

        public static void Clear()
        {
            VideoConsole.Clear();
        }

        public static void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight,
            int targetLeft, int targetTop, char sourceChar, ConsoleColor sourceForeColor, ConsoleColor sourceBackColor)
        {
            throw new NotImplementedException("Not implemented: MoveBufferArea");
        }

        public static int Read()
        {
            Cosmos.System.KeyEvent xResult;

            if (Cosmos.System.KeyboardManager.TryReadKey(out xResult))
            {
                return xResult.KeyChar;
            }
            else
            {
                return -1;
            }
        }

        public static ConsoleKeyInfo ReadKey()
        {
            return ReadKey(false);
        }

        public static ConsoleKeyInfo ReadKey(bool intercept)
        {
            return VideoConsole.ReadKey(intercept);
        }

        public static string ReadLine()
        {
            return VideoConsole.ReadLine();
        }

        public static void ResetColor()
        {
            set_BackgroundColor(ConsoleColor.Black);
            set_ForegroundColor(ConsoleColor.White);
        }

        public static void SetBufferSize(int width, int height)
        {
            throw new NotImplementedException("Not implemented: SetBufferSize");
        }

        public static void SetCursorPosition(int left, int top)
        {
            set_CursorLeft(left);
            set_CursorTop(top);
        }

        public static (int Left, int Top) GetCursorPosition()
        {
            int Left = get_CursorLeft();
            int Top = get_CursorTop();

            return (Left, Top);
        }

        public static void SetWindowPosition(int left, int top)
        {
            throw new NotImplementedException("Not implemented: SetWindowPosition");
        }

        public static void SetWindowSize(int width, int height)
        {
            throw new NotImplementedException("Not implemented: SetWindowSize");
        }

        public static void Write(bool aBool)
        {
            Write(aBool.ToString());
        }

        public static void Write(char aChar) => Write(aChar.ToString());

        public static void Write(char[] aBuffer) => Write(aBuffer, 0, aBuffer.Length);

        public static void Write(decimal aDecimal) => Write(aDecimal.ToString());

        public static void Write(double aDouble) => Write(aDouble.ToString());

        public static void Write(float aFloat) => Write(aFloat.ToString());

        public static void Write(int aInt) => Write(aInt.ToString());

        public static void Write(long aLong) => Write(aLong.ToString());

        public static void Write(object value) => Write(value ?? String.Empty);

        public static void Write(string aText)
        {
            VideoConsole.Write(aText);
        }

        public static void Write(uint aInt) => Write(aInt.ToString());

        public static void Write(ulong aLong) => Write(aLong.ToString());

        public static void Write(string format, object arg0) => Write(String.Format(format, arg0));

        public static void Write(string format, object arg0, object arg1) => Write(String.Format(format, arg0, arg1));

        public static void Write(string format, object arg0, object arg1, object arg2) => Write(String.Format(format, arg0, arg1, arg2));

        public static void Write(string format, params object[] arg) => Write(String.Format(format, arg));

        public static void Write(char[] aBuffer, int aIndex, int aCount)
        {
            if (aBuffer == null)
            {
                throw new ArgumentNullException("aBuffer");
            }
            if (aIndex < 0)
            {
                throw new ArgumentOutOfRangeException("aIndex");
            }
            if (aCount < 0)
            {
                throw new ArgumentOutOfRangeException("aCount");
            }
            if (aBuffer.Length - aIndex < aCount)
            {
                throw new ArgumentException();
            }
            for (int i = 0; i < aCount; i++)
            {
                Write(aBuffer[aIndex + i]);
            }
        }

        public static void WriteLine() => Write(Environment.NewLine);

        public static void WriteLine(bool aBool) => WriteLine(aBool.ToString());

        public static void WriteLine(char aChar) => WriteLine(aChar.ToString());

        public static void WriteLine(char[] aBuffer) => WriteLine(new string(aBuffer));

        public static void WriteLine(decimal aDecimal) => WriteLine(aDecimal.ToString());

        public static void WriteLine(double aDouble) => WriteLine(aDouble.ToString());

        public static void WriteLine(float aFloat) => WriteLine(aFloat.ToString());

        public static void WriteLine(int aInt) => WriteLine(aInt.ToString());

        public static void WriteLine(long aLong) => WriteLine(aLong.ToString());

        public static void WriteLine(object value) => Write((value ?? String.Empty) + Environment.NewLine);

        public static void WriteLine(string aText) => Write(aText + Environment.NewLine);

        public static void WriteLine(uint aInt) => WriteLine(aInt.ToString());

        public static void WriteLine(ulong aLong) => WriteLine(aLong.ToString());

        public static void WriteLine(string format, object arg0) => WriteLine(String.Format(format, arg0));

        public static void WriteLine(string format, object arg0, object arg1) => WriteLine(String.Format(format, arg0, arg1));

        public static void WriteLine(string format, object arg0, object arg1, object arg2) => WriteLine(String.Format(format, arg0, arg1, arg2));

        public static void WriteLine(string format, params object[] arg) => WriteLine(String.Format(format, arg));

        public static void WriteLine(char[] aBuffer, int aIndex, int aCount)
        {
            Write(aBuffer, aIndex, aCount);
            WriteLine();
        }
    }
}