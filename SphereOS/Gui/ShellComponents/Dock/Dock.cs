using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using SphereOS.Core;
using SphereOS.Gui.SmoothMono;
using SphereOS.Gui.UILib;
using SphereOS.UILib.Animations;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace SphereOS.Gui.ShellComponents.Dock
{
    internal class Dock : Process
    {
        internal Dock() : base("Dock", ProcessType.Application)
        {
        }

        internal static Dock CurrentDock
        {
            get
            {
                Dock dock = ProcessManager.GetProcess<Dock>();
                return dock;
            }
        }

        Window window;

        List<BaseDockIcon> Icons = new List<BaseDockIcon>();

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        SettingsService settingsService = ProcessManager.GetProcess<SettingsService>();

        internal static readonly int IconSize = 64;
        internal static readonly int IconImageMaxSize = 48;

        private void Render()
        {
            int newDockWidth = 0;
            foreach (var icon in Icons)
            {
                newDockWidth += (int)icon.Size;
            }

            if (newDockWidth != window.Width)
            {
                window.MoveAndResize((int)(wm.ScreenWidth / 2 - newDockWidth / 2), window.Y, newDockWidth, window.Height);
            }

            window.Clear(Color.FromArgb(130, 202, 255));

            int x = 0;
            foreach (var icon in Icons)
            {
                if (icon.Image != null)
                {
                    Bitmap resizedImage = icon.Image.ResizeWidthKeepRatio((uint)Math.Min(IconImageMaxSize, icon.Size));

                    int imageX = (int)(x + ((icon.Size / 2) - (resizedImage.Width / 2)));
                    window.DrawImageAlpha(resizedImage, imageX, (int)((window.Height / 2) - (resizedImage.Height / 2)));
                }

                x += (int)icon.Size;
            }

            wm.Update(window);
        }

        internal int GetDockHeight()
        {
            return window.Height;
        }

        internal void UpdateWindows()
        {
            // Add new windows and update icons.
            foreach (var window in wm.Windows)
            {
                if (window is not AppWindow appWindow)
                {
                    continue;
                }

                bool found = false;

                foreach (BaseDockIcon icon in Icons)
                {
                    if (icon is AppDockIcon appDockIcon && appDockIcon.AppWindow == appWindow)
                    {
                        icon.Image = appWindow.Icon;

                        found = true;
                    }
                }

                if (!found)
                {
                    Icons.Add(new AppDockIcon(appWindow));
                }
            }

            // Remove deleted windows.
            foreach (BaseDockIcon icon in Icons)
            {
                if (icon is not AppDockIcon appDockIcon)
                {
                    continue;
                }

                bool found = false;

                foreach (Window window in wm.Windows)
                {
                    if (window == appDockIcon.AppWindow)
                    {
                        found = true;
                    }
                }

                if (!found)
                {
                    icon.Closing = true;
                }
            }

            Render();
        }

        private void DockClick(int x, int y)
        {
            int end = 0;
            foreach (var icon in Icons)
            {
                end += (int)icon.Size;

                if (x < end)
                {
                    icon.Clicked();
                    return;
                }
            }
        }

        internal override void Start()
        {
            base.Start();

            window = new Window(this, (int)(wm.ScreenWidth / 2), (int)(wm.ScreenHeight + IconSize), IconSize, IconSize);
            window.OnClick = DockClick;
            wm.AddWindow(window);

            Icons.Add(new StartMenuDockIcon());

            Render();

            MovementAnimation animation = new MovementAnimation(window)
            {
                From = new Rectangle(window.X, window.Y, window.Width, window.Height),
                To = new Rectangle(window.X, (int)(wm.ScreenHeight - IconSize), window.Width, window.Height),
                Duration = 10
            };
            animation.Start();
        }

        internal override void Run()
        {
            bool rerenderNeeded = false;

            for (int i = Icons.Count - 1; i >= 0; i--)
            {
                BaseDockIcon icon = Icons[i];

                if (icon.RunAnimation())
                {
                    rerenderNeeded = true;
                }

                if (icon.CloseAnimationComplete)
                {
                    Icons.Remove(icon);
                }
            }

            if (rerenderNeeded)
            {
                Render();
            }
        }
    }
}
