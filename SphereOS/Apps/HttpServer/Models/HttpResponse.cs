using Cosmos.System.Network.IPv4.TCP;
using System;
using System.Text;

namespace SphereOS.HttpServer.Models
{
    /// <summary>
    /// Represents a response to a HTTP request.
    /// </summary>
    internal class HttpResponse
    {
        /// <summary>
        /// The data to send to the client.
        /// </summary>
        internal byte[] Data { get; set; }

        /// <summary>
        /// The <see cref="HttpStatus"/> status code to send to the client.
        /// </summary>
        internal HttpStatus Status { get; set; }

        /// <summary>
        /// Create a new <see cref="HttpResponse"/> with the specified data and status code.
        /// </summary>
        /// <param name="data">The data to send to the client.</param>
        /// <param name="status">The <see cref="HttpStatus"/> status code to send to the client.</param>
        internal HttpResponse(byte[] data, HttpStatus status)
        {
            Data = data;
            Status = status;
        }

        /// <summary>
        /// Returns the name for a specified <see cref="HttpStatus"/>.
        /// </summary>
        /// <param name="status">The status code.</param>
        /// <returns>The name of the status code.</returns>
        private string GetHttpStatusName(HttpStatus status)
        {
            switch (status)
            {
                case HttpStatus.OK:
                    return "OK";
                default:
                    Console.WriteLine("ERROR::GetHttpStatusName: Unknown.");
                    return "Unknown";
            }
        }

        /// <summary>
        /// Sends the response to a specified <see cref="TcpClient"/>.
        /// </summary>
        /// <param name="client">The client to send the response to.</param>
        internal void SendToTcpClient(TcpClient client)
        {
            Console.WriteLine("Sending response to client + closing");
            StringBuilder stringBuilder = new();

            string statusName = GetHttpStatusName(Status);

            stringBuilder.Append($"HTTP/1.1 {Status} {statusName}\n\n");
            stringBuilder.Append(Data);

            client.Close();
        }
    }
}
