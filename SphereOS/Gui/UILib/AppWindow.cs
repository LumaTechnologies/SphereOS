using Cosmos.System;
using Cosmos.System.Graphics;
using SphereOS.Core;
using SphereOS.Gui.ShellComponents.Dock;
using SphereOS.Gui.SmoothMono;
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

            Icon = defaultAppIconBitmap;

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

        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.AppIcons.Default.bmp")]
        private static byte[] defaultAppIconBytes;
        private static Bitmap defaultAppIconBitmap = new Bitmap(defaultAppIconBytes);

        internal Action Closing;

        private Bitmap _icon;
        private Bitmap _smallIcon;
        internal Bitmap Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;
                _smallIcon = _icon.Resize(20, 20);

                RenderDecoration();

                ProcessManager.GetProcess<Dock>()?.UpdateWindows();
            }
        }

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

        private bool _canClose = true;
        internal bool CanClose
        {
            get
            {
                return _canClose;
            }
            set
            {
                _canClose = value;
                RenderDecoration();
            }
        }

        private bool _canMove = true;
        internal bool CanMove
        {
            get
            {
                return _canMove;
            }
            set
            {
                _canMove = value;
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
            if (x >= Width - titlebarHeight && _canClose)
            {
                // Close.
                Closing?.Invoke();
                wm.RemoveWindow(this);
            }
            else if (x >= Width - (titlebarHeight * (_canClose ? 2 : 1)) && _canResize)
            {
                // Maximise / restore.
                if (maximised)
                {
                    maximised = false;

                    MoveAndResize(originalX, originalY, originalWidth, originalHeight, sendWMEvent: false);

                    decorationWindow.Resize(originalWidth, titlebarHeight, sendWMEvent: false);

                    UserResized?.Invoke();
                    ProcessManager.GetProcess<WindowManager>().RerenderAll();
                }
                else
                {
                    maximised = true;

                    var taskbar = ProcessManager.GetProcess<ShellComponents.Taskbar>();
                    int taskbarHeight = taskbar.GetTaskbarHeight();

                    var dock = ProcessManager.GetProcess<ShellComponents.Dock.Dock>();
                    int dockHeight = dock.GetDockHeight();

                    originalX = X;
                    originalY = Y;
                    originalWidth = Width;
                    originalHeight = Height;

                    MoveAndResize(
                        0,
                        taskbarHeight + titlebarHeight,
                        (int)wm.ScreenWidth,
                        (int)wm.ScreenHeight - titlebarHeight - taskbarHeight - dockHeight,
                        sendWMEvent: false
                    );

                    decorationWindow.Resize((int)wm.ScreenWidth, titlebarHeight, sendWMEvent: false);

                    UserResized?.Invoke();
                    ProcessManager.GetProcess<WindowManager>().RerenderAll();
                }
                RenderDecoration();
            }
        }

        private void DecorationDown(int x, int y)
        {
            int buttonSpace = 0;
            if (_canClose)
            {
                buttonSpace += titlebarHeight;
            }
            if (_canResize)
            {
                buttonSpace += titlebarHeight;
            }
            if (x >= Width - buttonSpace || maximised || !_canMove) return;

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
            decorationWindow.Clear(Color.FromArgb(56, 56, 71));

            if (_smallIcon != null)
            {
                decorationWindow.DrawImageAlpha(_smallIcon, 2, 2);
            }

            decorationWindow.DrawString(Title, Color.White, (Width / 2) - ((FontData.Width * Title.Length) / 2), 4);

            if (_canClose)
            {
                decorationWindow.DrawImageAlpha(closeBitmap, Width - titlebarHeight, 0);
            }
            if (_canResize)
            {
                decorationWindow.DrawImageAlpha(maximised ? restoreBitmap : maximiseBitmap, Width - (titlebarHeight * (_canClose ? 2 : 1)), 0);
            }
            wm.Update(decorationWindow);
        }
    }
}
