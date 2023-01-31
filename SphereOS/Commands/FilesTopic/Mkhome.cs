using SphereOS.Shell;
using SphereOS.Users;
using System;
using System.IO;

namespace SphereOS.Commands.FilesTopic
{
    internal class Mkhome : Command
    {
        public Mkhome() : base("mkhome")
        {
            Description = "Create a home directory for a user.";

            Usage = "<user>";

            Topic = "files";
        }

        internal override ReturnCode Execute(string[] args)
        {
            if (args.Length != 2)
            {
                Util.PrintLine(ConsoleColor.Red, "Invalid usage. Please provide the username of the user.");
                return ReturnCode.Invalid;
            }

            if (!Kernel.CurrentUser.Admin)
            {
                Util.PrintLine(ConsoleColor.Red, "Unauthorised. You must be an admin to run this command.");
                return ReturnCode.Unauthorised;
            }

            User user = UserManager.GetUser(args[1]);

            if (user == null)
            {
                Util.PrintLine(ConsoleColor.Red, "Unknown user.");
                return ReturnCode.Failure;
            }

            if (Directory.Exists($@"0:\users\{user.Username}"))
            {
                Util.PrintLine(ConsoleColor.Red, "This user already has a home directory.");
                return ReturnCode.Failure;
            }

            UserManager.CreateHomeDirectory(user);

            Util.PrintLine(ConsoleColor.Green, $@"Created a home directory at 0:\users\{user.Username}.");

            Util.Print(ConsoleColor.Gray, $@"[i] Tip: Run");
            Util.Print(ConsoleColor.White, $@" cd ~ ");
            Util.PrintLine(ConsoleColor.Gray, $@"to navigate to your home directory.");

            return ReturnCode.Success;
        }
    }
}
