using SphereOS.Core;
using SphereOS.Gui.UILib;
using System.Drawing;

namespace SphereOS.Gui.Apps
{
    internal class Settings : Process
    {
        internal Settings() : base("Settings", ProcessType.Application) { }

        AppWindow window;

        Window currentCategoryWindow;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        private void CategorySelected(int index)
        {
            switch (index)
            {
                case 0:
                    ShowAppearanceCategory();
                    break;
                case 1:
                    ShowDateTimeCategory();
                    break;
                default:
                    return;
            }
        }

        private void LeftStartButtonChanged(bool @checked)
        {
            ShellComponents.Taskbar taskbar = ProcessManager.GetProcess<ShellComponents.Taskbar>();
            taskbar.SetLeftStartButton(@checked);
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

            CheckBox leftStartButton = new CheckBox(appearance, 12, 40, 244, 16);
            leftStartButton.Text = "Left-hand start button";
            leftStartButton.CheckBoxChanged = LeftStartButtonChanged;
            wm.AddWindow(leftStartButton);

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
            dateTime.DrawString("Coming soon.", Color.Black, 12, 40);
            wm.AddWindow(dateTime);

            /*CheckBox leftStartButton = new CheckBox(dateTime, 12, 40, 244, 16);
            leftStartButton.Text = "12-hour clock";
            wm.AddWindow(leftStartButton);*/

            wm.Update(window);
        }

        #region Process
        internal override void Start()
        {
            base.Start();
            window = new AppWindow(this, 256, 256, 384, 256);
            window.Closing = TryStop;
            wm.AddWindow(window);

            window.Title = "Settings";

            Table categoryTable = new Table(window, 0, 0, 128, window.Height);
            categoryTable.Cells.Add("Appearance");
            categoryTable.Cells.Add("Date & Time");
            categoryTable.TableCellSelected = CategorySelected;
            categoryTable.SelectedCell = 0;
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
