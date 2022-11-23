using SphereOS.Core;
using SphereOS.Logging;
using SphereOS.Users;
using System;
using Sys = Cosmos.System;
using SphereOS.Shell;
using Microsoft.VisualBasic;

namespace SphereOS
{
    public class Kernel : Sys.Kernel
    {
        public const string Version = "0.1.5 Preview";
        
        internal static User CurrentUser = null;

        protected override void BeforeRun()
        {
            try
            {
                Log.Info("Kernel", "Starting SphereOS kernel.");

                Boot.BootManager.StartAll();

                Log.Info("Kernel", "SphereOS kernel started.");

                Util.PrintTask("Starting GUI...");

                var wm = new Gui.WindowManager();
                Gui.AppManager.LoadAllApps();
                ProcessManager.AddProcess(wm).Start(); 
                ProcessManager.AddProcess(wm, new Gui.ShellComponents.Taskbar()).Start();
                //ProcessManager.AddProcess(new Shell.Shell()).Start();
            }
            catch (Exception e)
            {
                CrashScreen.ShowCrashScreen(e);
            }
        }

        protected override void Run()
        {
            /*for (int i = 1; i < 15; i++)
            {
                var win3 = new Gui.Window(100 + i * 30, 100 + i * 20, 100, 100);
                win3.Clear(System.Drawing.Color.LightGray);
                win3.DrawCircle(50, 50, 50, System.Drawing.Color.Black);
                win3.DrawCircle(50, 50, 48, System.Drawing.Color.White);
                win3.DrawLine(50, 50, 50, 0, System.Drawing.Color.Black);
                win3.DrawLine(50, 50, 100, 50, System.Drawing.Color.Black);
                wm.AddWindow(win3);
                wm.Update(win3);
            }
            var win = new Gui.Window(20, 20, 200, 100);
            win.Clear(System.Drawing.Color.LightCyan);
            win.DrawRectangle(190, 90, 10, 10, System.Drawing.Color.CadetBlue);
            win.DrawString("Circles!", System.Drawing.Color.Red, 10, 10);
            win.DrawPoint(80, 80, System.Drawing.Color.AliceBlue);
            win.DrawLine(0, 0, 30, 30, System.Drawing.Color.Orange);
            win.DrawLine(0, 0, 0, 30, System.Drawing.Color.Pink);
            win.DrawLine(0, 0, 30, 0, System.Drawing.Color.DarkMagenta);
            win.DrawCircle(50, 50, 25, System.Drawing.Color.Blue);
            wm.AddWindow(win);
            var win2 = new Gui.Window(50, 50, 200, 100);
            win2.RelativeTo = win;
            win2.Clear(System.Drawing.Color.White);
            win2.DrawRectangle(190, 90, 10, 10, System.Drawing.Color.CadetBlue);
            win2.DrawString("Beautiful Graphics!", System.Drawing.Color.Red, 10, 10);
            win2.DrawPoint(80, 80, System.Drawing.Color.AliceBlue);
            win2.DrawLine(10, 10, 30, 30, System.Drawing.Color.Orange);
            win2.DrawLine(10, 10, 0, 30, System.Drawing.Color.Pink);
            win2.DrawLine(10, 10, 30, 0, System.Drawing.Color.DarkMagenta);
            win2.DrawCircle(75, 75, 25, System.Drawing.Color.Gray);
            wm.AddWindow(win2);
            wm.Update(win);
            for (int i = 0; i < 10; i++)
            {
                var wx = new Gui.Window(500, 20 + (i * 50), 150, 40);
                wx.Clear(System.Drawing.Color.White);
                wm.AddWindow(wx);
                var wx2 = new Gui.Window(0, 0, 128, 24);
                wx2.RelativeTo = wx;
                wx2.Clear(System.Drawing.Color.DarkGray);
                wx2.DrawRectangle(0, 0, 128, 24, System.Drawing.Color.Gray);
                wx2.DrawString("Button " + i.ToString(), System.Drawing.Color.White, 0, 0);
                wm.AddWindow(wx2);
                wm.Update(wx);
                int clicks = 0;
                wx2.OnClick = (int x, int y) =>
                {
                    Kernel.PrintDebug("1");
                    clicks++;
                    Kernel.PrintDebug("2");
                    if (wx2 == null)
                    {
                        Kernel.PrintDebug("IT'S GONE!!!");
                        return;
                    }
                    wx2.Clear(System.Drawing.Color.White);
                    Kernel.PrintDebug("3");
                    wx2.DrawString(clicks.ToString() + " clicks!", System.Drawing.Color.Black, 0, 0);
                    Kernel.PrintDebug("4");
                    wm.Update(wx2);
                    Kernel.PrintDebug("5");
                };
            }*/
            //var win = new Gui.Window(256, 256, 384, 256);
            try
            {
                //Shell.Shell.Execute();
                ProcessManager.Yield();
                //ProcessManager.Sweep();
            }
            catch (Exception e)
            {
                CrashScreen.ShowCrashScreen(e);
            }
        }
    }
}
