using System;

namespace SphereOS.UILib.Animations
{
    /// <summary>
    /// Easing utilities for animations.
    /// </summary>
    internal static class Easing
    {
        /// <summary>
        /// Calculate the value of an easing function.
        /// </summary>
        /// <param name="t">The absolute progress of the animation, from 0 to 1.</param>
        /// <param name="type">The type of the easing function.</param>
        /// <param name="direction">The direction of the easing function.</param>
        /// <returns>The value of the easing function at the given progress.</returns>
        /// <exception cref="ArgumentOutOfRangeException">An exception is thrown if the progress is out of range.</exception>
        /// <exception cref="ArgumentException">An exception is thrown if the type or direction is ininvalid.</exception>
        internal static double Ease(double t, EasingType type, EasingDirection direction)
        {
            if (t < 0 || t > 1) throw new ArgumentOutOfRangeException();
            switch (type)
            {
                case EasingType.Linear:
                    return t;
                case EasingType.Sine:
                    switch (direction)
                    {
                        case EasingDirection.In:
                            return 1 - Math.Cos(t * Math.PI / 2);
                        case EasingDirection.Out:
                            return Math.Sin(t * Math.PI / 2);
                        case EasingDirection.InOut:
                            return -0.5 * (Math.Cos(Math.PI * t) - 1);
                        default:
                            throw new ArgumentException("Unknown easing direction.");
                    }
                default:
                    throw new ArgumentException("Unknown easing type.");
            }
        }

        /// <summary>
        /// Linearly interpolate between two values.
        /// </summary>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        /// <param name="z">The value of the interpolation.</param>
        /// <returns>The interpolated value.</returns>
        internal static double Lerp(double x, double y, double z)
        {
            return x * (1 - z) + y * z;
        }
    }
}
