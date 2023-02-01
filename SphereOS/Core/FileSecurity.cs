using SphereOS.Users;

namespace SphereOS.Core
{
    internal static class FileSecurity
    {
        internal static bool CanAccess(User user, string path)
        {
            if (user != null && user.Admin)
            {
                return true;
            }

            string sanitised = PathSanitiser.SanitisePath(path);

            if (sanitised.StartsWith(@"0:\etc\") || sanitised == @"0:\etc")
                return false;

            // Legacy file
            if (sanitised == @"0:\users.ini")
                return false;

            // Legacy file
            if (sanitised == @"0:\settings.ini")
                return false;

            if (sanitised == @"0:\users")
                return false;

            if (sanitised.StartsWith(@"0:\users\"))
            {
                if (user == null)
                    return false;

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
