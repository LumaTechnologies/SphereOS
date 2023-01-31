using IL2CPU.API.Attribs;
using System;
using static Cosmos.Core.INTs;

namespace SphereOS.Core.CorePlugs
{
    [Plug(Target = typeof(Cosmos.Core.INTs))]
    internal class Interrupt
    {
        public static void HandleException(uint eip, string desc, string name, ref IRQContext context, uint lastKnownAddressValue = 0)
        {
            CrashScreen.ShowCrashScreen(new Exception("Kernel received interrupt."));
        }
    }
}
