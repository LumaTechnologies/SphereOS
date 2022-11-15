using System.Collections.Generic;
using System;
using static System.Collections.Specialized.BitVector32;

namespace SphereOS.Text
{
    internal class IniReader
    {
        internal IniReader(string Source)
        {
            this.Source = Source.Replace("\r", "");
            this.Lines = Source.Split('\n');
        }

        internal string Source { get; private set; }
        internal string[] Lines { get; private set; }

        internal string ReadString(string key, string? section = null)
        {
            string _section = string.Empty;
            foreach (var line in this.Lines)
            {
                int equalIndex = line.IndexOf('=');
                if (equalIndex == -1)
                {
                    string trimmed = line.Trim();
                    if (trimmed.StartsWith('[') && trimmed.EndsWith(']'))
                    {
                        _section = trimmed.Substring(1, trimmed.Length - 2);
                        continue;
                    }
                    else
                    {
                        if (line.Trim() == string.Empty)
                        {
                            continue;
                        }
                        else
                        {
                            throw new FormatException();
                        }
                    }
                }
                if (equalIndex < 1)
                {
                    throw new System.FormatException(line);
                }
                string _key = line.Substring(0, equalIndex).Trim();
                if (key == _key)
                {
                    if (section != null)
                    {
                        if (section != _section)
                        {
                            continue;
                        }
                    }
                    if (line.Length >= 3)
                    {
                        return line.Substring(equalIndex + 1).Trim();
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
            throw new KeyNotFoundException();
        }

        internal int ReadInt(string key, string? section = null)
        {
            string value = ReadString(key, section);
            if (int.TryParse(value, out int result))
            {
                return result;
            }
            else
            {
                throw new System.FormatException(value);
            }
        }

        internal bool ReadBool(string key, string? section = null)
        {
            string value = ReadString(key, section);
            if (bool.TryParse(value, out bool result))
            {
                return result;
            }
            else
            {
                throw new System.FormatException(value);
            }
        }

        internal long ReadLong(string key, string? section = null)
        {
            string value = ReadString(key, section);
            if (long.TryParse(value, out long result))
            {
                return result;
            }
            else
            {
                throw new System.FormatException(value);
            }
        }

        internal List<string> GetSections()
        {
            List<string> sections = new List<string>();
            foreach (var line in this.Lines)
            {
                if (line.IndexOf('=') == -1)
                {
                    string trimmed = line.Trim();
                    if (trimmed.StartsWith('[') && trimmed.EndsWith(']'))
                    {
                        sections.Add(trimmed.Substring(1, trimmed.Length - 2));
                    }
                }
            }
            return sections;
        }

        internal bool TryReadString(string key, out string value, string? section = null)
        {
            try
            {
                value = ReadString(key, section);
                return true;
            }
            catch
            {
                value = null;
                return false;
            }
        }
    }
}