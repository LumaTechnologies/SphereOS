using Cosmos.System;
using Cosmos.System.Graphics;
using SphereOS.Core;
using SphereOS.Gui.ShellComponents;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace SphereOS.Gui
{
    internal class WindowManager : Process
    {
        internal WindowManager() : base("WindowManager", ProcessType.Service)
        {
            Critical = true;
        }

        private Cosmos.HAL.Drivers.PCI.Video.VMWareSVGAII driver;

        private List<Window> windows = new List<Window>();

        internal Queue<Window> UpdateQueue = new Queue<Window>();

        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.Cursor.bmp")]
        private static byte[] cursorBytes;
        private static Bitmap cursorBitmap = new Bitmap(cursorBytes);

        /*[IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.WaitCursor.bmp")]
        private static byte[] waitCursorBytes;
        private static Bitmap waitCursorBitmap = new Bitmap(waitCursorBytes);*/

        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.Wallpaper_1280_800.bmp")]
        private static byte[] wallpaperBytes;
        private static Bitmap wallpaperBitmap = new Bitmap(wallpaperBytes);

        internal uint ScreenWidth { get; private set; }
        internal uint ScreenHeight { get; private set; }

        internal Window Focus { get; set; }

        private uint bytesPerPixel { get; set; }

        private bool bufferModified = false;

        private DateTime lastClickDate = DateTime.MinValue;
        private TimeSpan doubleClickTimeSpan = TimeSpan.FromSeconds(1);

        private Window fpsCounter;
        private int fps;
        private int framesThisSecond;
        private int lastSecond;

        private MouseState lastMouseState = MouseState.None;
        private uint lastMouseX = 0;
        private uint lastMouseY = 0;

        private int sweepCounter = 0;

        private Window wallpaperWindow;

        internal int Fps
        {
            get
            {
                return fps;
            }
        }

        internal List<Mode> AvailableModes { get; } = new List<Mode>
        {
            /* SD Resolutions */
            /*new Mode(320, 200, ColorDepth.ColorDepth32),
            new Mode(320, 240, ColorDepth.ColorDepth32),
            new Mode(640, 480, ColorDepth.ColorDepth32),
            new Mode(720, 480, ColorDepth.ColorDepth32),*/
            new Mode(800, 600, ColorDepth.ColorDepth32),
            new Mode(1024, 768, ColorDepth.ColorDepth32),
            new Mode(1152, 768, ColorDepth.ColorDepth32),

            /* Old HD-Ready Resolutions */
            new Mode(1280, 720, ColorDepth.ColorDepth32),
            new Mode(1280, 768, ColorDepth.ColorDepth32),
            new Mode(1280, 800, ColorDepth.ColorDepth32),  // WXGA
            new Mode(1280, 1024, ColorDepth.ColorDepth32), // SXGA

            /* Better HD-Ready Resolutions */
            new Mode(1360, 768, ColorDepth.ColorDepth32),
            new Mode(1440, 900, ColorDepth.ColorDepth32),  // WXGA+
            new Mode(1400, 1050, ColorDepth.ColorDepth32), // SXGA+
            new Mode(1600, 1200, ColorDepth.ColorDepth32), // UXGA
            new Mode(1680, 1050, ColorDepth.ColorDepth32), // WXGA++

            /* HDTV Resolutions */
            new Mode(1920, 1080, ColorDepth.ColorDepth32),
            new Mode(1920, 1200, ColorDepth.ColorDepth32), // WUXGA

            /* 2K Resolutions */
            /*new Mode(2048, 1536, ColorDepth.ColorDepth32), // QXGA
            new Mode(2560, 1080, ColorDepth.ColorDepth32), // UW-UXGA
            new Mode(2560, 1600, ColorDepth.ColorDepth32), // WQXGA
            new Mode(2560, 2048, ColorDepth.ColorDepth32), // QXGA+
            new Mode(3200, 2048, ColorDepth.ColorDepth32), // WQXGA+
            new Mode(3200, 2400, ColorDepth.ColorDepth32), // QUXGA
            new Mode(3840, 2400, ColorDepth.ColorDepth32), // WQUXGA*/
        };

        private void RenderWindow(Window window)
        {
            bufferModified = true;
            int screenX = window.ScreenX;
            int screenY = window.ScreenY;
            for (int y = 0; y < window.Height; y++)
            {
                int index = (int)(((y + screenY) * ScreenWidth) + screenX);
                driver.VideoMemory.Copy((int)((index * bytesPerPixel) + driver.FrameSize), window.Buffer, y * window.Width, window.Width);
            }
        }

        private void UpdateAbove(Window window)
        {
            Rectangle aRect = new Rectangle(window.ScreenX, window.ScreenY, window.Width, window.Height);
            int aboveIndex = windows.IndexOf(window) + 1;
            for (int i = aboveIndex; i < windows.Count; i++)
            {
                Window bWindow = windows[i];
                Rectangle bRect = new Rectangle(bWindow.ScreenX, bWindow.ScreenY, bWindow.Width, bWindow.Height);
                if (aRect.IntersectsWith(bRect))
                {
                    RenderWindow(bWindow);
                    aRect = Rectangle.Union(aRect, bRect);
                }
            }
        }

        internal void Update(Window window)
        {
            if (!windows.Contains(window)) return;
            RenderWindow(window);
            UpdateAbove(window);
        }

        internal void RerenderAll()
        {
            foreach (Window window in windows)
            {
                RenderWindow(window);
            }
        }

        internal void AddWindow(Window window)
        {
            if (windows.Contains(window)) return;
            windows.Add(window);
            window.WM_RefreshAll = RerenderAll;
        }

        internal void RemoveWindow(Window window, bool rerender = true)
        {
            windows.Remove(window);
            for (int i = windows.Count - 1; i >= 0; i--)
            {
                if (i >= windows.Count) continue;
                if (windows[i].RelativeTo == window)
                {
                    RemoveWindow(windows[i], rerender: false);
                }
            }
            if (rerender)
            {
                foreach (Window window1 in windows)
                {
                    RenderWindow(window1);
                }
            }
        }

        internal void Clear()
        {
            for (int i = windows.Count - 1; i >= 0; i--)
            {
                if (i >= windows.Count) continue;
                if (windows[i].Process != this)
                {
                    if (windows[i] is UILib.AppWindow appWindow)
                    {
                        appWindow.Closing?.Invoke();
                    }
                    RemoveWindow(windows[i], rerender: false);
                }
            }
            RerenderAll();
        }

        private void SetupDriver()
        {
            driver = new Cosmos.HAL.Drivers.PCI.Video.VMWareSVGAII();
            driver.SetMode(ScreenWidth, ScreenHeight, depth: bytesPerPixel * 8);
        }

        private void SetupMouse()
        {
            MouseManager.ScreenWidth = ScreenWidth;
            MouseManager.ScreenHeight = ScreenHeight;

            MouseManager.X = ScreenWidth / 2;
            MouseManager.Y = ScreenHeight / 2;

            driver.DefineAlphaCursor(cursorBitmap.Width, cursorBitmap.Height, cursorBitmap.rawData);
        }

        private Window GetWindowAtPos(uint x, uint y)
        {
            for (int i = windows.Count - 1; i >= 0; i--)
            {
                Window window = windows[i];
                if (MouseManager.X >= window.ScreenX
                    && MouseManager.Y >= window.ScreenY
                    && MouseManager.X < window.ScreenX + window.Width
                    && MouseManager.Y < window.ScreenY + window.Height)
                {
                    return window;
                }
            }
            return null;
        }

        private void DispatchEvents()
        {
            Window window = GetWindowAtPos(MouseManager.X, MouseManager.Y);

            if (window != null)
            {
                int relativeX = (int)(MouseManager.X - window.ScreenX);
                int relativeY = (int)(MouseManager.Y - window.ScreenY);

                if (MouseManager.MouseState == MouseState.Left && lastMouseState == MouseState.None)
                {
                    lastMouseState = MouseManager.MouseState;
                    window.OnDown?.Invoke(relativeX, relativeY);
                }
                else if (MouseManager.MouseState == MouseState.None && lastMouseState == MouseState.Left)
                {
                    lastMouseState = MouseManager.MouseState;

                    if (Focus != null && Focus != window)
                    {
                        Focus.OnUnfocused?.Invoke();
                    }
                    Focus = window;
                    window.OnFocused?.Invoke();

                    window.OnClick?.Invoke(relativeX, relativeY);

                    if (DateTime.Now - lastClickDate < doubleClickTimeSpan)
                    {
                        window.OnDoubleClick?.Invoke(relativeX, relativeY);
                    }
                    lastClickDate = DateTime.Now;
                }
            }

            if (KeyboardManager.TryReadKey(out KeyEvent key))
            {
                // To-do: Move this out of WindowManager.
                if (key.Key == ConsoleKeyEx.LWin || key.Key == ConsoleKeyEx.RWin)
                {
                    StartMenu.CurrentStartMenu?.ToggleStartMenu(focusSearch: true);
                    return;
                }
                Focus?.OnKeyPressed?.Invoke(key);
            }
        }

        private void UpdateFps()
        {
            framesThisSecond++;
            int second = DateTime.Now.Second;
            if (second != lastSecond)
            {
                fps = framesThisSecond;
                framesThisSecond = 0;
                fpsCounter.Clear(Color.Black);
                fpsCounter.DrawString($"{fps.ToString()} FPS", Color.White, 0, 0);
                Update(fpsCounter);
            }
            lastSecond = second;
        }

        private void SetupWallpaper()
        {
            Window wallpaperWindow = new Window(this, 0, 0, (int)ScreenWidth, (int)ScreenHeight);
            AddWindow(wallpaperWindow);

            Bitmap resized = wallpaperBitmap.Resize(ScreenWidth, ScreenHeight);

            resized.rawData.CopyTo(wallpaperWindow.Buffer, 0);
            Update(wallpaperWindow);
        }

        private void UpdateCursor()
        {
            uint mouseX = MouseManager.X;
            uint mouseY = MouseManager.Y;
            if (mouseX != lastMouseX || mouseY != lastMouseY)
            {
                driver.SetCursor(true, mouseX, mouseY);
            }
            lastMouseX = mouseX;
            lastMouseY = mouseY;
        }

        private void Sweep()
        {
            if (sweepCounter % 10 == 0)
            {
                foreach (Window window in windows)
                {
                    if (window.Process != null && !window.Process.IsRunning)
                    {
                        RemoveWindow(window);
                    }
                }
            }
            sweepCounter++;
        }

        #region Process
        internal override void Start()
        {
            base.Start();

            SettingsService settingsService = ProcessManager.GetProcess<SettingsService>();

            ScreenWidth = (uint)settingsService.Mode.Width;
            ScreenHeight = (uint)settingsService.Mode.Height;
            bytesPerPixel = 4;

            SetupDriver();
            SetupMouse();
            SetupWallpaper();

            fpsCounter = new Window(this, (int)(ScreenWidth) - 64, (int)(ScreenHeight - 16), 64, 16);
            AddWindow(fpsCounter);
        }

        internal override void Run()
        {
            UpdateFps();

            Sweep();

            if (UpdateQueue.Count > 0)
            {
                Window toUpdate = UpdateQueue.Dequeue();
                if (toUpdate is UILib.Control control)
                {
                    control.Render();
                }
                RenderWindow(toUpdate);
            }

            DispatchEvents();

            if (bufferModified)
            {
                driver.DoubleBufferUpdate();
            }

            UpdateCursor();
        }

        internal override void Stop()
        {
            base.Stop();

            driver.Disable();
        }
        #endregion
    }
}
