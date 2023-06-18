using SphereOS.Logging;
using SphereOS.Shell;
using System.IO;
using Sys = Cosmos.System;

namespace SphereOS.Core
{
    internal static class FsManager
    {
        /// <summary>
        /// The virtual file system.
        /// </summary>
        internal static Sys.FileSystem.CosmosVFS Fs;

        private static void CreateIfNotExists(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                Logging.Log.Info($"FsManager", $"Directory '{dir}' was created.");
            }
        }

        /// <summary>
        /// Initialise the virtual file system.
        /// </summary>
        internal static bool Initialise()
        {
            Util.PrintSystem("Registering file system...");
            Fs = new Sys.FileSystem.CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(Fs);
            try
            {
                Directory.GetFiles(@"0:\");

                CreateIfNotExists(@"0:\etc");

                SysCfg.Load();

                if (SysCfg.BootLock)
                {
                    if (File.Exists(@"0:\etc\bootlock.tmp"))
                    {
                        Util.PrintWarning("SphereOS shut down unexpectedly last time.\nEnsure you use the 'shutdown' command when shutting down to avoid data loss.");
                        Log.Warning("FsManager", "SphereOS shut down unexpectedly last time.");
                    }
                    else
                    {
                        File.Create(@"0:\etc\bootlock.tmp");
                    }
                }

                return true;
            }
            catch (System.Exception e)
            {
                Util.PrintSystem($"Failed to initialise filesystem. Ensure you have a valid FAT32 filesystem on the disk. {e.ToString()}");
                Logging.Log.Error("FsManager", $"Failed to initialise filesystem: {e.ToString()}");
                return false;
            }
        }
    }
}
