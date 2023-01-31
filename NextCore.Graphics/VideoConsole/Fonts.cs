#pragma warning disable CS8618

using Cosmos.System.Graphics.Fonts;
using IL2CPU.API.Attribs;

namespace NextCore.Graphics.VideoConsole
{
    internal static class Fonts
    {
        [ManifestResourceStream(ResourceName = "NextCore.Graphics.Resources.zap-light16.psf")]
        private static byte[] _videoConsoleFontBytes;

        internal static PCScreenFont VideoConsoleFont = PCScreenFont.LoadFont(_videoConsoleFontBytes);
    }
}
