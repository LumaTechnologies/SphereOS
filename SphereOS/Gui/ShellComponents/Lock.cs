using SphereOS.Core;
using SphereOS.Gui.UILib;
using System.Drawing;
using Cosmos.System.Graphics;
using SphereOS.Users;
using SphereOS.Shell;
using System;

namespace SphereOS.Gui.ShellComponents
{
    internal class Lock : Process
    {
        internal Lock() : base("Lock", ProcessType.Application) { }

        Window window;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        Sound.SoundService soundService = ProcessManager.GetProcess<Sound.SoundService>();

        private static class Icons
        {
            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.Lock.User.bmp")]
            private static byte[] _iconBytes_User;
            internal static Bitmap Icon_User = new Bitmap(_iconBytes_User);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.Lock.UserArrow.bmp")]
            private static byte[] _iconBytes_UserArrow;
            internal static Bitmap Icon_UserArrow = new Bitmap(_iconBytes_UserArrow);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.Lock.Gradient.bmp")]
            private static byte[] _iconBytes_Gradient;
            internal static Bitmap Icon_Gradient = new Bitmap(_iconBytes_Gradient);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.Lock.Background.bmp")]
            private static byte[] _iconBytes_Background;
            internal static Bitmap Icon_Background = new Bitmap(_iconBytes_Background);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.Lock.ShutDown.bmp")]
            private static byte[] _iconBytes_ShutDown;
            internal static Bitmap Icon_ShutDown = new Bitmap(_iconBytes_ShutDown);
        }

        private Color borderColor = Color.FromArgb(0, 70, 138);
        private int borderHeight = 128;

        private int gradientHeight = 4;

        private User user = UserManager.Users[0];

        private void RenderBackground(string errorMessage = null)
        {
            window.Clear(Color.CornflowerBlue);

            window.DrawImage(Icons.Icon_Background, 0, borderHeight + gradientHeight);

            window.DrawFilledRectangle(0, 0, window.Width, borderHeight, borderColor);
            window.DrawFilledRectangle(0, window.Height - borderHeight, window.Width, borderHeight, borderColor);

            Bitmap gradientResized = Icons.Icon_Gradient.Resize((uint)window.Width, (uint)gradientHeight);
            window.DrawImage(gradientResized, 0, borderHeight);
            window.DrawImage(gradientResized, 0, window.Height - borderHeight - gradientHeight);

            window.DrawImageAlpha(Icons.Icon_User, (int)(window.Width / 2 - Icons.Icon_User.Width / 2) + (window.Width / 4), (int)(window.Height / 2 - Icons.Icon_User.Height / 2));
            window.DrawString(user.Username, Color.White, (window.Width / 2 - user.Username.Length * 8 / 2) + (window.Width / 4), (int)(window.Height / 2 + Icons.Icon_User.Height / 2 + 12));
            
            if (errorMessage != null)
            {
                window.DrawString(errorMessage, Color.FromArgb(255, 209, 243), (window.Width / 2 - errorMessage.Length * 8 / 2) + (window.Width / 4), (int)(window.Height / 2 + Icons.Icon_User.Height / 2 + 96));
            }
        }

        #region Process
        internal override void Start()
        {
            base.Start();
            window = new Window(this, 0, 0, (int)wm.ScreenWidth, (int)wm.ScreenHeight);
            wm.AddWindow(window);

            RenderBackground();

            TextBox passwordBox = new TextBox(window, (window.Width / 4) + (window.Width / 2) - (128 / 2), (int)(window.Height / 2 + Icons.Icon_User.Height / 2 + 48), 128, 20);
            passwordBox.Shield = true;
            passwordBox.Submitted = () =>
            {
                if (user.Authenticate(passwordBox.Text))
                {
                    TryStop();
                    Kernel.CurrentUser = user;
                    ProcessManager.AddProcess(wm, new Gui.ShellComponents.Taskbar()).Start();
                    soundService.PlaySystemSound(Sound.SystemSound.Login);
                }
                else
                {
                    passwordBox.Text = string.Empty;
                    if (user.LockedOut)
                    {
                        TimeSpan remaining = user.LockoutEnd - DateTime.Now;
                        if (remaining.Minutes > 0)
                        {
                            RenderBackground(errorMessage: $"Try again in {remaining.Minutes}m, {remaining.Seconds}s.");
                        }
                        else
                        {
                            RenderBackground(errorMessage: $"Try again in {remaining.Seconds}s.");
                        }
                    }
                    else
                    {
                        RenderBackground(errorMessage: "Incorrect password.");
                    }
                    wm.Update(window);

                    soundService.PlaySystemSound(Sound.SystemSound.Alert);
                }
            };
            wm.AddWindow(passwordBox);

            Table usersTable = new Table(window, (int)((Icons.Icon_Background.Width / 2) - (160 / 2)), borderHeight + 256, 160, 28 * UserManager.Users.Count);
            usersTable.Background = Color.CornflowerBlue;
            usersTable.Border = Color.CornflowerBlue;
            usersTable.Foreground = Color.Black;
            usersTable.CellHeight = 28;
            usersTable.SelectedBackground = Color.FromArgb(127, 174, 255);
            usersTable.SelectedBorder = Color.White;
            foreach (User user in UserManager.Users)
            {
                usersTable.Cells.Add(new TableCell(Icons.Icon_UserArrow, user.Username));
            }
            usersTable.TableCellSelected = (int index) =>
            {
                user = UserManager.Users[usersTable.SelectedCellIndex];
                RenderBackground();
                passwordBox.Text = string.Empty;
                wm.Update(window);
            };
            usersTable.SelectedCellIndex = 0;
            usersTable.AllowDeselection = false;
            usersTable.Render();
            wm.AddWindow(usersTable);

            Button shutdownButton = new Button(window, 48, window.Height - (borderHeight / 2) - (48 / 2), 160, 32);
            shutdownButton.ImageLocation = Button.ButtonImageLocation.Left;
            shutdownButton.Image = Icons.Icon_ShutDown;
            shutdownButton.Background = borderColor;
            shutdownButton.Border = borderColor;
            shutdownButton.Text = "Shut down";
            shutdownButton.Foreground = Color.White;
            shutdownButton.OnClick = (int x, int y) =>
            {
                Power.Shutdown(reboot: false);
            };
            wm.AddWindow(shutdownButton);

            wm.Update(window);
        }

        internal override void Run()
        {

        }

        internal override void Stop()
        {
            base.Stop();
            wm.RemoveWindow(window);
        }
        #endregion
    }
}
