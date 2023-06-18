using Cosmos.HAL;
using SphereOS.ConsoleApps;
using SphereOS.Core;

namespace SphereOS.Commands.GeneralTopic
{
    internal class Pci : Command
    {
        public Pci() : base("pci")
        {
            Description = "Show a list of installed PCI devices.";

            Topic = "general";
        }

        internal override ReturnCode Execute(string[] args)
        {
            string text = string.Empty;

            foreach (PCIDevice device in Cosmos.HAL.PCI.Devices)
            {
                if (device.Claimed)
                {
                    text += "- PCI Device  (Driver Claimed) -\n";
                }
                else
                {
                    text += "---------- PCI Device ----------\n";
                }
                text += $"ID     - 0x{device.DeviceID.ToString("X")}\n";
                text += $"Vendor - 0x{device.VendorID.ToString("X")} ({PciUtils.GetVendorName(device.VendorID)})\n";
                text += "\n";
            }

            TextEditor textEditor = new(text, isPath: false);

            textEditor.Start();

            return ReturnCode.Success;
        }
    }
}
