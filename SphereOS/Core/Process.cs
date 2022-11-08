using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SphereOS.Users;

namespace SphereOS.Core
{
    internal abstract class Process
    {
        internal Process(string name, ProcessType type)
        {
            Name = name;
            Type = type;
        }

        internal ulong Id { get; set; }

        internal List<string> Args { get; private set; }

        internal string Name { get; set; }

        internal ProcessType Type { get; private set; }

        internal DateTime Created { get; private set; } = DateTime.Now;

        internal bool IsRunning { get; private set; } = false;

        internal bool Swept { get; set; } = false;

        internal User User { get; set; }

        internal virtual void Start()
        {
            IsRunning = true;
        }

        internal abstract void Run();

        internal virtual void Stop()
        {
            IsRunning = false;
        }
    }
}
