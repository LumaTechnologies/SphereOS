using Cosmos.System.Graphics;
using SphereOS.Core;
using SphereOS.Gui.SmoothMono;
using SphereOS.Gui.UILib;
using SphereOS.UILib.Animations;
using System;
using System.Collections.Generic;
using System.Drawing;

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
        private Button allAppsButton;

        private const int buttonsPadding = 12;
        private const int buttonsWidth = 96;
        private const int buttonsHeight = 20;
        private const int userHeight = 56;
        private const int userPadding = 12;
        private const int searchWidth = 128;

        private bool isOpen = false;

        internal void ShowStartMenu(bool focusSearch = false)
        {
            isOpen = true;

            bool leftHandStartButton = settingsService.LeftHandStartButton;

            window = new Window(this, leftHandStartButton ? 0 : (int)(wm.ScreenWidth / 2 - 408 / 2), 24, 408, 222);

            window.Clear(Color.FromArgb(56, 56, 71));

            window.DrawString($"Start", Color.White, 12, 12);

            Rectangle userRect = new Rectangle(userPadding, window.Height - userHeight + userPadding, window.Width - (userPadding * 2), userHeight - (userPadding * 2));
            window.DrawImageAlpha(Icons.Icon_User, userRect.X, (int)(userRect.Y + (userRect.Height / 2) - (Icons.Icon_User.Height / 2)));
            window.DrawString(Kernel.CurrentUser.Username, Color.White, (int)(userRect.X + Icons.Icon_User.Width + userPadding), (int)(userRect.Y + (userRect.Height / 2) - (16 / 2)));

            wm.AddWindow(window);

            int x = 12;
            int y = 44;
            for (int i = 0; i < 4; i++)
            {
                App app = AppManager.Apps[i];

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

            shutdownButton = new Button(window, window.Width - buttonsWidth - buttonsPadding, window.Height - buttonsHeight - ((userHeight / 2) - (buttonsHeight / 2)), buttonsWidth, buttonsHeight);
            shutdownButton.Text = "Shut down";
            shutdownButton.OnClick = ShutdownClicked;
            wm.AddWindow(shutdownButton);

            rebootButton = new Button(window, window.Width - (buttonsPadding * 2 + buttonsWidth * 2), window.Height - buttonsHeight - ((userHeight / 2) - (buttonsHeight / 2)), buttonsWidth, buttonsHeight);
            rebootButton.Text = "Restart";
            rebootButton.OnClick = RebootClicked;
            wm.AddWindow(rebootButton);

            allAppsButton = new Button(window, window.Width - buttonsWidth - buttonsPadding, window.Height - buttonsHeight - userHeight, buttonsWidth, buttonsHeight);
            allAppsButton.Text = "All apps >";
            allAppsButton.OnClick = AllAppsClicked;
            wm.AddWindow(allAppsButton);

            Table searchResults = null;
            TextBox searchBox = new TextBox(window, (window.Width / 2) - (searchWidth / 2), 12, searchWidth, 20);
            if (focusSearch)
            {
                wm.Focus = searchBox;
            }
            searchBox.PlaceholderText = "Search";
            searchBox.Changed = () =>
            {
                if (searchResults == null)
                {
                    searchResults = new Table(searchBox, 0, searchBox.Height, searchBox.Width, 0);

                    searchResults.CellHeight = 24;

                    searchResults.TableCellSelected = (int index) =>
                    {
                        if (index != -1)
                        {
                            ((App)searchResults.Cells[index].Tag).Start(this);
                            HideStartMenu();
                        }
                    };
                }

                searchResults.Cells.Clear();

                if (searchBox.Text.Trim().Length > 0)
                {
                    foreach (App app in AppManager.Apps)
                    {
                        if (app.Name.ToLower().StartsWith(searchBox.Text.ToLower()))
                        {
                            searchResults.Cells.Add(new TableCell(app.Icon.Resize(20, 20), app.Name, tag: app));
                        }
                    }
                }

                if (searchResults.Cells.Count > 0)
                {
                    searchResults.Resize(searchResults.Width, searchResults.Cells.Count * searchResults.CellHeight);
                    searchResults.Render();

                    wm.AddWindow(searchResults);

                    wm.Update(searchResults);
                }
                else
                {
                    wm.RemoveWindow(searchResults);
                }
            };
            searchBox.Submitted = () =>
            {
                searchBox.Text = string.Empty;
                wm.Update(searchBox);

                if (searchResults != null && searchResults.Cells.Count > 0)
                {
                    ((App)searchResults.Cells[0].Tag).Start(this);
                    HideStartMenu();
                }
            };
            searchBox.OnUnfocused = () =>
            {
                if (searchResults != null)
                {
                    wm.RemoveWindow(searchResults);
                    searchResults = null;

                    searchBox.Text = string.Empty;
                    wm.Update(searchBox);
                }
            };
            wm.AddWindow(searchBox);

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

        private void AllAppsClicked(int x, int y)
        {
            Table allAppsTable = new Table(window, 0, 0, window.Width, window.Height);

            allAppsTable.CellHeight = 24;

            allAppsTable.Background = Color.FromArgb(56, 56, 71);
            allAppsTable.Border = Color.FromArgb(36, 36, 51);
            allAppsTable.Foreground = Color.White;

            foreach (App app in AppManager.Apps)
            {
                allAppsTable.Cells.Add(new TableCell(app.Icon.Resize(20, 20), app.Name));
            }
            allAppsTable.Render();

            allAppsTable.TableCellSelected = (int index) =>
            {
                if (index != -1)
                {
                    AppManager.Apps[index].Start(this);
                    HideStartMenu();
                }
            };

            wm.AddWindow(allAppsTable);

            wm.Update(allAppsTable);
        }

        internal void HideStartMenu()
        {
            isOpen = false;
            wm.RemoveWindow(window);
        }

        internal void ToggleStartMenu(bool focusSearch = false)
        {
            if (isOpen)
            {
                HideStartMenu();
            }
            else
            {
                ShowStartMenu(focusSearch);
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
