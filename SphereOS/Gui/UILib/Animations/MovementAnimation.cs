using System;
using System.Drawing;
using SphereOS.Gui;
using SphereOS.Gui.UILib;

namespace SphereOS.UILib.Animations
{
    /// <summary>
    /// An animation that moves or resizes a window.
    /// </summary>
    internal class MovementAnimation : Animation
    {
        /// <summary>
        /// Initialise the animation.
        /// </summary>
        /// <param name="window">The window associated with the animation.</param>
        /// <param name="to">The goal of the animation.</param>
        internal MovementAnimation(Window window)
        {
            Window = window;
            From = new Rectangle(window.X, window.Y, window.Width, window.Height);
        }

        /// <summary>
        /// The starting rectangle of the animation.
        /// </summary>
        internal Rectangle From;

        /// <summary>
        /// The goal rectangle of the animation. 
        /// </summary>
        internal Rectangle To;

        internal override bool Advance()
        {
            if (From.IsEmpty || To.IsEmpty) throw new Exception("The From or To value of this MovementAnimation is empty.");
            Position++;
            if (Position == Duration)
            {
                Window.MoveAndResize(To.X, To.Y, To.Width, To.Height);
                if (Window is Control control)
                {
                    control.Render();
                }
            }
            else
            {
                double t = Easing.Ease(Position / (double)Duration, EasingType, EasingDirection);
                Rectangle current = new Rectangle(
                    (int)Easing.Lerp(From.X, To.X, t),
                    (int)Easing.Lerp(From.Y, To.Y, t),
                    (int)Easing.Lerp(From.Width, To.Width, t),
                    (int)Easing.Lerp(From.Height, To.Height, t)
                );
                Window.MoveAndResize(current.X, current.Y, current.Width, current.Height);
                if (Window is Control control)
                {
                    control.Render();
                }
            }
            return Finished;
        }
    }
}
