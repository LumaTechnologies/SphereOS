using Cosmos.System;
using Cosmos.System.Graphics;
using SphereOS.Core;
using System;
using System.Drawing;

namespace SphereOS.Gui.UILib
{
    internal class AppWindow : Window
    {
        internal AppWindow(Process process, int x, int y, int width, int height) : base(process, x, y, width, height)
        {
            wm = ProcessManager.GetProcess<WindowManager>();

            decorationWindow = new Window(process, 0, -titlebarHeight, width, titlebarHeight);
            wm.AddWindow(decorationWindow);

            decorationWindow.RelativeTo = this;

            decorationWindow.OnClick = DecorationClicked;
            decorationWindow.OnDown = DecorationDown;

            RenderDecoration();
        }

        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.Close.bmp")]
        private static byte[] closeBytes;
        private static Bitmap closeBitmap = new Bitmap(closeBytes);

        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.Maximise.bmp")]
        private static byte[] maximiseBytes;
        private static Bitmap maximiseBitmap = new Bitmap(maximiseBytes);

        /*[IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.Minimise.bmp")]
        private static byte[] minimiseBytes;
        private static Bitmap minimiseBitmap = new Bitmap(minimiseBytes);*/

        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.Restore.bmp")]
        private static byte[] restoreBytes;
        private static Bitmap restoreBitmap = new Bitmap(restoreBytes);

        internal Action Closing;

        private string _title = "Window";
        internal string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                RenderDecoration();
            }
        }

        private bool _canResize = false;
        internal bool CanResize
        {
            get
            {
                return _canResize;
            }
            set
            {
                _canResize = value;
                RenderDecoration();
            }
        }

        private const int titlebarHeight = 24;

        private Window decorationWindow;

        private WindowManager wm;

        private bool maximised = false;
        private int originalX;
        private int originalY;
        private int originalWidth;
        private int originalHeight;

        private void DecorationClicked(int x, int y)
        {
            if (x >= Width - titlebarHeight)
            {
                // Close.
                Closing?.Invoke();
                wm.RemoveWindow(this);
            }
            else if (x >= Width - (titlebarHeight * 2) && _canResize)
            {
                // Maximise / restore.
                if (maximised)
                {
                    maximised = false;

                    MoveAndResize(originalX, originalY, originalWidth, originalHeight, sendWMEvent: false);

                    decorationWindow.Resize(originalWidth, titlebarHeight, sendWMEvent: false);

                    UserResized?.Invoke();
                    WM_RefreshAll?.Invoke();
                }
                else
                {
                    maximised = true;

                    var taskbar = ProcessManager.GetProcess<ShellComponents.Taskbar>();
                    int taskbarHeight = taskbar.GetTaskbarHeight();

                    originalX = X;
                    originalY = Y;
                    originalWidth = Width;
                    originalHeight = Height;

                    MoveAndResize(0, taskbarHeight + titlebarHeight, (int)wm.ScreenWidth, (int)(wm.ScreenHeight - taskbarHeight - titlebarHeight), sendWMEvent: false);

                    decorationWindow.Resize((int)wm.ScreenWidth, titlebarHeight, sendWMEvent: false);

                    UserResized?.Invoke();
                    WM_RefreshAll?.Invoke();
                }
                RenderDecoration();
            }
        }

        private void DecorationDown(int x, int y)
        {
            if (x >= Width - (titlebarHeight * (_canResize ? 2 : 1)) || maximised) return;

            uint startMouseX = MouseManager.X;
            uint startMouseY = MouseManager.Y;

            int startWindowX = X;
            int startWindowY = Y;

            while (MouseManager.MouseState == MouseState.Left)
            {
                X = (int)(startWindowX + (MouseManager.X - startMouseX));
                Y = (int)(startWindowY + (MouseManager.Y - startMouseY));

                ProcessManager.Yield();
            }
        }

        private void RenderDecoration()
        {
            decorationWindow.Clear(Color.DarkGray);
            decorationWindow.DrawString(Title, Color.White, 4, 4);
            decorationWindow.DrawImageAlpha(closeBitmap, Width - titlebarHeight, 0);
            if (CanResize)
            {
                decorationWindow.DrawImageAlpha(maximised ? restoreBitmap : maximiseBitmap, Width - (titlebarHeight * 2), 0);
            }
            wm.Update(decorationWindow);
        }
    }
}
