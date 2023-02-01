using SphereOS.Shell;
using SphereOS.Users;
using System;

namespace SphereOS.Commands.UsersTopic
{
    internal class Adduser : Command
    {
        public Adduser() : base("adduser")
        {
            Description = "Add a new user.";

            Usage = "<user>";

            Topic = "users";
        }

        internal override ReturnCode Execute(string[] args)
        {
            if (!Kernel.CurrentUser.Admin)
            {
                Util.PrintLine(ConsoleColor.Red, "Unauthorised. You must be an admin to run this command.");
                return ReturnCode.Unauthorised;
            }

            if (args.Length != 2)
            {
                Util.PrintLine(ConsoleColor.Red, "Invalid usage. Please provide a new username.");
                return ReturnCode.Invalid;
            }

            var username = args[1].Trim();
            if (UserManager.GetUser(username) != null)
            {
                Util.PrintLine(ConsoleColor.Red, $"A user named {username} already exists.");
                return ReturnCode.Failure;
            }

            Util.Print(ConsoleColor.Cyan, $"New password for {username}: ");
            var result = Util.ReadLineEx(mask: true);

            User user = UserManager.AddUser(username, result.Input, admin: false);

            Util.PrintLine(ConsoleColor.Green, $"Successfully created user {username}.");

            Util.Print(ConsoleColor.White, @$"Create a home directory for {username} in 0:\users\{username}? [y/N]: ");
            if (Console.ReadKey(true).Key == ConsoleKey.Y)
            {
                Console.WriteLine();

                UserManager.CreateHomeDirectory(user);

                Util.PrintLine(ConsoleColor.Green, "Home directory created.");
            }

            return ReturnCode.Success;
        }
    }
}
