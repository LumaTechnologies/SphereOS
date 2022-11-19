using SphereOS.Users;
using System;

namespace SphereOS.Commands.UsersTopic
{
    internal class Broadcast : Command
    {
        public Broadcast() : base("broadcast")
        {
            Description = "Broadcast a message to all users.";

            Topic = "users";
        }

        internal override ReturnCode Execute(string[] args)
        {
            if (!Kernel.CurrentUser.Admin)
            {
                Util.PrintLine(ConsoleColor.Red, "Unauthorised. You must be an admin to run this command.");
                return ReturnCode.Unauthorised;
            }

            if (args.Length < 2)
            {
                Util.PrintLine(ConsoleColor.Red, "Invalid usage. Please provide the message.");
                return ReturnCode.Invalid;
            }

            string body = string.Empty;
            for (int i = 1; i < args.Length; i++)
            {
                body += args[i];
                if (i != args.Length - 1)
                {
                    body += " ";
                }
            }

            foreach (User user in UserManager.Users)
            {
                user.Messages.Add(new Message(from: Kernel.CurrentUser, body));
            }
            UserManager.Flush();

            Util.PrintLine(ConsoleColor.Green, $"Sent {UserManager.Users.Count} message(s).");

            return ReturnCode.Success;
        }
    }
}
