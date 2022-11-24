using SphereOS.Core;
using SphereOS.Gui.UILib;
using System.Drawing;

namespace SphereOS.Gui.ShellComponents
{
    internal class StartMenu : Process
    {
        internal StartMenu() : base("StartMenu", ProcessType.Application) { }

        internal static StartMenu CurrentStartMenu
        {
            get
            {
                StartMenu startMenu = ProcessManager.GetProcess<StartMenu>();
                if (startMenu == null)
                {
                    startMenu = (StartMenu)ProcessManager.AddProcess(ProcessManager.GetProcess<WindowManager>(), new StartMenu());
                    startMenu.Start();
                }
                return startMenu;
            }
        }

        Window window;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        SettingsService settingsService = ProcessManager.GetProcess<SettingsService>();

        private Button shutdownButton;
        private Button rebootButton;
        private Button exitButton;

        private const int buttonsPadding = 12;
        private const int buttonsWidth = 96;
        private const int buttonsHeight = 20;

        private bool isOpen = false;

        internal void ShowStartMenu()
        {
            isOpen = true;

            bool leftHandStartButton = settingsService.LeftHandStartButton;

            window = new Window(this, leftHandStartButton ? 0 : (int)(wm.ScreenWidth / 2 - 408 / 2), 24, 408, 272);
            window.Clear(Color.FromArgb(56, 56, 71));
            window.DrawString($"Welcome", Color.White, 12, 12);
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

            shutdownButton = new Button(window, buttonsPadding, window.Height - buttonsHeight - buttonsPadding, buttonsWidth, buttonsHeight);
            shutdownButton.Text = "Shut down";
            shutdownButton.OnClick = ShutdownClicked;
            wm.AddWindow(shutdownButton);

            rebootButton = new Button(window, buttonsPadding * 2 + buttonsWidth, window.Height - buttonsHeight - buttonsPadding, buttonsWidth, buttonsHeight);
            rebootButton.Text = "Reboot";
            rebootButton.OnClick = RebootClicked;
            wm.AddWindow(rebootButton);

            /*exitButton = new Button(window, buttonsPadding * 3 + buttonsWidth * 2, window.Height - buttonsHeight - buttonsPadding, buttonsWidth, buttonsHeight);
            exitButton.Text = "Exit";
            exitButton.OnClick = ExitClicked;
            wm.AddWindow(exitButton);*/

            wm.Update(window);
        }

        private void ShutdownClicked(int x, int y)
        {
            wm.TryStop();
            Power.Shutdown(reboot: false);
        }

        private void RebootClicked(int x, int y)
        {
            wm.TryStop();
            Power.Shutdown(reboot: true);
        }

        /*private void ExitClicked(int x, int y)
        {
            ProcessManager.AddProcess(new Shell.Shell()).Start();
            ProcessManager.GetProcess<WindowManager>().Stop();
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
