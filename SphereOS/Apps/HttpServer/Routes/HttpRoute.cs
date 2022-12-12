using SphereOS.HttpServer.Models;
using System;

namespace SphereOS.HttpServer.Routes
{
    /// <summary>
    /// Represents a route for a HTTP web server.
    /// </summary>
    internal class HttpRoute
    {
        /// <summary>
        /// Create a new route with the GET method and the specified path.
        /// </summary>
        /// <param name="path">The path that the route responds to.</param>
        internal HttpRoute(string path)
        {
            Method = HttpMethod.GET;
            Path = path;
        }

        /// <summary>
        /// Create a new route with the specified method and path.
        /// </summary>
        /// <param name="method">The method that the route responds to.</param>
        /// <param name="path">The path that the route responds to.</param>
        internal HttpRoute(HttpMethod method, string path)
        {
            Method = method;
            Path = path;
        }

        /// <summary>
        /// The method that the route responds to.
        /// </summary>
        internal HttpMethod Method { get; set; }

        /// <summary>
        /// The path that the route responds to.
        /// </summary>
        internal string Path { get; set; }

        /// <summary>
        /// The <see cref="Func{HttpRequest, HttpResponse}"/> that is called to generate a response to a request for this route.
        /// </summary>
        internal Func<HttpRequest, HttpResponse> Func { get; set; }
    }
}
