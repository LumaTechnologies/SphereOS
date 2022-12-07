using SphereOS.Core;
using SphereOS.Gui.UILib;
using System.Drawing;
using Cosmos.System.Graphics;
using System.Security.Principal;

namespace SphereOS.Gui.ShellComponents
{
    internal class StartMenu : Process
    {
        internal StartMenu() : base("StartMenu", ProcessType.Application)
        {
        }

        internal static StartMenu CurrentStartMenu
        {
            get
            {
                StartMenu startMenu = ProcessManager.GetProcess<StartMenu>();
                if (startMenu == null && ProcessManager.GetProcess<Taskbar>() != null)
                {
                    startMenu = (StartMenu)ProcessManager.AddProcess(ProcessManager.GetProcess<WindowManager>(), new StartMenu());
                    startMenu.Start();
                }
                return startMenu;
            }
        }

        private static class Icons
        {
            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.StartMenu.User.bmp")]
            private static byte[] _iconBytes_User;
            internal static Bitmap Icon_User = new Bitmap(_iconBytes_User);
        }

        Window window;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        SettingsService settingsService = ProcessManager.GetProcess<SettingsService>();

        private Button shutdownButton;
        private Button rebootButton;
        /*private Button logoutButton;*/

        private const int buttonsPadding = 12;
        private const int buttonsWidth = 96;
        private const int buttonsHeight = 20;
        private const int userHeight = 56;
        private const int userPadding = 12;

        private bool isOpen = false;

        internal void ShowStartMenu()
        {
            isOpen = true;

            bool leftHandStartButton = settingsService.LeftHandStartButton;

            window = new Window(this, leftHandStartButton ? 0 : (int)(wm.ScreenWidth / 2 - 408 / 2), 24, 408, 320);

            window.Clear(Color.FromArgb(56, 56, 71));

            window.DrawString($"Welcome", Color.White, 12, 12);

            Rectangle userRect = new Rectangle(userPadding, window.Height - userHeight + userPadding, window.Width - (userPadding * 2), userHeight - (userPadding * 2));
            window.DrawImageAlpha(Icons.Icon_User, userRect.X, (int)(userRect.Y + (userRect.Height / 2) - (Icons.Icon_User.Height / 2)));
            window.DrawString(Kernel.CurrentUser.Username, Color.White, (int)(userRect.X + Icons.Icon_User.Width + userPadding), (int)(userRect.Y + (userRect.Height / 2) - (16 / 2)));

            wm.AddWindow(window);

            int x = 12;
            int y = 40;
            foreach (App app in AppManager.Apps)
            {
                Button appButton = new Button(window, x, y, 90, 90);
                appButton.Background = app.ThemeColor;
                appButton.Foreground = app.ThemeColor.GetForegroundColour();
                appButton.Text = app.Name;
                appButton.Image = app.Icon;
                appButton.OnClick = (x, y) =>
                {
                    app.Start(this);
                    HideStartMenu();
                };
                wm.AddWindow(appButton);
                x += appButton.Width + 8;
                if (x > window.Width - 90)
                {
                    x = 12;
                    y += 90 + 8;
                }
            }

            shutdownButton = new Button(window, buttonsPadding, window.Height - buttonsHeight - userHeight, buttonsWidth, buttonsHeight);
            shutdownButton.Text = "Shut down";
            shutdownButton.OnClick = ShutdownClicked;
            wm.AddWindow(shutdownButton);

            rebootButton = new Button(window, buttonsPadding * 2 + buttonsWidth, window.Height - buttonsHeight - userHeight, buttonsWidth, buttonsHeight);
            rebootButton.Text = "Restart";
            rebootButton.OnClick = RebootClicked;
            wm.AddWindow(rebootButton);

            /*logoutButton = new Button(window, buttonsPadding * 3 + buttonsWidth * 2, window.Height - buttonsHeight - buttonsPadding, buttonsWidth, buttonsHeight);
            logoutButton.Text = "Log Out";
            logoutButton.OnClick = LogoutClicked;
            wm.AddWindow(logoutButton);*/

            wm.Update(window);
        }

        private void ShutdownClicked(int x, int y)
        {
            Power.Shutdown(reboot: false);
        }

        private void RebootClicked(int x, int y)
        {
            Power.Shutdown(reboot: true);
        }

        /*private void LogoutClicked(int x, int y)
        {
            wm.Clear();

            Kernel.CurrentUser = null;

            ProcessManager.AddProcess(new ShellComponents.Lock()).Start();
        }*/

        internal void HideStartMenu()
        {
            isOpen = false;
            wm.RemoveWindow(window);
        }

        internal void ToggleStartMenu()
        {
            if (isOpen)
            {
                HideStartMenu();
            }
            else
            {
                ShowStartMenu();
            }
        }

        internal override void Start()
        {
            base.Start();
        }

        internal override void Run()
        {
        }
    }
}
