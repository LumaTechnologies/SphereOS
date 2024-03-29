﻿using SphereOS.Core;
using System;
using System.Drawing;

namespace SphereOS.Gui.UILib
{
    internal class MessageBox
    {
        internal MessageBox(Process process, string title, string message)
        {
            this.Process = process;
            Title = title;
            Message = message;
        }

        protected const int Padding = 12;

        internal void Show()
        {
            WindowManager wm = ProcessManager.GetProcess<WindowManager>();

            int longestLineLength = 0;
            foreach (string line in Message.Split('\n'))
            {
                longestLineLength = Math.Max(longestLineLength, line.Length);
            }

            int width = Math.Max(192, (Padding * 2) + (8 * longestLineLength));
            int height = 128 + ((Message.Split('\n').Length - 1) * 16);

            AppWindow window = new AppWindow(Process, (int)((wm.ScreenWidth / 2) - (height / 2)), (int)((wm.ScreenWidth / 2) - (width / 2)), width, height);
            window.Title = Title;
            wm.AddWindow(window);

            window.Clear(Color.LightGray);
            window.DrawFilledRectangle(0, window.Height - (Padding * 2) - 20, window.Width, (Padding * 2) + 20, Color.Gray);
            window.DrawString(Message, Color.Black, Padding, Padding);

            Button ok = new Button(window, window.Width - 80 - Padding, window.Height - 20 - Padding, 80, 20);
            ok.Text = "OK";
            ok.OnClick = (int x, int y) =>
            {
                wm.RemoveWindow(window);
            };
            wm.AddWindow(ok);

            wm.Update(window);

            ProcessManager.GetProcess<Sound.SoundService>().PlaySystemSound(Sound.SystemSound.Alert);
        }

        internal Process Process { get; private set; }

        internal string Title { get; private set; }

        internal string Message { get; private set; }
    }
}
