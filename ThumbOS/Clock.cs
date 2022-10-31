using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThumbOS
{
    internal static class Clock
    {
        private static string GetDate()
        {
            return DateTime.Now.ToString("dddd, dd/MM/yyyy HH:mm:ss");
        }

        internal static void Main(string[] args)
        {
            Util.Print(ConsoleColor.Gray, "[!]");
            Util.PrintLine(ConsoleColor.White, " Tip: Press ESC to exit the clock.");
            Util.PrintLine(ConsoleColor.Cyan, "Enter a message to display (optional): ");
            var message = Console.ReadLine();

            Console.CursorVisible = false;
            Console.Clear();

            int startY = 0;
            if (message != "")
            {
                Util.PrintLine(ConsoleColor.Cyan, message);
                startY = Console.GetCursorPosition().Top + 1;
            }

            string lastDate = "";
            while (true)
            {
                if (Cosmos.System.KeyboardManager.TryReadKey(out var key))
                {
                    if (key.Key == Cosmos.System.ConsoleKeyEx.Escape)
                    {
                        break;
                    }
                }

                string date = GetDate();

                if (lastDate != date)
                {
                    Console.SetCursorPosition(0, startY);

                    Util.Print(ConsoleColor.White, date + new string(' ', Console.WindowWidth - date.Length));
                }

                lastDate = date;
                Cosmos.Core.Memory.Heap.Collect();
            }

            Console.CursorVisible = true;
            Console.SetCursorPosition(0, 0);
            Console.Clear();
        }
    }
}
