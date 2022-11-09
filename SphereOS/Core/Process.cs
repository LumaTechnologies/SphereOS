using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SphereOS.Logging;
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

        internal Process(string name, ProcessType type, Process parent)
        {
            Name = name;
            Type = type;
            Parent = parent;
        }

        internal ulong Id { get; set; }

        internal List<string> Args { get; private set; }

        internal string Name { get; set; }

        internal ProcessType Type { get; private set; }

        internal DateTime Created { get; private set; } = DateTime.Now;

        internal bool IsRunning { get; private set; } = false;

        internal bool Swept { get; set; } = false;

        internal bool Critical { get; set; } = false;

        internal Process Parent { get; set; }

        internal virtual void Start()
        {
            IsRunning = true;
        }

        internal abstract void Run();

        internal void TryRun()
        {
            try
            {
                Run();
            }
            catch (Exception e)
            {
                IsRunning = false;
                Log.Error(Name, $"Process \"{Name}\" ({Id}) crashed: {e.ToString()}");
                if (Critical)
                {
                    throw new Exception("Critical process crashed.", e);
                }
            }
        }

        internal virtual void Stop()
        {
            IsRunning = false;
        }
    }
}
