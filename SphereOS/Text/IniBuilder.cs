using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SphereOS.Text
{
    internal class IniBuilder
    {
        internal string Source { get; private set; } = string.Empty;

        internal void BeginSection(string section)
        {
            Source += $"[{section.Trim()}]\n";
        }

        internal void AddKey(string key, object value)
        {
            Source += $"{key.Trim()}={value.ToString()!.Trim()}\n";
        }

        public override string ToString()
        {
            return Source;
        }
    }
}
