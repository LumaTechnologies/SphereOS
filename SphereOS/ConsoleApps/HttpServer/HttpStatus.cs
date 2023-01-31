namespace SphereOS.HttpServer
{
    /// <summary>
    /// A HTTP status code that is returned by the server in a HTTP response.
    /// </summary>
    internal enum HttpStatus
    {
        /// <summary>
        /// OK (200)
        /// </summary>
        OK = 200,

        /// <summary>
        /// Not Found (404)
        /// </summary>
        NotFound = 404,

        /// <summary>
        /// Bad Request (400)
        /// </summary>
        BadRequest = 400,

        /// <summary>
        /// Forbidden (403)
        /// </summary>
        Forbidden = 403
    }
}
