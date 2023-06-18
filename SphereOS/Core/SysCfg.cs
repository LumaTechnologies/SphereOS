using SphereOS.Logging;
using SphereOS.Text;
using System;
using System.IO;

namespace SphereOS.Core
{
    internal static class SysCfg
    {
        private static string path = @"0:\etc\sys.cfg";

        internal static string Name { get; set; } = "SphereOS";

        internal static DateTime InstallDate { get; set; } = DateTime.Now;

        internal static bool BootLock { get; set; } = true;

        internal static bool RsAdminOnly { get; set; } = false;

        internal static string InstallVer { get; set; } = Kernel.Version;

        internal static void Load()
        {
            if (!File.Exists(path))
            {
                Flush();
                return;
            }

            IniReader reader = new IniReader(File.ReadAllText(path));

            Name = reader.ReadString("Name", section: "SysMeta");
            InstallDate = new DateTime(reader.ReadLong("InstallDate", section: "SysMeta"));
            InstallVer = reader.ReadString("InstallVer", section: "SysMeta");

            BootLock = reader.ReadBool("BootLock", section: "Config");

            RsAdminOnly = reader.ReadBool("RsAdminOnly", section: "Security");

            Log.Info("SysCfg", "SysCfg loaded.");
        }

        internal static void Flush()
        {
            IniBuilder builder = new IniBuilder();

            builder.BeginSection("SysMeta");
            builder.AddKey("Name", Name);
            builder.AddKey("InstallDate", InstallDate.Ticks);
            builder.AddKey("InstallVer", InstallVer);

            builder.BeginSection("Canonical");
            builder.AddKey("Ver", Kernel.Version);

            builder.BeginSection("Config");
            builder.AddKey("BootLock", BootLock);

            builder.BeginSection("Security");
            builder.AddKey("RsAdminOnly", RsAdminOnly);

            File.WriteAllText(path, builder.ToString());

            Log.Info("SysCfg", "SysCfg flushed.");
        }
    }
}
