using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.TCP;
using Cosmos.System.Network.IPv4.UDP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SphereOS
{
    internal static class CloudChat
    {
        private static string input = "";
        private static string userId;
        private static EndPoint endpoint = new EndPoint(Address.Zero, 0);
        internal static bool Running = false;
        private static UdpClient xClient;
        private static Random random = new Random();
        private static int lastSecond = 0;

        private static void DrawTextBox()
        {
            (int, int) oldPos = Console.GetCursorPosition();
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = ConsoleColor.Black;

            string textToDisplay = input + "_";
            if (input == "")
            {
                textToDisplay = "Type a message...";
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            Console.Write(textToDisplay + new string(' ', Console.WindowWidth - textToDisplay.Length));

            Console.SetCursorPosition(oldPos.Item1, oldPos.Item2);

            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
        }

        internal static void Loop()
        {
            int second = DateTime.UtcNow.Second;
            if (lastSecond != second)
            {
                lastSecond = second;
                xClient.Send(Encoding.ASCII.GetBytes($"{userId}_refresh:"));
            }
            if (Cosmos.System.KeyboardManager.TryReadKey(out var key))
            {
                if (key.Key == Cosmos.System.ConsoleKeyEx.Backspace)
                {
                    if (input.Length >= 1)
                    {
                        input = input.Substring(0, input.Length - 1);
                        DrawTextBox();
                    }
                    return;
                }
                else if (key.Key == Cosmos.System.ConsoleKeyEx.Enter)
                {
                    if (input != "")
                    {
                        xClient.Send(Encoding.ASCII.GetBytes($"{userId}_message:{input}"));
                        input = "";
                        DrawTextBox();
                    }
                }
                else
                {
                    input += key.KeyChar;
                    DrawTextBox();
                }
            }
            var data = xClient.NonBlockingReceive(ref endpoint);
            if (data != null)
            {
                string dataString = Encoding.ASCII.GetString(data);
                int index = dataString.IndexOf(':');
                string option = dataString.Substring(0, index);
                string message = dataString.Substring(index + 1);
                switch (option)
                {
                    case "message":
                        Console.WriteLine(message);
                        DrawTextBox();
                        break;
                    default:
                        throw new Exception("Unknown server command!");
                }
            }
        }

        private static void GenerateUserId()
        {
            var builder = new StringBuilder(16);
            for (int i = 0; i < 16; i++)
            {
                builder.Append(random.Next(0, 15).ToString("X"));
            }
            userId = builder.ToString();
        }

        internal static void Init()
        {
            input = "";
            Running = true;
            GenerateUserId();

            Console.BackgroundColor = ConsoleColor.Blue;
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = false;

            Util.PrintLine(ConsoleColor.White, "Connecting to online services...");

            xClient = new UdpClient(random.Next(49152, 65535));

            xClient.Connect(new Address(178, 62, 77, 32), 4242);
            xClient.Send(Encoding.ASCII.GetBytes($"{userId}_connect:{Kernel.Version}"));

            Console.Clear();
            Console.SetCursorPosition(0, 1);
            DrawTextBox();
        }
    }
}