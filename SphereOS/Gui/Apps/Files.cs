using Cosmos.System.Graphics;
using SphereOS.Core;
using SphereOS.Gui.UILib;
using System.Drawing;
using System.IO;

namespace SphereOS.Gui.Apps
{
    internal class Files : Process
    {
        internal Files() : base("Files", ProcessType.Application) { }

        AppWindow window;

        Table entryTable;
        Table shortcutsTable;

        ImageBlock up;

        TextBox pathBox;

        Window header;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        private static class Icons
        {
            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.Files.Directory.bmp")]
            private static byte[] _iconBytes_Directory;
            internal static Bitmap Icon_Directory = new Bitmap(_iconBytes_Directory);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.Files.File.bmp")]
            private static byte[] _iconBytes_File;
            internal static Bitmap Icon_File = new Bitmap(_iconBytes_File);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.Files.Drive.bmp")]
            private static byte[] _iconBytes_Drive;
            internal static Bitmap Icon_Drive = new Bitmap(_iconBytes_Drive);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.Files.Home.bmp")]
            private static byte[] _iconBytes_Home;
            internal static Bitmap Icon_Home = new Bitmap(_iconBytes_Home);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.Files.Up.bmp")]
            private static byte[] _iconBytes_Up;
            internal static Bitmap Icon_Up = new Bitmap(_iconBytes_Up);
        }

        private string currentDir = @"0:\";

        private const int pathBoxMargin = 4;
        private const int pathBoxHeight = 24;
        private const int shortcutsWidth = 128;
        private const int headerHeight = 32;

        private const string dirEmptyMessage = "This folder is empty.";

