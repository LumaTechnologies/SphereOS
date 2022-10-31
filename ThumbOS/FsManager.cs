using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys = Cosmos.System;

namespace ThumbOS
{
    internal static class FsManager
    {
        /// <summary>
        /// The virtual file system.
        /// </summary>
        internal static Sys.FileSystem.CosmosVFS Fs;

        /// <summary>
        /// Initialise the virtual file system.
        /// </summary>
        internal static void Initialise()
        {
            Util.PrintTask("Initialising file system...");
            Fs = new Sys.FileSystem.CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(Fs);
        }
    }
}
