using SphereOS.Users;
using System;

namespace SphereOS.Core
{
    internal static class FileSecurity
    {
        internal static bool CanAccess(User user, string path)
        {
            if (user == null)
            {
                throw new ArgumentException("user");
            }

            if (user.Admin)
            {
                return true;
            }

            string sanitised = PathSanitiser.SanitisePath(path);

            if (sanitised == @"0:\users.ini")
                return false;

            if (sanitised == @"0:\users")
                return false;

            if (sanitised.StartsWith(@"0:\users\"))
            {
                if (sanitised == $@"0:\users\{user.Username}")
                    return true;

                if (sanitised.StartsWith($@"0:\users\{user.Username}\"))
                    return true;

                return false;
            }

            return true;
        }
    }
}
