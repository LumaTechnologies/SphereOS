namespace SphereOS.HttpServer.Models
{
    /// <summary>
    /// A HTTP method that is sent by a client.
    /// </summary>
    internal enum HttpMethod
    {
        /// <summary>
        /// GET method
        /// </summary>
        GET,

        /// <summary>
        /// POST method
        /// </summary>
        POST,

        /// <summary>
        /// PUT method
        /// </summary>
        PUT,

        /// <summary>
        /// DELETE method
        /// </summary>
        DELETE,

        /// <summary>
        /// HEAD method
        /// </summary>
        HEAD,

        /// <summary>
        /// OPTIONS method
        /// </summary>
        OPTIONS,

        /// <summary>
        /// TRACE method
        /// </summary>
        TRACE,

        /// <summary>
        /// PATCH method
        /// </summary>
        PATCH,

        /// <summary>
        /// CONNECT method
        /// </summary>
        CONNECT
    }
}
