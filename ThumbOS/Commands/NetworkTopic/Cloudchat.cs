using Cosmos.System.Network.IPv4.UDP.DNS;
using Cosmos.System.Network.IPv4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThumbOS.Commands.NetworkTopic
{
    internal class Cloudchat : Command
    {
        public Cloudchat() : base("cloudchat")
        {
            Description = "Start CloudChat!";

            Topic = "network";
        }

        internal override ReturnCode Execute(string[] args)
        {
            CloudChat.Init();

            return ReturnCode.Success;
        }
    }
}
