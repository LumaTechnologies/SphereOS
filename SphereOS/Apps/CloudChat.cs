using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.UDP;
using SphereOS.Core;
using SphereOS.Shell;
using System;
using System.Text;

namespace SphereOS.Apps
{
    internal class CloudChat
    {
        private string input = "";
        private string userId;
        private EndPoint endpoint = new EndPoint(Address.Zero, 0);
        private bool Quit = false;
        private UdpClient xClient;
        private Random random = new Random();
        private int lastSecond = 0;

        private void DrawTextBox()
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

        internal void Loop()
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
                    if (input.Trim() == "/exit")
                    {
                        Quit = true;
                        return;
                    }
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
            ProcessManager.Yield();
        }

        private void GenerateUserId()
        {
            var builder = new StringBuilder(16);
            for (int i = 0; i < 16; i++)
            {
                builder.Append(random.Next(0, 15).ToString("X"));
            }
            userId = builder.ToString();
        }

        internal void Init()
        {
            input = "";
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

            while (!Quit)
            {
                Loop();
            }

            xClient.Close();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = true;
        }
    }
}