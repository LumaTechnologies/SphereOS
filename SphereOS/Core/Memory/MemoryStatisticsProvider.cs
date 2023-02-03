using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SphereOS.Core.Memory
{
    internal static class MemoryStatisticsProvider
    {
        internal static MemoryStatistics GetMemoryStatistics()
        {
            uint memTotal = Cosmos.Core.CPU.GetAmountOfRAM();
            uint memUnavail = memTotal - (uint)Cosmos.Core.GCImplementation.GetAvailableRAM();
            uint memUsed = (Cosmos.Core.GCImplementation.GetUsedRAM() / 1024 / 1024) + memUnavail;
            uint memFree = memTotal - memUsed;
            uint memPercentUsed = (uint)((float)memUsed / memTotal * 100f);

            return new MemoryStatistics(
                memTotal,
                memUnavail,
                memUsed,
                memFree,
                memPercentUsed
            );
        }
    }
}
