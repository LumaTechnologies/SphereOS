using Cosmos.System.Graphics;
using SphereOS.Core;
using System;

namespace SphereOS.Gui
{
    internal class App
    {
        public App(string name, Func<Process> createProcess, Bitmap icon)
        {
            Name = name;
            CreateProcess = createProcess;
            Icon = icon;
        }

        internal void Start(Process parent)
        {
            ProcessManager.AddProcess(parent, CreateProcess()).Start();
        }

        internal void Start()
        {
            ProcessManager.AddProcess(CreateProcess()).Start();
        }

        internal string Name { get; private set; }

        internal Func<Process> CreateProcess { get; private set; }

        internal Bitmap Icon { get; private set; }
    }
}
