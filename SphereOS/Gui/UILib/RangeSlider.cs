using Cosmos.System;
using SphereOS.Core;
using SphereOS.Gui.SmoothMono;
using System;
using System.Drawing;

namespace SphereOS.Gui.UILib
{
    internal class RangeSlider : Control
    {
        public RangeSlider(Window parent, int x, int y, int width, int height) : base(parent, x, y, width, height)
        {
            OnDown = RangeSliderDown;
        }

        public RangeSlider(Window parent, int x, int y, int width, int height, float min, float value, float max) : base(parent, x, y, width, height)
        {
            OnDown = RangeSliderDown;

            _minimum = min;
            _value = value;
            _maximum = max;

            Render();
        }

        private Color _background = Color.White;
        internal Color Background
        {
            get
            {
                return _background;
            }
            set
            {
                _background = value;
                Render();
            }
        }

        private Color _foreground = Color.Gray;
        internal Color Foreground
        {
            get
            {
                return _foreground;
            }
            set
            {
                _foreground = value;
                Render();
            }
        }

        private float _minimum = 0;
        internal float Minimum
        {
            get
            {
                return _minimum;
            }
            set
            {
                if (_minimum != value)
                {
                    _minimum = value;
                    Render();
                }
            }
        }

        private float _value = 50;
        internal float Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    Render();

                    Changed?.Invoke(value);
                }
            }
        }

        private float _maximum = 100;
        internal float Maximum
        {
            get
            {
                return _maximum;
            }
            set
            {
                if (_maximum != value)
                {
                    _maximum = value;
                    Render();
                }
            }
        }

        private bool _rangeLabels = true;
        internal bool RangeLabels
        {
            get
            {
                return _rangeLabels;
            }
            set
            {
                if (_rangeLabels != value)
                {
                    _rangeLabels = value;
                    Render();
                }
            }
        }

        internal Action<float> Changed { get; set; }

        private bool held = false;

        private static int slotHeight = 3;
        private static int sliderHeight = 15;
        private static int sliderWidth = 5;

        private void RangeSliderDown(int x, int y)
        {
            held = true;
            Render();
        }

        internal override void Render()
        {
            if (held && MouseManager.MouseState != MouseState.Left)
            {
                held = false;
            }

            if (held)
            {
                float relativeX = (float)(MouseManager.X - ScreenX);
                float clamped = Math.Clamp(relativeX, 0, Width - sliderWidth);
                //DrawString(clamped.ToString(), Color.Red, 0, 0);
                Value = (float)clamped.Map(0, Width - sliderWidth, (float)_minimum, (float)_maximum);

                WM.UpdateQueue.Enqueue(this);
            }

            Clear(Background);

            int slotY;
            int sliderY;

            if (_rangeLabels)
            {
                slotY = (sliderHeight / 2) - (slotHeight / 2);
                sliderY = 0;
            }
            else
            {
                slotY = (Height / 2) - (slotHeight / 2);
                sliderY = (Height / 2) - (sliderHeight / 2);
            }

            // Slot
            DrawFilledRectangle(0, slotY, Width, slotHeight, Color.FromArgb(168, 168, 168));

            // Slider
            DrawFilledRectangle(
                (int)(_value.Map((float)_minimum, (float)_maximum, 0, Width - sliderWidth)),
                sliderY,
                sliderWidth,
                sliderHeight,
                held ? Color.FromArgb(0, 71, 112) : Color.FromArgb(0, 115, 186)
            );

            if (_rangeLabels)
            {
                DrawString(_minimum.ToString(), Foreground, 0, Height - FontData.Height);

                DrawString(_maximum.ToString(), Foreground, Width - (FontData.Width * _maximum.ToString().Length), Height - FontData.Height);
            }

            WM.Update(this);
        }
    }
}
