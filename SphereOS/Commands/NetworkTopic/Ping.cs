using Cosmos.System;
using Cosmos.System.Network.IPv4;
using SphereOS.Shell;
using System;
using Console = System.Console;
using EndPoint = Cosmos.System.Network.IPv4.EndPoint;

namespace SphereOS.Commands.NetworkTopic
{
    internal class Ping : Command
    {
        public Ping() : base("ping")
        {
            Description = "Ping an IP address.";

            Topic = "network";
        }

        private const int echoCount = 4;

        private bool ParseAddress(string ip, out Address address)
        {
            string[] octetStrings = ip.Split('.');
            byte[] octets = new byte[4];
            if (octetStrings.Length != 4)
            {
                address = Address.Zero;
                return false;
            }
            for (int i = 0; i < octetStrings.Length; i++)
            {
                if (byte.TryParse(octetStrings[i], out byte octet))
                {
                    octets[i] = octet;
                }
                else
                {
                    address = Address.Zero;
                    return false;
                }
            }
            address = new Address(octets[0], octets[1], octets[2], octets[3]);
            return true;
        }

        internal override ReturnCode Execute(string[] args)
        {
            if (args.Length != 2)
            {
                Util.PrintLine(ConsoleColor.Red, "Invalid usage. Please provide an IP address.");
                return ReturnCode.Invalid;
            }

            var ip = args[1];

            if (ParseAddress(ip, out Address address))
            {
                Console.WriteLine();

                Util.PrintLine(ConsoleColor.Cyan, $"Pinging {address.ToString()}:");

                EndPoint endpoint = new EndPoint(Address.Zero, 0);

                int sent = 0;
                int received = 0;

                using (var icmp = new ICMPClient())
                {
                    icmp.Connect(address);
                    for (int i = 0; i < echoCount; i++)
                    {
                        icmp.SendEcho();
                        sent++;
                        int time = icmp.Receive(ref endpoint);
                        if (time != -1)
                        {
                            received++;
                            Util.PrintLine(ConsoleColor.White, $"Reply from {address.ToString()}: time={time - 1}s");
                        }
                        else
                        {
                            Util.PrintLine(ConsoleColor.Red, "Request timed out.");
                        }
                        if (KeyboardManager.TryReadKey(out KeyEvent key))
                        {
                            if (key.Modifiers == ConsoleModifiers.Control && key.Key == ConsoleKeyEx.C)
                            {
                                return ReturnCode.Aborted;
                            }
                        }
                    }
                    icmp.Close();
                }

                Console.WriteLine();
                Util.PrintLine(ConsoleColor.Cyan, $"Ping statistics for {address.ToString()}:");
                int lossPercent = (int)((sent - received) / (float)sent * 100);
                Util.PrintLine(ConsoleColor.White, $"    Packets: Sent = {sent}, Received = {received}, Lost = {sent - received} ({lossPercent}% loss)");
                Console.WriteLine();

                if (sent == received)
                {
                    return ReturnCode.Success;
                }
                else
                {
                    return ReturnCode.Failure;
                }
            }
            else
            {
                Util.PrintLine(ConsoleColor.Red, "Invalid IP address.");
                return ReturnCode.Invalid;
            }
        }
    }
}
