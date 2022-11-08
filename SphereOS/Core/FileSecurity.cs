using SphereOS.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                return true;

            switch (path.ToLower())
            {
                case @"0:\users.ini":
                    return false;
                default:
                    return true;
            }
        }
    }
}
