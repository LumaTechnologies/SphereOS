
using Cosmos.System.Graphics;
using SphereOS.Core;
using SphereOS.Gui.UILib;
using SphereOS.Shell;
using System;
using System.Drawing;

namespace SphereOS.Gui.ShellComponents
{
    internal class StartMenu : Process
    {
        internal StartMenu() : base("StartMenu", ProcessType.Application) { }

        Window window;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

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

            window = new Window(this, (int)(wm.ScreenWidth / 2 - 384 / 2), 24, 384, 256);
            window.Clear(Color.DarkGray);
            window.DrawString($"Welcome", Color.Black, 12, 12);
            wm.AddWindow(window);

            int x = 12;
            int y = 40;
            foreach (App app in AppManager.Apps)
            {
                Button appButton = new Button(window, x, y, 80, 80);
                appButton.Background = Color.LightGray;
                appButton.Foreground = Color.White;
                appButton.Text = app.Name;
                appButton.Image = app.Icon;
                appButton.OnClick = (x, y) =>
                {
                    app.Start(this);
                    HideStartMenu();
                };
                wm.AddWindow(appButton);
                x += appButton.Width + 8;
                if (x >= window.Width - 80 - 12)
                {
                    x = 12;
                    y += 80 + 8;
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

            exitButton = new Button(window, buttonsPadding * 3 + buttonsWidth * 2, window.Height - buttonsHeight - buttonsPadding, buttonsWidth, buttonsHeight);
            exitButton.Text = "Exit";
            exitButton.OnClick = ExitClicked;
            wm.AddWindow(exitButton);

            wm.Update(window);
        }

        internal void ShutdownClicked(int x, int y)
        {
            Power.Shutdown(reboot: false);
        }

        internal void RebootClicked(int x, int y)
        {
            Power.Shutdown(reboot: true);
        }

        internal void ExitClicked(int x, int y)
        {
            ProcessManager.GetProcess<WindowManager>().Stop();

            ProcessManager.Sweep();

            ProcessManager.AddProcess(new Shell.Shell()).Start();
        }

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
