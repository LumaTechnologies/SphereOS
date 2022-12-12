using Cosmos.System.Network.IPv4.TCP;
using SphereOS.HttpServer.Models;
using SphereOS.HttpServer.Routes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SphereOS.HttpServer
{
    /// <summary>
    /// Represents an object that routes HTTP requests to the desired <see cref="HttpRoute"/>.
    /// </summary>
    internal class HttpRouter
    {
        /// <summary>
        /// The <see cref="HttpRoute"/> that the router contains.
        /// </summary>
        List<HttpRoute> Routes { get; set; } = new List<HttpRoute>();

        /// <summary>
        /// Receives a request from a <see cref="TcpClient"/> and returns a response from the requested <see cref="HttpRoute"/>.
        /// </summary>
        /// <param name="client">The <see cref="TcpClient"/> that is sending the request.</param>
        internal void Handle(TcpClient client)
        {
            Console.WriteLine("Handling");

            HttpRequest request = HttpRequest.FromTcpClient(client);
            foreach (var route in Routes)
            {
                if (request.Path == route.Path)
                {
                    HttpResponse response = route.Func.Invoke(request);

                    response.SendToTcpClient(client);

                    return;
                }
            }

            HttpResponse notFoundResponse = new HttpResponse(Encoding.UTF8.GetBytes("Not Found"), HttpStatus.NotFound);

            notFoundResponse.SendToTcpClient(client);
        }
    }
}
