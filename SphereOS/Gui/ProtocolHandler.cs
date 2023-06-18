using SphereOS.Core;
using SphereOS.Gui.Apps;
using SphereOS.Gui.SmoothMono;
using SphereOS.Gui.UILib;
using SphereOS.Logging;
using System;
using System.Drawing;
using System.IO;

namespace SphereOS.Gui
{
    internal static class ProtocolHandler
    {
        private static int padding = 12;

        private static void ShowRsAppChoiceDialogue(string path)
        {
            WindowManager wm = ProcessManager.GetProcess<WindowManager>();

            var window = new AppWindow(wm, 256, 256, 320, 192);
            window.Title = "Open File";

            window.Clear(Color.LightGray);
            window.DrawFilledRectangle(0, window.Height - (padding * 2) - 20, window.Width, (padding * 2) + 20, Color.Gray);
            window.DrawString($"Choose an app to open '{Path.GetFileName(path)}'.", Color.Black, padding, padding);

            wm.AddWindow(window);

            Table table = new Table(window, padding, padding + FontData.Height + padding, window.Width - (padding * 2), window.Height - (padding * 5) - 20 - FontData.Height);
            table.Cells.Add(new TableCell(AppManager.GetAppMetadata("Notepad").Icon.Resize(20, 20), "Notepad"));
            table.Cells.Add(new TableCell("RiverScript Interpreter"));
            table.CellHeight = 20;
            table.AllowDeselection = false;
            table.SelectedCellIndex = 0;
            table.Render();
            wm.AddWindow(table);

            Button open = new Button(window, window.Width - 80 - padding, window.Height - 20 - padding, 80, 20);
            open.Text = "Open";
            open.OnClick = (int x, int y) =>
            {
                wm.RemoveWindow(window);

                switch (table.SelectedCellIndex)
                {
                    case 0:
                        ProcessManager.AddProcess(new Notepad(path)).Start();
                        break;
                    case 1:
                        var script = new RiverScript.Script(File.ReadAllText(path));
                        script.Lex();
                        var dialogue = new RiverScriptUIConsole(wm, script, Path.GetFileName(path));
                        dialogue.Show();
                        break;
                    default:
                        throw new Exception("ProtocolHandler: Unrecognised option.");
                }
            };
            wm.AddWindow(open);

            wm.Update(window);
        }

        internal static void Open(string path)
        {
            WindowManager wm = ProcessManager.GetProcess<WindowManager>();

            if (!FileSecurity.CanAccess(Kernel.CurrentUser, path))
            {
                MessageBox messageBox = new MessageBox(wm, "Unauthorised", $"Access to '{Path.GetFileName(path)}' is unauthorised.");
                messageBox.Show();

                Log.Info("ProtocolHandler", $"{Kernel.CurrentUser.Username}: Unauthorised file blocked.");

                return;
            }

            switch (Path.GetExtension(path).ToLower())
            {
                case ".rs":
                    ShowRsAppChoiceDialogue(path);
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
