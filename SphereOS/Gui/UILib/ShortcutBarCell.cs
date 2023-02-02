using Cosmos.System.Graphics;
using System;

namespace SphereOS.Gui.UILib
{
    internal class ShortcutBarCell
    {
        internal ShortcutBarCell(string text, Action onClick)
        {
            Text = text;
            OnClick = onClick;
        }

        internal string Text { get; set; } = string.Empty;

        internal Action OnClick { get; set; }
    }
}
