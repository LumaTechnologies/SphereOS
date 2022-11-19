using System.Collections.Generic;

namespace SphereOS.Core
{
    internal static class ProcessManager
    {
        internal static List<Process> Processes = new List<Process>();

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
            for (int i = Processes.Count - 1; i >= 0; i--)
            {
                if (!Processes[i].IsRunning)
                {
                    Processes.Remove(Processes[i]);
                }
            }
        }

        internal static void Yield()
        {
            for (int i = Processes.Count - 1; i >= 0; i--)
            {
                Process process = Processes[i];
                if (process.IsRunning)
                {
                    process.TryRun();
                }
            }
        }
    }
}
