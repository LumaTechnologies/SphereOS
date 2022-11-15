using SphereOS.Logging;
using SphereOS.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SphereOS.Core
{
    internal static class FileSecurity
    {
        internal static bool CanAccess(User user, string path)
        {
            if (user.Admin)
            {
                return true;
            }

            string sanitised = PathSanitiser.SanitisePath(path);

            if (Path.TrimEndingDirectorySeparator(sanitised).EndsWith(@"\policies.ini"))
            {
                return false;
            }

            if (sanitised == @"0:\users.ini")
            {
                return false;
            }

            Policy policy = PolicyManager.GetPathPolicy(sanitised);

            return policy.Authenticate(user);
        }
    }
}
