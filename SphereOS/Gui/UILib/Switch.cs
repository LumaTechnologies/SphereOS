using Cosmos.System;
using Cosmos.System.Graphics;
using System;
using System.Drawing;

namespace SphereOS.Gui.UILib
{
    internal class Switch : CheckBox
    {
        public Switch(Window parent, int x, int y, int width, int height) : base(parent, x, y, width, height)
        {
            OnDown = SwitchDown;
            OnClick = null;
        }

        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.SwitchOff.bmp")]
        private static byte[] offBytes;
        private static Bitmap offBitmap = new Bitmap(offBytes);

        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.SwitchOn.bmp")]
        private static byte[] onBytes;
        private static Bitmap onBitmap = new Bitmap(onBytes);

        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.SwitchKnob.bmp")]
        private static byte[] knobBytes;
        private static Bitmap knobBitmap = new Bitmap(knobBytes);

        private const int maximumToggleDrag = 4;

        private int lastMouseX = 0;
        private int totalDragged = 0;
        private bool held = false;

        private void SwitchDown(int x, int y)
        {
            lastMouseX = (int)MouseManager.X;
            totalDragged = 0;
            held = true;
            Render();
        }

        private void Release()
        {
            held = false;
            if (totalDragged <= maximumToggleDrag)
            {
                // Interpret as a toggle.
                Checked = !Checked;
            }
            else
            {
                // Interpret as a drag rather than a toggle,
                // setting the Checked state based on where
                // the switch knob is.
                Checked = knobX >= (offBitmap.Width / 2) - (knobBitmap.Width / 2);
            }
        }

        private double knobX = -1;
        private double knobGoal = 0;

        internal override void Render()
        {
            knobGoal = (int)(Checked ? offBitmap.Width - knobBitmap.Width : 0);

            if (held && MouseManager.MouseState != MouseState.Left)
            {
                Release();
            }

            if (held)
            {
                int diff = (int)(MouseManager.X - lastMouseX);
                lastMouseX = (int)MouseManager.X;
                totalDragged += diff;
                knobX = Math.Clamp(knobX + diff, 0, offBitmap.Width - knobBitmap.Width);

                WM.UpdateQueue.Enqueue(this);
            }
            else
            {
                double oldKnobX = knobX;
                if (knobX == -1)
                {
                    knobX = knobGoal;
                }
                else
                {
                    double diff = knobGoal - knobX;
                    double move = diff / 8d;
                    knobX += move;
                }
                if (Math.Abs(knobX - oldKnobX) < 0.25)
                {
                    knobX = knobGoal;
                }
                else
                {
                    WM.UpdateQueue.Enqueue(this);
                }
            }

            Clear(Background);

            int switchX = 0;
            int switchY = (Height / 2) - ((int)offBitmap.Height / 2);

            int textX = (int)(offBitmap.Width + 8);
            int textY = (Height / 2) - (16 / 2);

            DrawImageAlpha(Checked ? onBitmap : offBitmap, switchX, switchY);
            DrawImageAlpha(knobBitmap, (int)knobX, switchY);

            DrawString(Text, Foreground, textX, textY);

            WM.Update(this);
        }
    }
}
