using SphereOS.Core;
using SphereOS.Gui.Apps;
using SphereOS.Gui.UILib;
using SphereOS.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SphereOS.Gui
{
    internal static class ProtocolHandler
    {
        internal static void Open(string path)
        {
            WindowManager wm = ProcessManager.GetProcess<WindowManager>();

            if (!FileSecurity.CanAccess(Kernel.CurrentUser, path))
            {
                MessageBox messageBox = new MessageBox(wm, "Unauthorised", $"Access to '{Path.GetFileName(path)}' is unauthorised.");
                messageBox.Show();

                Log.Info("FileOpener", $"{Kernel.CurrentUser.Username}: Unauthorised file blocked.");

                return;
            }

            switch (Path.GetExtension(path).ToLower())
            {
                case "rs":
                    var script = new RiverScript.Script(File.ReadAllText(path));
                    var dialogue = new RsDialogue(wm, script, Path.GetFileName(path));
                    dialogue.Show();
                    break;
                case ".txt" or ".ini" or ".cfg":
                    ProcessManager.AddProcess(new Notepad(path)).Start();
                    break;
                default:
                    MessageBox messageBox = new MessageBox(wm, "Cannot Open File", "SphereOS cannot open this type of file.");
                    messageBox.Show();
                    break;
            }
        }
    }
}
