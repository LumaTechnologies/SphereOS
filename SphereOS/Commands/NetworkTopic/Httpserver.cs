using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using System;

namespace SphereOS.Commands.NetworkTopic
{
    internal class Httpserver : Command
    {
        public Httpserver() : base("httpserver")
        {
            Description = "Start the SphereOS Web Server.";

            Topic = "network";
        }

        private static Address DnsAddress = new Address(8, 8, 8, 8);

        internal override ReturnCode Execute(string[] args)
        {
            HttpServer.HttpServer server = new HttpServer.HttpServer(8080);

            Console.WriteLine($"IP: {NetworkConfiguration.CurrentNetworkConfig.IPConfig.IPAddress.ToString()}");
            Console.WriteLine($"Starting HTTP server. (Port: {server.Port})");
            server.Listen();

            return ReturnCode.Success;
        }
    }
}
