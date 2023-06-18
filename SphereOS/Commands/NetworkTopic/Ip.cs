using Cosmos.System.Network.Config;
using Console = System.Console;

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
