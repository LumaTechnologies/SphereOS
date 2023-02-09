using Cosmos.System.Graphics;
using SphereOS.Core;
using SphereOS.Gui.UILib;
using SphereOS.Logging;
using SphereOS.Users;
using System;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace SphereOS.Gui.ShellComponents
{
    internal class Lock : Process
    {
        internal Lock() : base("Lock", ProcessType.Application) { }

        AppWindow window;

        TextBox usernameBox;

        TextBox passwordBox;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        Sound.SoundService soundService = ProcessManager.GetProcess<Sound.SoundService>();

        private static class Images
        {
            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.Lock.Key.bmp")]
            private static byte[] _iconBytes_Key;
            internal static Bitmap Icon_Key = new Bitmap(_iconBytes_Key);
        }

        private const int width = 352;
        private const int height = 128;
        private const int padding = 12;

        private double shakiness = 0;

        private void RenderBackground()
        {
            window.Clear(Color.LightGray);

            window.DrawImageAlpha(Images.Icon_Key, padding, padding);

            window.DrawString("Enter your username and password,\nthen press Enter to log on", Color.Black, (int)(padding + Images.Icon_Key.Width + padding), padding);
        }

        private void ShowError(string text)
        {
            MessageBox messageBox = new MessageBox(this, "Logon Failed", text);
            messageBox.Show();
        }

        private void Shake()
        {
            shakiness = 24;
        }

        private void LogOn()
        {
            if (usernameBox.Text.Trim() == string.Empty || passwordBox.Text.Trim() == string.Empty)
            {
                return;
            }

            User user = UserManager.GetUser(usernameBox.Text.Trim());

            if (user == null)
            {
                Shake();

                return;
            }

            if (user.Authenticate(passwordBox.Text.Trim()))
            {
                TryStop();
                Kernel.CurrentUser = user;
                ProcessManager.AddProcess(wm, new ShellComponents.Taskbar()).Start();
                ProcessManager.AddProcess(wm, new ShellComponents.Dock.Dock()).Start();
                soundService.PlaySystemSound(Sound.SystemSound.Login);

                Log.Info("Lock", $"{user.Username} logged on to the GUI.");
            }
            else
            {
                passwordBox.Text = string.Empty;

                if (user.LockedOut)
                {
                    TimeSpan remaining = user.LockoutEnd - DateTime.Now;
                    if (remaining.Minutes > 0)
                    {
                        ShowError($"Try again in {remaining.Minutes}m, {remaining.Seconds}s.");
                    }
                    else
                    {
                        ShowError($"Try again in {remaining.Seconds}s.");
                    }
                }
                wm.Update(window);

                soundService.PlaySystemSound(Sound.SystemSound.Alert);
                Shake();
            }
        }

        private void LogOnClick(int x, int y)
        {
            LogOn();
        }

        #region Process
        internal override void Start()
        {
            base.Start();
            window = new AppWindow(this, (int)(wm.ScreenWidth / 2 - width / 2), (int)(wm.ScreenHeight / 2 - height / 2), width, height);;
            window.Title = "SphereOS Logon";
            window.Icon = Images.Icon_Key;
            window.CanMove = false;
            window.CanClose = false;
            wm.AddWindow(window);

            RenderBackground();

            int boxesStartY = (int)(padding + Images.Icon_Key.Height + padding);

            usernameBox = new TextBox(window, padding, boxesStartY, 160, 20);
            usernameBox.PlaceholderText = "Username";
            usernameBox.Submitted = LogOn;
            wm.AddWindow(usernameBox);

            passwordBox = new TextBox(window, padding, boxesStartY + padding + 20, 160, 20);
            passwordBox.Shield = true;
            passwordBox.PlaceholderText = "Password";
            passwordBox.Submitted = LogOn;
            wm.AddWindow(passwordBox);

            wm.Update(window);
        }

        internal override void Run()
        {
            int oldX = window.X;
            int newX = (int)((wm.ScreenWidth / 2) - (width / 2) + (Math.Sin(shakiness) * 8));
            if (oldX != newX)
            {
                window.Move(newX, window.Y);
            }
            shakiness /= 1.1;
        }

        internal override void Stop()
        {
            base.Stop();
            wm.RemoveWindow(window);
        }
        #endregion
    }
}
