using SphereOS.Logging;
using SphereOS.Shell;
using SphereOS.Users;
using System;

namespace SphereOS.Commands.UsersTopic
{
    internal class Send : Command
    {
        public Send() : base("send")
        {
            Description = "Send a message to another user on the PC.";

            Usage = "<user> <message>";

            Topic = "users";
        }

        internal override ReturnCode Execute(string[] args)
        {
            if (args.Length < 3)
            {
                Util.PrintLine(ConsoleColor.Red, "Invalid usage. Please provide the user to send the message to and the message.");
                return ReturnCode.Invalid;
            }

            User user = UserManager.GetUser(args[1]);
            if (user == null)
            {
                Util.PrintLine(ConsoleColor.Red, "No users were affected.");
                return ReturnCode.Failure;
            }

            string body = string.Empty;
            for (int i = 2; i < args.Length; i++)
            {
                body += args[i];
                if (i != args.Length - 1)
                {
                    body += " ";
                }
            }

            user.Messages.Add(new Message(from: Kernel.CurrentUser, body));
            UserManager.Flush();

            Util.PrintLine(ConsoleColor.Green, $"Sent message to '{user.Username}'.");

            Log.Info("Send", $"User '{Kernel.CurrentUser.Username}' sent a message to '{user.Username}'.");

            return ReturnCode.Success;
        }
    }
}
