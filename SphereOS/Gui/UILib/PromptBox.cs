using SphereOS.Core;
using SphereOS.Gui.SmoothMono;
using System;
using System.Drawing;

namespace SphereOS.Gui.UILib
{
    internal class PromptBox : MessageBox
    {
        internal PromptBox(Process process, string title, string message, string placeholder, Action<string> submitted) : base(process, title, message)
        {
            Placeholder = placeholder;
            Submitted = submitted;
        }

        internal void Show()
        {
            WindowManager wm = ProcessManager.GetProcess<WindowManager>();

            int longestLineLength = 0;
            foreach (string line in Message.Split('\n'))
            {
                longestLineLength = Math.Max(longestLineLength, line.Length);
            }

            int width = Math.Max(256, (Padding * 2) + (8 * longestLineLength));
            int height = 128 + ((Message.Split('\n').Length - 1) * 16);

            AppWindow window = new AppWindow(Process, (int)((wm.ScreenWidth / 2) - (height / 2)), (int)((wm.ScreenWidth / 2) - (width / 2)), width, height);
            window.Title = Title;
            wm.AddWindow(window);

            window.Clear(Color.LightGray);
            window.DrawFilledRectangle(0, window.Height - (Padding * 2) - 20, window.Width, (Padding * 2) + 20, Color.Gray);
            window.DrawString(Message, Color.Black, Padding, Padding);

            TextBox textBox = new TextBox(window, Padding, Padding + FontData.Height + 8, 192, 20);
            textBox.PlaceholderText = Placeholder;
            wm.AddWindow(textBox);

            Button ok = new Button(window, window.Width - 80 - Padding, window.Height - 20 - Padding, 80, 20);
            ok.Text = "OK";
            ok.OnClick = (int x, int y) =>
            {
                wm.RemoveWindow(window);

                Submitted.Invoke(textBox.Text);
            };
            wm.AddWindow(ok);

            wm.Update(window);

            ProcessManager.GetProcess<Sound.SoundService>().PlaySystemSound(Sound.SystemSound.Alert);
        }

        internal Action<string> Submitted { get; private set; }

        internal string Placeholder { get; private set; }
    }
}
