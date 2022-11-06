using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SphereOS
{
    internal class Message
    {
        internal Message(User from, string body)
        {
            From = from;
            Body = body;
            Sent = DateTime.Now;
        }

        internal User From { get; set; }

        internal string Body { get; set; }

        internal DateTime Sent { get; set; }
    }
}
