namespace SphereOS.Core.Memory
{
    internal struct MemoryStatistics
    {
        internal MemoryStatistics(uint totalMB, uint unavailableMB, uint usedMB, uint freeMB, uint percentUsed)
        {
            TotalMB = totalMB;
            UnavailableMB = unavailableMB;
            UsedMB = usedMB;
            FreeMB = freeMB;
            PercentUsed = percentUsed;
        }

        internal uint TotalMB;
        internal uint UnavailableMB;
        internal uint UsedMB;
        internal uint FreeMB;
        internal uint PercentUsed;
    }
}
