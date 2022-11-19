using System.IO;
using System.Text;

namespace SphereOS.Core
{
    internal static class PathSanitiser
    {
        private static string RemoveDuplicateDirectorySeparators(string path)
        {
            var result = new StringBuilder();
            bool lastSeparator = false;
            for (int i = 0; i < path.Length; i++)
            {
                if (path[i] == '\\')
                {
                    if (lastSeparator)
                    {
                        continue;
                    }
                    lastSeparator = true;
                }
                else
                {
                    lastSeparator = false;
                }
                result.Append(path[i]);
            }
            return result.ToString();
        }

        internal static string SanitisePath(string path)
        {
            return RemoveDuplicateDirectorySeparators(Path.GetFullPath(path).Replace('/', '\\').ToLower().Trim());
        }
    }
}