        private readonly (string Name, string Path)[] shortcuts = new (string, string)[]
        {
            ("SphereOS (0:)", @"0:\"),
            ("My Home", @$"0:\users\{Kernel.CurrentUser.Username}"),
            ("Users", @"0:\users")
        };

        private Bitmap GetFileIcon(string path)
        {
            return Icons.Icon_File;
        }

        private Bitmap GetDirectoryIcon(string path)
        {
            if (Path.TrimEndingDirectorySeparator(path).StartsWith(@"0:\users\"))
            {
                return Icons.Icon_Home;
            }

            switch (path)
            {
                case @"0:\":
                    return Icons.Icon_Drive;
                default:
                    return Icons.Icon_Directory;
            }
        }

        private void PopulateEntryTable()
        {
            entryTable.Cells.Clear();
            entryTable.SelectedCellIndex = -1;

            bool empty = true;
            foreach (string path in Directory.GetDirectories(currentDir))
            {
                entryTable.Cells.Add(new TableCell(GetDirectoryIcon(path), Path.GetFileName(path), tag: "Directory"));
                empty = false;
            }
            foreach (string path in Directory.GetFiles(currentDir))
            {
                entryTable.Cells.Add(new TableCell(GetFileIcon(path), Path.GetFileName(path), tag: "File"));
                empty = false;
            }

            if (empty)
            {
                entryTable.Clear(entryTable.Background);
                entryTable.DrawString(dirEmptyMessage, Color.Black, (entryTable.Width / 2) - ((dirEmptyMessage.Length * 8) / 2), 12);
                wm.Update(entryTable);
            }
            else
            {
                entryTable.Render();
            }
        }

        private void PopulateShortcutTable()
        {
            shortcutsTable.Cells.Clear();
            foreach ((string Name, string Path) item in shortcuts)
            {
                shortcutsTable.Cells.Add(new TableCell(GetDirectoryIcon(item.Path), item.Name, tag: item.Path));
            }
            shortcutsTable.Render();
        }

        private bool NavigateTo(string path)
        {
            string sanitised = PathSanitiser.SanitisePath(path);

            if (!FileSecurity.CanAccess(Kernel.CurrentUser, sanitised))
            {
                MessageBox messageBox = new MessageBox(this, "Unauthorised", $"Access to {Path.GetFileName(sanitised)} is unauthorised.");
                messageBox.Show();

                return false;
            }

            if (!Directory.Exists(sanitised))
            {
                MessageBox messageBox = new MessageBox(this, "Files", $"SphereOS can't find that folder. Check the spelling and try again.");
                messageBox.Show();

                return false;
            }

            currentDir = sanitised;
            pathBox.Text = sanitised;

            PopulateEntryTable();
            UpdateSelectedShortcut();
            RenderHeader();

            if (sanitised == @"0:\")
            {
                window.Title = "Files";
            }
            else
            {
                window.Title = Path.GetDirectoryName(sanitised);
            }

            return true;
        }

        private void UpdateSelectedShortcut()
        {
            for (int i = 0; i < shortcuts.Length; i++)
            {
                if (shortcuts[i].Path == currentDir)
                {
                    shortcutsTable.SelectedCellIndex = i;
                    return;
                }
            }
            shortcutsTable.SelectedCellIndex = -1;
        }

        private void EntryTableDoubleClicked(int x, int y)
        {
            if (entryTable.SelectedCellIndex != -1)
            {
                TableCell cell = entryTable.Cells[entryTable.SelectedCellIndex];
                string path = Path.Join(currentDir, cell.Text);
                if ((string)cell.Tag == "Directory")
                {
                    NavigateTo(path);
                }
                else if ((string)cell.Tag == "File")
                {
                    if (!FileSecurity.CanAccess(Kernel.CurrentUser, path))
                    {
                        MessageBox messageBox = new MessageBox(this, "Unauthorised", $"Access to {Path.GetFileName(path)} is unauthorised.");
                        messageBox.Show();
                        return;
                    }

                    switch (Path.GetExtension(path))
                    {
                        case ".txt" or ".ini" or ".rs":
                            ProcessManager.AddProcess(new Notepad(path)).Start();
                            break;
                        default:
                            MessageBox messageBox = new MessageBox(this, "Cannot Open File", "SphereOS cannot open this type of file.");
                            messageBox.Show();
                            break;
                    }
                }
            }
        }

        private void ShortcutsTableCellSelected(int index)
        {
            if (index != -1)
            {
                bool success = NavigateTo(shortcuts[index].Path);
                if (!success)
                {
                    UpdateSelectedShortcut();
                }
            }
        }

        private void PathBoxSubmitted()
        {
            bool success = NavigateTo(pathBox.Text);
            if (!success)
            {
                pathBox.Text = currentDir;
            }
        }

        private void UpClicked()
        {
            DirectoryInfo parent = Directory.GetParent(currentDir);
            if (parent != null)
            {
                NavigateTo(parent.FullName);
            }
        }

        private void RenderHeader(bool updateWindow = true)
        {
            header.Clear(Color.DarkBlue);

            header.DrawImageAlpha(GetDirectoryIcon(currentDir), 8, 8);

            DirectoryInfo info = new DirectoryInfo(currentDir);

            string currentDirFriendlyName = Path.GetFileName(currentDir);
            if (currentDir == $@"0:\users\{Kernel.CurrentUser.Username}")
            {
                currentDirFriendlyName = "My Home";
            }
            header.DrawString(info.Parent == null ? currentDir : currentDirFriendlyName, Color.White, 32, 8);

            if (updateWindow)
            {
                wm.Update(header);
            }
        }

        private void WindowClicked(int x, int y)
        {
            if (x < pathBoxHeight && y < pathBoxHeight)
            {
                UpClicked();
            }
        }

        internal override void Start()
        {
            base.Start();
            window = new AppWindow(this, 288, 240, 512, 304);
            wm.AddWindow(window);
            window.Title = "Files";
            window.OnClick = WindowClicked;
            window.Closing = TryStop;

            window.Clear(Color.DarkGray);

            window.DrawImageAlpha(Icons.Icon_Up, 0, 0);

            pathBox = new TextBox(window, (pathBoxMargin / 2) + pathBoxHeight, pathBoxMargin / 2, window.Width - pathBoxMargin - pathBoxHeight, 24 - pathBoxMargin);
            pathBox.Text = currentDir;
            pathBox.Submitted = PathBoxSubmitted;
            wm.AddWindow(pathBox);

            entryTable = new Table(window, shortcutsWidth, pathBoxHeight + headerHeight, window.Width - shortcutsWidth, window.Height - pathBoxHeight - headerHeight);
            entryTable.OnDoubleClick = EntryTableDoubleClicked;
            PopulateEntryTable();
            wm.AddWindow(entryTable);

            header = new Window(this, window, shortcutsWidth, pathBoxHeight, window.Width - shortcutsWidth, headerHeight);
            wm.AddWindow(header);
            RenderHeader(updateWindow: false);

            shortcutsTable = new Table(window, 0, pathBoxHeight, shortcutsWidth, window.Height - pathBoxHeight);
            shortcutsTable.AllowDeselection = false;
            shortcutsTable.Background = Color.DarkGray;
            shortcutsTable.Foreground = Color.White;
            PopulateShortcutTable();
            shortcutsTable.SelectedCellIndex = 0;
            shortcutsTable.TableCellSelected = ShortcutsTableCellSelected;
            wm.AddWindow(shortcutsTable);

            wm.Update(window);
        }

        internal override void Run()
        {
        }
    }
}
