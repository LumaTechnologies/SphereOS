using SphereOS.Core;
using SphereOS.Gui.SmoothMono;
using SphereOS.Gui.UILib;
using SphereOS.Logging;
using System;
using System.Drawing;
using Cosmos.System.Graphics;
using System.Text;

namespace SphereOS.Gui.Apps
{
    internal class Logs : Process
    {
        internal Logs() : base("Logs", ProcessType.Application) { }

        AppWindow window;

        Table table;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        private static class Icons
        {
            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.Logs.Info.bmp")]
            private static byte[] _iconBytes_Info;
            internal static Bitmap Icon_Info = new Bitmap(_iconBytes_Info);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.Logs.Warning.bmp")]
            private static byte[] _iconBytes_Warning;
            internal static Bitmap Icon_Warning = new Bitmap(_iconBytes_Warning);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.Logs.Error.bmp")]
            private static byte[] _iconBytes_Error;
            internal static Bitmap Icon_Error = new Bitmap(_iconBytes_Error);
        }

        private void AddLogToTable(LogEvent log)
        {
            Bitmap icon;
            switch (log.Priority)
            {
                case LogPriority.Info:
                    icon = Icons.Icon_Info;
                    break;
                case LogPriority.Warning:
                    icon = Icons.Icon_Warning;
                    break;
                case LogPriority.Error:
                    icon = Icons.Icon_Error;
                    break;
                default:
                    icon = null;
                    break;
            }
            table.Cells.Add(new TableCell(icon, $"{log.Date.ToString("HH:mm")} - {log.Source}: {log.Message}", log));
        }

        private void Update()
        {
            window.Clear(Color.Gray);

            string text = $"{Log.Logs.Count} messages";
            window.DrawString(text, Color.White, window.Width - (FontData.Width * text.Length) - 12, window.Height - FontData.Height - 12);

            table.Render();
            table.ScrollToBottom();

            wm.Update(window);
        }

        internal override void Start()
        {
            base.Start();

            if (Kernel.CurrentUser == null || !Kernel.CurrentUser.Admin)
            {
                MessageBox messageBox = new MessageBox(Parent, Name, "You must be an admin to run this app.");
                messageBox.Show();

                TryStop();
                return;
            }

            window = new AppWindow(this, 256, 256, 512, 352);
            wm.AddWindow(window);
            window.Title = "Event Log";
            window.Closing = TryStop;

            table = new Table(window, 12, 12, window.Width - 24, window.Height - 24 - 16 - 12);
            table.OnDoubleClick = (int x, int y) =>
            {
                if (table.SelectedCellIndex != -1)
                {
                    LogEvent log = (LogEvent)table.Cells[table.SelectedCellIndex].Tag;

                    string priority = log.Priority switch {
                        LogPriority.Info => "Info",
                        LogPriority.Warning => "Warning",
                        LogPriority.Error => "Error",
                        _ => "Unknown"
                    };

                    StringBuilder builder = new StringBuilder();

                    builder.AppendLine($"Date: {log.Date.ToLongDateString()} {log.Date.ToLongTimeString()}");
                    builder.AppendLine($"Source: {log.Source}");
                    builder.AppendLine($"Priority: {priority}");
                    builder.AppendLine();
                    builder.Append(log.Message);

                    MessageBox messageBox = new MessageBox(this, $"{log.Source}: Log Event", builder.ToString());
                    messageBox.Show();
                }
            };
            wm.AddWindow(table);

            table.Cells.Clear();
            foreach (LogEvent log in Log.Logs)
            {
                AddLogToTable(log);
            }

            Log.LogEmittedReceivers.Add((LogEvent log) =>
            {
                AddLogToTable(log);
                Update();
            });

            Update();
        }

        internal override void Run()
        {

        }
    }
}
