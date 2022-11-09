using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SphereOS
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

        internal static void PrintTask(string task)
        {
            Print(ConsoleColor.Gray, "[SphereOS] ");
            PrintLine(ConsoleColor.White, task);
        }

        /// <summary>
        /// Read a password from the console.
        /// </summary>
        /// <param name="cancelKey">An optional key that will cancel the function and return null.</param>
        /// <returns>The password entered, or null if cancelKey was pressed.</returns>
        internal static string ReadPassword(Cosmos.System.ConsoleKeyEx? cancelKey = null)
        {
            var chars = new List<char>(32);
            Cosmos.System.KeyEvent current;
            int currentCount = 0;

            while ((current = Cosmos.System.KeyboardManager.ReadKey()).Key != Cosmos.System.ConsoleKeyEx.Enter)
            {
                if (current.Key == cancelKey)
                {
                    return null;
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
                        Console.SetCursorPosition(Console.GetCursorPosition().Left - 1, Console.GetCursorPosition().Top);

                        for (int x = currentCount - 1; x < chars.Count; x++)
                        {
                            Console.Write(chars[x]);
                        }

                        Console.Write(' ');

                        Console.SetCursorPosition(curCharTemp - 1, Console.GetCursorPosition().Top);

                        currentCount--;
                    }
                    continue;
                }
                else if (current.Key == Cosmos.System.ConsoleKeyEx.LeftArrow)
                {
                    if (currentCount > 0)
                    {
                        Console.SetCursorPosition(Console.GetCursorPosition().Left - 1, Console.GetCursorPosition().Top);
                        currentCount--;
                    }
                    continue;
                }
                else if (current.Key == Cosmos.System.ConsoleKeyEx.RightArrow)
                {
                    if (currentCount < chars.Count)
                    {
                        Console.SetCursorPosition(Console.GetCursorPosition().Left + 1, Console.GetCursorPosition().Top);
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
                    Console.Write('*');
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
                        Console.Write('*');
                    }

                    Console.SetCursorPosition(Console.GetCursorPosition().Left - (chars.Count - currentCount) - 1, Console.GetCursorPosition().Top);
                    currentCount++;
                }
            }
            Console.WriteLine();

            char[] final = chars.ToArray();
            return new string(final);
        }

        internal static void PrintTable(List<List<string>> columns, ConsoleColor color = ConsoleColor.White, ConsoleColor headerColour = ConsoleColor.White, int margin = 2)
        {
            int[] longestWidths = new int[columns.Count];
            for (int i = 0; i < columns.Count; i++)
            {
                for (int j = 0; j < columns[i].Count; j++)
                {
                    longestWidths[i] = Math.Max(longestWidths[i], columns[i][j].Length);
                }
            }
            int rowCount = 0;
            foreach (var column in columns)
            {
                rowCount = Math.Max(rowCount, column.Count);
            }
            string[] rows = new string[rowCount];
            for (int i = 0; i < columns.Count; i++)
            {
                for (int j = 0; j < columns[i].Count; j++)
                {
                    string cell = columns[i][j];
                    rows[j] += cell + new string(' ', longestWidths[i] - cell.Length + margin);
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
