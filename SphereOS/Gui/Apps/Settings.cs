using Cosmos.System;
using Cosmos.System.Graphics;
using SphereOS.Core;
using SphereOS.Gui.UILib;
using SphereOS.Users;
using System.Drawing;

namespace SphereOS.Gui.Apps
{
    internal class Settings : Process
    {
        internal Settings() : base("Settings", ProcessType.Application) { }

        AppWindow window;

        Window currentCategoryWindow;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        SettingsService settingsService = ProcessManager.GetProcess<SettingsService>();

        private static class Icons
        {
            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.Settings.User.bmp")]
            private static byte[] _iconBytes_User;
            internal static Bitmap Icon_User = new Bitmap(_iconBytes_User);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.Settings.Admin.bmp")]
            private static byte[] _iconBytes_Admin;
            internal static Bitmap Icon_Admin = new Bitmap(_iconBytes_Admin);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.Settings.Info.bmp")]
            private static byte[] _iconBytes_Info;
            internal static Bitmap Icon_Info = new Bitmap(_iconBytes_Info);
        }

        private void CategorySelected(int index)
        {
            switch (index)
            {
                case 0:
                    ShowAppearanceCategory();
                    break;
                case 1:
                    ShowDisplayCategory();
                    break;
                case 2:
                    ShowDateTimeCategory();
                    break;
                case 3:
                    ShowUsersCategory();
                    break;
                case 4:
                    ShowMouseCategory();
                    break;
                default:
                    return;
            }
        }

        private void LeftStartButtonChanged(bool @checked)
        {
            settingsService.LeftHandStartButton = @checked;
        }

        private void TwelveHourClockChanged(bool @checked)
        {
            settingsService.TwelveHourClock = @checked;
        }

        private void ShowFpsChanged(bool @checked)
        {
            settingsService.ShowFps = @checked;
        }

        private void MouseSensitivityChanged(float value)
        {
            settingsService.MouseSensitivity = value;
        }

        private void ShowAppearanceCategory()
        {
            if (currentCategoryWindow != null)
            {
                wm.RemoveWindow(currentCategoryWindow);
            }
            Window appearance = new Window(this, window, 128, 0, window.Width - 128, window.Height);
            currentCategoryWindow = appearance;
            appearance.DrawString("Appearance Settings", Color.DarkBlue, 12, 12);
            wm.AddWindow(appearance);

            Switch leftStartButton = new Switch(appearance, 12, 40, 244, 16);
            leftStartButton.Text = "Left-hand start button";
            leftStartButton.Checked = settingsService.LeftHandStartButton;
            leftStartButton.CheckBoxChanged = LeftStartButtonChanged;
            wm.AddWindow(leftStartButton);

            Switch showFps = new Switch(appearance, 12, 68, 244, 16);
            showFps.Text = "Show frames per second";
            showFps.Checked = settingsService.ShowFps;
            showFps.CheckBoxChanged = ShowFpsChanged;
            wm.AddWindow(showFps);

            wm.Update(window);
        }

        private void ShowDateTimeCategory()
        {
            if (currentCategoryWindow != null)
            {
                wm.RemoveWindow(currentCategoryWindow);
            }
            Window dateTime = new Window(this, window, 128, 0, window.Width - 128, window.Height);
            currentCategoryWindow = dateTime;
            dateTime.DrawString("Date & Time Settings", Color.DarkBlue, 12, 12);
            wm.AddWindow(dateTime);

            Switch twelveHourClock = new Switch(dateTime, 12, 40, 244, 16);
            twelveHourClock.Text = "12-hour clock";
            twelveHourClock.Checked = settingsService.TwelveHourClock;
            twelveHourClock.CheckBoxChanged = TwelveHourClockChanged;
            wm.AddWindow(twelveHourClock);

            AppMetadata calendarApp = AppManager.GetApp("Calendar");
            Button openCalendar = new Button(dateTime, 12, 68, 160, 20);
            openCalendar.Text = "Open Calendar";
            openCalendar.Image = calendarApp.Icon.Resize(20, 20);
            openCalendar.ImageLocation = Button.ButtonImageLocation.Left;
            openCalendar.OnClick = (int x, int y) =>
            {
                calendarApp.Start(wm);
            };
            wm.AddWindow(openCalendar);

            wm.Update(window);
        }

        private void ShowDisplayCategory()
        {

            if (currentCategoryWindow != null)
            {
                wm.RemoveWindow(currentCategoryWindow);
            }
            Window display = new Window(this, window, 128, 0, window.Width - 128, window.Height);
            currentCategoryWindow = display;
            display.DrawString("Display Settings", Color.DarkBlue, 12, 12);
            wm.AddWindow(display);

            Table resolutionsTable = new Table(display, 12, 40, display.Width - 24, display.Height - 12 - 16 - 12 - 12 - 16 - 12);
            resolutionsTable.AllowDeselection = false;
            for (int i = 0; i < wm.AvailableModes.Count; i++)
            {
                Mode mode = wm.AvailableModes[i];
                resolutionsTable.Cells.Add(new TableCell($"{mode.Width}x{mode.Height}"));
                if (mode.Equals(settingsService.Mode))
                {
                    resolutionsTable.SelectedCellIndex = i;
                }
            }
            resolutionsTable.Render();
            resolutionsTable.TableCellSelected = (int index) =>
            {
                Mode mode = wm.AvailableModes[index];
                settingsService.Mode = mode;
                settingsService.Flush();

                MessageBox messageBox = new MessageBox(this, "Restart Required", "Restart your PC to apply changes.");
                messageBox.Show();
            };
            wm.AddWindow(resolutionsTable);

            display.DrawImageAlpha(Icons.Icon_Info, 12, window.Height - 16 - 12);
            display.DrawString("Select a screen resolution.", Color.Gray, 36, window.Height - 16 - 12);

            wm.AddWindow(display);

            wm.Update(display);
        }

