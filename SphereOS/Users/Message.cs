using System;

namespace SphereOS.Users
{
    internal class Message
    {
        internal Message(User from, string body)
        {
            From = from;
            Body = body;
            Sent = DateTime.Now;
        }

        internal Message(User from, string body, DateTime sent)
        {
            From = from;
            Body = body;
            Sent = sent;
        }

        internal User From { get; set; }

        internal string Body { get; set; }

        internal DateTime Sent { get; set; }
    }
}
