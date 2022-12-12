using Cosmos.System.Network.IPv4.TCP;
using SphereOS.Logging;
using System;

namespace SphereOS.HttpServer
{
    /// <summary>
    /// A HTTP web server that receives and responds to requests.
    /// </summary>
    internal class HttpServer
    {
        /// <summary>
        /// The <see cref="HttpRouter"/> that routes HTTP requests to routes.
        /// </summary>
        internal HttpRouter Router { get; init; }

        /// <summary>
        /// The <see cref="TcpListener"/> that listens to TCP requests to the server.
        /// </summary>
        internal TcpListener Listener { get; private set; }

        /// <summary>
        /// The TCP port that the server operates on.
        /// </summary>
        internal ushort Port { get; private set; }

        /// <summary>
        /// Create a new HTTP web server on a specified port.
        /// </summary>
        /// <param name="port">The TCP port that the server operates on.</param>
        internal HttpServer(ushort port)
        {
            Port = port;
            Router = new HttpRouter();
        }

        /// <summary>
        /// Start the HTTP web server.
        /// </summary>
        internal void Listen()
        {
            Listener = new TcpListener(Port);
            Listener.Start();

            Log.Info("HttpServer", $"Server started on port {Port}.");

            while (true)
            {
                Console.WriteLine("Waiting...");
                TcpClient client = Listener.AcceptTcpClient(timeout: -1);
                Console.WriteLine($"Connection: {client.RemoteEndPoint.Address.ToString()}");
                Router.Handle(client);
            }
            // todo: dispose listener
        }
    }
}
