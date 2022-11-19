using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SphereOS.Core
{
    internal class MemService : Process
    {
        internal MemService() : base("MemService", ProcessType.Service)
        {
            Critical = true;
        }

        internal override void Run()
        {
            Cosmos.Core.Memory.Heap.Collect();
        }
    }
}
