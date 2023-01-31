using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.TCP;
using System;
using System.Text;

namespace SphereOS.HttpServer.Models
{
    /// <summary>
    /// Represents a request to a HTTP web server.
    /// </summary>
    internal class HttpRequest
    {
        /// <summary>
        /// The <see cref="HttpMethod"/> that the client is using.
        /// </summary>
        internal HttpMethod Method { get; set; }

        /// <summary>
        /// The path that the client is requesting.
        /// <para>
        /// Examples:
        /// <list type="bullet">
        /// <item>
        /// <code>/</code>
        /// </item>
        /// <item>
        /// <code>/index.html</code>
        /// </item>
        /// </list>
        /// </para>
        /// </summary>
        internal string Path { get; set; }

        /// <summary>
        /// Create a <see cref="HttpRequest"/> from a specified <see cref="TcpClient"/>.
        /// </summary>
        /// <param name="client">The <see cref="TcpClient"/> to receive the request from.</param>
        /// <returns>The <see cref="HttpRequest"/>.</returns>
        /// <exception cref="Exception">The client sent an invalid request.</exception>
        internal static HttpRequest FromTcpClient(TcpClient client)
        {
            HttpRequest request = new HttpRequest();

            EndPoint endpoint = new EndPoint(Address.Zero, 0);

            byte[] rawData = client.Receive(ref endpoint);

            string data = Encoding.ASCII.GetString(rawData).Replace("\r", "");
            Console.WriteLine(data);

            string[] lines = data.Split('\n');

            string[] requestLine = lines[0].Split(' ');

            if (requestLine.Length != 3)
            {
                throw new Exception("Invalid HTTP header!");
            }

            string method = requestLine[0].ToUpper();
            string path = requestLine[1];
            string protocol = requestLine[2];

            request.Path = path;

            switch (method)
            {
                case "GET":
                    request.Method = HttpMethod.GET;
                    break;
                default:
                    throw new Exception("Unknown HTTP method!");
            }

            return request;
        }
    }
}
