using Cosmos.System.Graphics.Fonts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SphereOS.Core
{
    internal static class PathUtil
    {
        private static void ApplySyntax(List<string> split)
        {
            for (int i = 0; i < split.Count; i++)
            {
                if (split[i] == "~" && i == 0)
                {
                    split.RemoveAt(i);
                    split.InsertRange(0, new string[] { "0:", "users", Kernel.CurrentUser.Username });
                }
                else if (split[i] == ".")
                {
                    split.RemoveAt(i);
                    i--;
                }
                else if (split[i] == "..")
                {
                    if (i < 2)
                    {
                        split.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        split.RemoveRange(i - 1, 2);
                        i -= 2;
                    }
                }
            }
        }

        private static void RemoveEmpty(List<string> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                if (list[i].Length == 0)
                {
                    list.RemoveAt(i);
                }
            }
        }

        internal static string JoinPaths(string front, string back)
        {
            front = PathSanitiser.RemoveDuplicateDirectorySeparators(front).Replace('/', '\\').Trim();
            back = PathSanitiser.RemoveDuplicateDirectorySeparators(back).Replace('/', '\\').Trim();

            List<string> frontParts = new List<string>(front.Split('\\'));
            List<string> backParts = new List<string>(back.Split('\\'));

            RemoveEmpty(frontParts);
            RemoveEmpty(backParts);

            List<string> parts;

            // Check if the back path is a rooted path.
            if (backParts[0].EndsWith(':') || backParts[0] == "~" || back[0] == '\\')
            {
                // If the back path starts with a path separator, we must assume the drive from the front path.
                if (back[0] == '\\')
                {
                    if (!frontParts[0].EndsWith(':'))
                    {
                        // Ambiguous path.
                        throw new InvalidOperationException("Unable to join paths.");
                    }

                    // Assume the drive from the front path.
                    parts = new List<string>() { frontParts[0] };
                    parts.AddRange(backParts);
                }
                else
                {
                    parts = backParts;
                }
            }
            else
            {
                // The back path is a relative path.
                parts = frontParts;

                parts.AddRange(backParts);
            }

            ApplySyntax(parts);

            var result = new StringBuilder();
            for (int i = 0; i < parts.Count; i++)
            {
                result.Append(parts[i]);
                if (parts.Count == 1 | i != parts.Count - 1)
                {
                    result.Append('\\');
                }
            }

            return PathSanitiser.RemoveDuplicateDirectorySeparators(result.ToString());
        }
    }
}
