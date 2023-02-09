using System;
using System.Collections.Generic;

namespace SphereOS.Shell
{
    /// <summary>
    /// Utilities for the console.
    /// </summary>
    internal static class Util
    {
        internal static void Print(ConsoleColor color, string text)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = oldColor;
        }

        internal static void PrintLine(ConsoleColor color, string text)
        {
            Print(color, text + "\n");
        }

        internal static void Print(ConsoleColor color, object obj)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(obj.ToString());
            Console.ForegroundColor = oldColor;
        }

        internal static void PrintLine(ConsoleColor color, object obj)
        {
            Print(color, obj.ToString() + "\n");
        }

        internal static void PrintSystem(string task)
        {
            Print(ConsoleColor.Cyan, "SphereOS - ");
            PrintLine(ConsoleColor.White, task);
        }

        internal static void PrintWarning(string warning)
        {
            Print(ConsoleColor.Cyan, "SphereOS - ");
            Print(ConsoleColor.Yellow, "Warning - ");
            PrintLine(ConsoleColor.White, warning);
        }

        private static void SetCursorPosWrap(int left, int top)
        {
            if (left < 0)
            {
                left = Console.WindowWidth - 1;
                top--;
            }
            if (left >= Console.WindowWidth)
            {
                left = 0;
                top++;
            }

            Console.SetCursorPosition(left, top);
        }

        /// <summary>
        /// Read line extended.
        /// </summary>
        /// <param name="cancelKey">An optional key that will cancel the function and return null.</param>
        /// <param name="mask">Whether to mask the password.</param>
        /// <returns>The text entered, or null if cancelKey was pressed.</returns>
        internal static ReadLineExResult ReadLineEx(Cosmos.System.ConsoleKeyEx[]? cancelKeys = null, bool mask = false, string initialValue = "", bool clearOnCancel = false)
        {
            var chars = new List<char>(32);
            Cosmos.System.KeyEvent current;
            int currentCount = 0;
            if (initialValue != null)
            {
                chars.AddRange(initialValue.ToCharArray());
                Console.Write(initialValue);
                currentCount = initialValue.Length;
            }
            while ((current = Cosmos.System.KeyboardManager.ReadKey()).Key != Cosmos.System.ConsoleKeyEx.Enter)
            {
                if (cancelKeys != null && Array.IndexOf(cancelKeys, current.Key) != -1)
                {
                    if (clearOnCancel)
                    {
                        while (chars.Count > 0)
                        {
                            int curCharTemp = Console.GetCursorPosition().Left;
                            chars.RemoveAt(currentCount - 1);
                            SetCursorPosWrap(Console.GetCursorPosition().Left - 1, Console.GetCursorPosition().Top);

                            for (int x = currentCount - 1; x < chars.Count; x++)
                            {
                                Console.Write(chars[x]);
                            }

                            Console.Write(' ');

                            SetCursorPosWrap(curCharTemp - 1, Console.GetCursorPosition().Top);

                            currentCount--;
                        }
                    }
                    return new ReadLineExResult(cancelKey: current.Key);
                }
                if (current.Key == Cosmos.System.ConsoleKeyEx.NumEnter)
                {
                    break;
                }
                if (current.Key == Cosmos.System.ConsoleKeyEx.Backspace)
                {
                    if (currentCount > 0)
                    {
                        int curCharTemp = Console.GetCursorPosition().Left;
                        chars.RemoveAt(currentCount - 1);
                        SetCursorPosWrap(Console.GetCursorPosition().Left - 1, Console.GetCursorPosition().Top);

                        for (int x = currentCount - 1; x < chars.Count; x++)
                        {
                            Console.Write(chars[x]);
                        }

                        Console.Write(' ');

                        SetCursorPosWrap(curCharTemp - 1, Console.GetCursorPosition().Top);

                        currentCount--;
                    }
                    continue;
                }
                else if (current.Key == Cosmos.System.ConsoleKeyEx.LeftArrow)
                {
                    if (currentCount > 0)
                    {
                        SetCursorPosWrap(Console.GetCursorPosition().Left - 1, Console.GetCursorPosition().Top);
                        currentCount--;
                    }
                    continue;
                }
                else if (current.Key == Cosmos.System.ConsoleKeyEx.RightArrow)
                {
                    if (currentCount < chars.Count)
                    {
                        SetCursorPosWrap(Console.GetCursorPosition().Left + 1, Console.GetCursorPosition().Top);
                        currentCount++;
                    }
                    continue;
                }

                if (current.KeyChar == '\0')
                {
                    continue;
                }

                if (currentCount == chars.Count)
                {
                    chars.Add(current.KeyChar);
                    Console.Write(mask ? '*' : chars[chars.Count - 1]);
                    currentCount++;
                }
                else
                {
                    var temp = new List<char>();

                    for (int x = 0; x < chars.Count; x++)
                    {
                        if (x == currentCount)
                        {
                            temp.Add(current.KeyChar);
                        }

                        temp.Add(chars[x]);
                    }

                    chars = temp;

                    for (int x = currentCount; x < chars.Count; x++)
                    {
                        Console.Write(mask ? '*' : chars[x]);
                    }

                    SetCursorPosWrap(Console.GetCursorPosition().Left - (chars.Count - currentCount) - 1, Console.GetCursorPosition().Top);
                    currentCount++;
                }
            }
            Console.WriteLine();

            char[] final = chars.ToArray();
            return new ReadLineExResult(new string(final));
        }

        internal static void PrintTable(List<List<string>> columns, ConsoleColor color = ConsoleColor.White, ConsoleColor headerColour = ConsoleColor.White, int margin = 2)
        {
            int[] longestWidths = new int[columns.Count];
            int tallestColumn = 0;
            for (int i = 0; i < columns.Count; i++)
            {
                for (int j = 0; j < columns[i].Count; j++)
                {
                    longestWidths[i] = Math.Max(longestWidths[i], columns[i][j].Length);
                }
                tallestColumn = Math.Max(tallestColumn, columns[i].Count);
            }
            int rowCount = 0;
            foreach (var column in columns)
            {
                rowCount = Math.Max(rowCount, column.Count);
            }
            string[] rows = new string[rowCount];
            for (int i = 0; i < columns.Count; i++)
            {
                for (int j = 0; j < tallestColumn; j++)
                {
                    if (j >= columns[i].Count)
                    {
                        rows[j] += new string(' ', longestWidths[i] + margin);
                    }
                    else
                    {
                        string cell = columns[i][j];
                        rows[j] += cell + new string(' ', Math.Max(0, longestWidths[i] - cell.Length + margin));
                    }
                }
            }
            for (int i = 0; i < rows.Length; i++)
            {
                if (i == 0)
                {
                    PrintLine(headerColour, rows[i]);
                }
                else
                {
                    PrintLine(color, rows[i]);
                }
            }
        }
    }
}
