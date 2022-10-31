using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SphereOS
{
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

        internal static string ReadPassword()
        {
            var chars = new List<char>(32);
            Cosmos.System.KeyEvent current;
            int currentCount = 0;

            while ((current = Cosmos.System.KeyboardManager.ReadKey()).Key != Cosmos.System.ConsoleKeyEx.Enter)
            {
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
    }
}
