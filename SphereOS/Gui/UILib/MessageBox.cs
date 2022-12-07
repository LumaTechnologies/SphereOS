using SphereOS.Core;
using System;
using System.Drawing;

namespace SphereOS.Gui.UILib
{
    internal class MessageBox
    {
        internal MessageBox(Process process, string title, string message)
        {
            this.process = process;
            Title = title;
            Message = message;
        }

        private const int padding = 12;

        internal void Show()
        {
            WindowManager wm = ProcessManager.GetProcess<WindowManager>();

            int longestLineLength = 0;
            foreach (string line in Message.Split('\n'))
            {
                longestLineLength = Math.Max(longestLineLength, line.Length);
            }

            int width = Math.Max(192, (padding * 2) + (8 * longestLineLength));
            int height = 128 + ((Message.Split('\n').Length - 1) * 16);

            AppWindow window = new AppWindow(process, (int)((wm.ScreenWidth / 2) - (height / 2)), (int)((wm.ScreenWidth / 2) - (width / 2)), width, height);
            window.Title = Title;
            wm.AddWindow(window);

            window.Clear(Color.LightGray);
            window.DrawFilledRectangle(0, window.Height - (padding * 2) - 20, window.Width, (padding * 2) + 20, Color.Gray);
            window.DrawString(Message, Color.Black, padding, padding);

            Button ok = new Button(window, window.Width - 80 - padding, window.Height - 20 - padding, 80, 20);
            ok.Text = "OK";
            ok.OnClick = (int x, int y) =>
            {
                wm.RemoveWindow(window);
            };
            wm.AddWindow(ok);

            wm.Update(window);

            ProcessManager.GetProcess<Sound.SoundService>().PlaySystemSound(Sound.SystemSound.Alert);
        }

        private Process process;

        internal string Title { get; private set; }

        internal string Message { get; private set; }
    }
}
