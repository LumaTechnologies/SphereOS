﻿using SphereOS.Shell;
using System;

namespace SphereOS.Commands.TimeTopic
{
    internal class Date : Command
    {
        public Date() : base("date")
        {
            Description = "Show the current date and time.";

            Topic = "time";
        }

        internal override ReturnCode Execute(string[] args)
        {
            Util.PrintLine(ConsoleColor.White, DateTime.Now.ToString("dddd, dd/MM/yyyy HH:mm:ss"));
            return ReturnCode.Success;
        }
    }
}
