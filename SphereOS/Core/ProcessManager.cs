using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SphereOS.Core
{
    internal static class ProcessManager
    {
        private static List<Process> Processes { get; } = new List<Process>();

        private static ulong nextProcessId = 0;

        internal static Process AddProcess(Process process)
        {
            if (Processes.Contains(process))
                return process;

            process.Id = nextProcessId;
            nextProcessId++;

            Processes.Add(process);

            return process;
        }

        internal static void Sweep()
        {
            foreach (var process in Processes)
            {
                if (!process.IsRunning)
                {
                    Processes.Remove(process);
                }
            }
        }

        internal static void Run()
        {
            for (int i = 0; i < Processes.Count; i++)
            {
                Process process = Processes[i];
                if (process.IsRunning)
                {
                    process.Run();
                }
            }
        }
    }
}
