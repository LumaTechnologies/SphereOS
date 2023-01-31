namespace SphereOS.Core
{
    internal class MemService : Process
    {
        internal MemService() : base("MemService", ProcessType.Service)
        {
            Critical = true;
        }

        int ticks = 0;

        internal override void Run()
        {
            if (ticks == 5)
            {
                ticks = 0;
                Cosmos.Core.Memory.Heap.Collect();
                ProcessManager.Sweep();
            }
            ticks++;
        }
    }
}
