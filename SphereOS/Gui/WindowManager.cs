using Cosmos.HAL.Drivers.PCI.Video;
using Cosmos.System;
using Cosmos.System.Graphics;
using SphereOS.Core;
using SphereOS.Gui.ShellComponents;
using System;
using System.Collections.Generic;
using System.Drawing;
using static Cosmos.HAL.Drivers.PCI.Video.VMWareSVGAII;

namespace SphereOS.Gui
{
    internal class WindowManager : Process
    {
        internal WindowManager() : base("WindowManager", ProcessType.Service)
        {
            Critical = true;
        }

        private VMWareSVGAII driver;

        private List<Window> windows = new List<Window>();

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

        private void SetCursor(bool visible, uint x, uint y)
        {
            driver.WriteRegister(Register.CursorOn, (uint)(visible ? 1 : 0));
            driver.WriteRegister(Register.CursorX, x);
            driver.WriteRegister(Register.CursorY, y);
            driver.WriteRegister((Register)0x0C, driver.ReadRegister((Register)0x0C) + 1); // CursorCount
        }

        private void DefineAlphaCursor(Bitmap bitmap)
        {
            driver.WaitForFifo();
            driver.WriteToFifo((uint)FIFOCommand.DEFINE_ALPHA_CURSOR);
            driver.WriteToFifo(0); // ID
            driver.WriteToFifo(0); // Hotspot X
            driver.WriteToFifo(0); // Hotspot Y
            driver.WriteToFifo(bitmap.Width); // Width
            driver.WriteToFifo(bitmap.Height); // Height
            for (int i = 0; i < bitmap.rawData.Length; i++)
                driver.WriteToFifo((uint)bitmap.rawData[i]);
            driver.WaitForFifo();
        }

        private void SetupDriver()
        {
            driver = new VMWareSVGAII();
            driver.SetMode(ScreenWidth, ScreenHeight, depth: bytesPerPixel * 8);
        }

        private void SetupMouse()
        {
            MouseManager.ScreenWidth = ScreenWidth;
            MouseManager.ScreenHeight = ScreenHeight;

            MouseManager.X = ScreenWidth / 2;
            MouseManager.Y = ScreenHeight / 2;

            DefineAlphaCursor(cursorBitmap);
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
                    Focus = window;
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
                    StartMenu.CurrentStartMenu.ToggleStartMenu();
                    return;
                }
                Focus?.KeyPressed?.Invoke(key);
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
                SetCursor(true, mouseX, mouseY);
            }
            lastMouseX = mouseX;
            lastMouseY = mouseY;
        }

        #region Process
        internal override void Start()
        {
            base.Start();

            ScreenWidth = 1280;
            ScreenHeight = 800;
            bytesPerPixel = 4;

            SetupDriver();
            SetupMouse();
            SetupWallpaper();

            fpsCounter = new Window(this, 0, (int)(ScreenHeight - 16), 64, 16);
            AddWindow(fpsCounter);
        }

        internal override void Run()
        {
            UpdateFps();

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
