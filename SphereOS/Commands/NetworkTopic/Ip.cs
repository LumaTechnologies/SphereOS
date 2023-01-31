using Cosmos.System;
using Cosmos.System.Network.IPv4;
using SphereOS.Shell;
using System;
using Console = System.Console;
using EndPoint = Cosmos.System.Network.IPv4.EndPoint;
using Cosmos.System.Network.Config;

namespace SphereOS.Commands.NetworkTopic
{
    internal class Ip : Command
    {
        public Ip() : base("ip")
        {
            Description = "Get the PC's private IP address.";

            Topic = "network";
        }

        internal override ReturnCode Execute(string[] args)
        {
            Console.WriteLine(NetworkConfiguration.CurrentNetworkConfig.IPConfig.IPAddress.ToString());

            return ReturnCode.Success;
        }
    }
}
