using SphereOS.Logging;
using SphereOS.Users;
using System;

namespace SphereOS.Shell
{
    internal static class LoginPrompt
    {


        internal static bool PromptLogin()
        {
            Util.Print(ConsoleColor.Cyan, "Username: ");
            var username = Console.ReadLine().Trim();
            User user = UserManager.GetUser(username);
            if (user != null)
            {
                if (user.LockedOut)
                {
                    Util.PrintLine(ConsoleColor.Red, $"This account has been locked out due to too many failed login attempts.");

                    TimeSpan remaining = user.LockoutEnd - DateTime.Now;
                    if (remaining.Minutes > 0)
                    {
                        Util.PrintLine(ConsoleColor.White, $"Try again in {remaining.Minutes}m, {remaining.Seconds}s.");
                    }
                    else
                    {
                        Util.PrintLine(ConsoleColor.White, $"Try again in {remaining.Seconds}s.");
                    }

                    return false;
                }
                Util.Print(ConsoleColor.Cyan, $"Password for {username}: ");
                var password = Util.ReadPassword();
                if (user.Authenticate(password))
                {
                    Kernel.CurrentUser = user;
                    Log.Info("LoginPrompt", $"{user.Username} logged on.");
                    Console.WriteLine();
                    Util.PrintLine(ConsoleColor.Green, $"Welcome to SphereOS!");
                    return true;
                }
                else
                {
                    Util.PrintLine(ConsoleColor.Red, "Incorrect password.");
                    return false;
                }
            }
            else
            {
                Util.PrintLine(ConsoleColor.Red, "Unknown user.");
                return false;
            }
        }
    }
}
