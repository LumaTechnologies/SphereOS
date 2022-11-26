using Cosmos.System.Graphics;
using System;
using System.Data.Common;
using System.Drawing;

namespace SphereOS.Gui.UILib
{
    internal class Calendar : Control
    {
        public Calendar(Window parent, int x, int y, int width, int height) : base(parent, x, y, width, height)
        {
        }

        internal void SetCalendar(int year, int month)
        {
            _year = year;
            _month = month;
            Render();
        }

        private int _year = DateTime.Now.Year;
        internal int Year
        {
            get
            {
                return _year;
            }
            set
            {
                _year = value;
                Render();
            }
        }

        private int _month = DateTime.Now.Month;
        internal int Month
        {
            get
            {
                return _month;
            }
            set
            {
                _month = value;
                Render();
            }
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

        private Color _foreground = Color.Black;
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

        private Color _weekendForeground = Color.Red;
        internal Color WeekendForeground
        {
            get
            {
                return _weekendForeground;
            }
            set
            {
                _weekendForeground = value;
                Render();
            }
        }

        private Color _todayBackground = Color.LightGray;
        internal Color TodayBackground
        {
            get
            {
                return _todayBackground;
            }
            set
            {
                _todayBackground = value;
                Render();
            }
        }

        private readonly string[] weekdaysShort = new string[]
        {
            "Mon",
            "Tue",
            "Wed",
            "Thu",
            "Fri",
            "Sat",
            "Sun"
        };

        private readonly string[] monthsLong = new string[]
        {
            "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December"
        };

        private int GetWeekdayIndex(DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Monday => 0,
                DayOfWeek.Tuesday => 1,
                DayOfWeek.Wednesday => 2,
                DayOfWeek.Thursday => 3,
                DayOfWeek.Friday => 4,
                DayOfWeek.Saturday => 5,
                DayOfWeek.Sunday => 6,
                _ => throw new Exception("Invalid DayOfWeek.")
            };
        }

        private const int cellPadding = 4;

        internal override void Render()
        {
            Clear(_background);

            DateTime now = DateTime.Now;
            int daysInMonth = DateTime.DaysInMonth(_year, _month);
            int startingWeekday = GetWeekdayIndex(new DateTime(_year, _month, 1).DayOfWeek);

            int headerHeight = 68;

            Rectangle availableSpace = new Rectangle(0, headerHeight, Width, Height - headerHeight);

            int cellWidth = availableSpace.Width / 7;
            int cellHeight = availableSpace.Height / 5;

            /* Header */
            string title = $"{monthsLong[_month - 1]} {_year}";
            DrawString(title, _foreground, (Width / 2) - ((title.Length * 8) / 2), 12);
            for (int i = 0; i < 7; i++)
            {
                string weekday = weekdaysShort[i];
                DrawString(weekday, _foreground, (i * cellWidth) + ((cellWidth / 2) - weekday.Length * (8 / 2) / 2), 40);
            }

            /* Days */
            int cellX = startingWeekday;
            int cellY = 0;
            for (int i = 1; i <= daysInMonth; i++)
            {
                if (cellX > 6)
                {
                    cellX = 0;
                    cellY++;
                }

                string str = i.ToString();
                bool weekend = cellX >= 5;

                int cellWindowX = availableSpace.X + (cellX * cellWidth);
                int cellWindowY = availableSpace.Y + (cellY * cellHeight);

                int textWindowX = (cellWindowX + cellWidth) - (8 * str.Length) - cellPadding;
                int textWindowY = cellWindowY + cellPadding;

                if (_year == now.Year && _month == now.Month && i == now.Day)
                {
                    DrawFilledRectangle(cellWindowX, cellWindowY, cellWidth, cellHeight, _todayBackground);
                }

                DrawString(str, weekend ? _weekendForeground : _foreground, textWindowX, textWindowY);

                cellX++;
            }

            WM.Update(this);
        }
    }
}