        private void ShowUsersCategory()
        {
            if (currentCategoryWindow != null)
            {
                wm.RemoveWindow(currentCategoryWindow);
            }
            Window users = new Window(this, window, 128, 0, window.Width - 128, window.Height);
            currentCategoryWindow = users;
            users.DrawString("Users", Color.DarkBlue, 12, 12);
            users.DrawImageAlpha(Icons.Icon_Info, 12, window.Height - 16 - 12);
            users.DrawString("Double-click on a user for info.", Color.Gray, 36, window.Height - 16 - 12);
            wm.AddWindow(users);

            Table usersTable = new Table(users, 12, 40, users.Width - 24, users.Height - 40 - 12 - 16 - 12);
            foreach (User user in UserManager.Users)
            {
                usersTable.Cells.Add(new TableCell(user.Admin ? Icons.Icon_Admin : Icons.Icon_User, user.Username, tag: user));
            }
            usersTable.OnDoubleClick = (int x, int y) =>
            {
                if (usersTable.SelectedCellIndex != -1)
                {
                    User user = (User)usersTable.Cells[usersTable.SelectedCellIndex].Tag;

                    AppWindow userDetail = new AppWindow(this, 256, 256, 256, 192);
                    userDetail.Title = user.Username;
                    wm.AddWindow(userDetail);

                    userDetail.DrawFilledRectangle(0, 0, userDetail.Width, 40, Color.DarkBlue);
                    userDetail.DrawImageAlpha(user.Admin ? Icons.Icon_Admin : Icons.Icon_User, 12, 12);
                    userDetail.DrawString($"User: {user.Username}", Color.White, 36, 12);

                    userDetail.DrawString($"Role: {(user.Admin ? "Admin" : "User")}", Color.Black, 12, 48);
                    userDetail.DrawString($"Unread messages: {user.Messages.Count}", Color.Black, 12, 76);

                    Button ok = new Button(userDetail, userDetail.Width - 80 - 12, userDetail.Height - 20 - 12, 80, 20);
                    ok.Text = "OK";
                    ok.OnClick = (int x, int y) =>
                    {
                        wm.RemoveWindow(userDetail);
                    };
                    wm.AddWindow(ok);

                    wm.Update(userDetail);
                }
            };
            usersTable.Render();
            wm.AddWindow(usersTable);

            wm.Update(window);
        }

        private void ShowMouseCategory()
        {
            if (currentCategoryWindow != null)
            {
                wm.RemoveWindow(currentCategoryWindow);
            }
            Window mouse = new Window(this, window, 128, 0, window.Width - 128, window.Height);
            currentCategoryWindow = mouse;
            mouse.DrawString("Mouse Settings", Color.DarkBlue, 12, 12);
            wm.AddWindow(mouse);

            mouse.DrawString("Mouse sensitivity", Color.Gray, 12, 40);

            RangeSlider mouseSensitivity = new RangeSlider(mouse, 12, 40, 244, 16, min: 0.5f, value: settingsService.MouseSensitivity, max: 1.5f);
            mouseSensitivity.Changed = MouseSensitivityChanged;
            wm.AddWindow(mouseSensitivity);

            wm.Update(window);
        }

        #region Process
        internal override void Start()
        {
            base.Start();
            window = new AppWindow(this, 256, 256, 448, 272);
            window.Closing = TryStop;
            wm.AddWindow(window);

            window.Title = "Settings";

            Table categoryTable = new Table(window, 0, 0, 128, window.Height);

            categoryTable.TextAlignment = Alignment.Middle;

            categoryTable.TableCellSelected = CategorySelected;
            categoryTable.SelectedCellIndex = 0;
            categoryTable.AllowDeselection = false;

            categoryTable.CellHeight = 24;

            /*categoryTable.Border = categoryTable.Background;
            categoryTable.SelectedBorder = categoryTable.SelectedBackground;*/

            categoryTable.Cells.Add(new TableCell("Appearance"));
            categoryTable.Cells.Add(new TableCell("Display"));
            categoryTable.Cells.Add(new TableCell("Date & Time"));
            categoryTable.Cells.Add(new TableCell("Users"));
            categoryTable.Cells.Add(new TableCell("Mouse"));

            categoryTable.Render();

            wm.AddWindow(categoryTable);

            ShowAppearanceCategory();

            wm.Update(window);
        }

        internal override void Run()
        {
        }
        #endregion
    }
}
