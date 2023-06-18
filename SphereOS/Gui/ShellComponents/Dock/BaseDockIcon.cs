using Cosmos.System.Graphics;

namespace SphereOS.Gui.ShellComponents.Dock
{
    internal abstract class BaseDockIcon
    {
        internal BaseDockIcon(Bitmap image, bool doAnimation = true)
        {
            if (doAnimation)
            {
                Size = 1;
            }
            else
            {
                // Skip to the end of the animation.
                Size = Dock.IconSize;
            }

            Image = image;
        }

        private double _size;
        internal double Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;

                if (_image != null)
                {
                    SizedImage = _image.ResizeWidthKeepRatio((uint)Size);
                }
            }
        }

        private Bitmap _image;
        internal Bitmap Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                SizedImage = value.ResizeWidthKeepRatio((uint)Size);
            }
        }


        internal Bitmap SizedImage { get; private set; }

        internal bool Closing { get; set; } = false;

        internal bool CloseAnimationComplete
        {
            get
            {
                return Closing && (int)_size == 1;
            }
        }

        /// <summary>
        /// Run the dock icon's animation.
        /// </summary>
        /// <returns>If the dock needs to be rerendered.</returns>
        internal bool RunAnimation()
        {
            int oldSize = (int)Size;
            int goalSize = Closing ? 1 : Dock.IconSize;
            Size += (goalSize - Size) / 16;

            return (int)Size != (int)oldSize;
        }

        internal abstract void Clicked();
    }
}
