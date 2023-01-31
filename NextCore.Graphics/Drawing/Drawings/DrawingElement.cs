using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextCore.Graphics.Drawings
{
    public abstract class DrawingElement
    {
        private float _x;
        public float X
        {
            get
            {
                return _x;
            }

            set
            {
                _x = value;
            }
        }

        private float _y;
        public float Y
        {
            get
            {
                return _y;
            }

            set
            {
                _y = value;
            }
        }

        private float _width;
        public float Width
        {
            get
            {
                return _width;
            }

            set
            {
                if (value < 0)
                {
                    throw new Exception("The width of a DrawingElement must be greater than 0.");
                }
                _width = value;
            }
        }

        private float _height;
        public float Height
        {
            get
            {
                return _height;
            }

            set
            {
                if (value < 0)
                {
                    throw new Exception("The height of a DrawingElement must be greater than 0.");
                }
                _height = value;
            }
        }

        public RectangleF Rectangle
        {
            get
            {
                return new RectangleF(X, Y, Width, Height);
            }

            set
            {
                X = value.X;

                Y = value.Y;

                Width = (uint)value.Width;

                Height = (uint)value.Height;
            }
        }
    }
}
