using SphereOS.Gui;
using System;

namespace SphereOS.UILib.Animations
{
    /// <summary>
    /// A window animation.
    /// </summary>
    internal abstract class Animation
    {
        /// <summary>
        /// The easing type of the animation.
        /// </summary>
        internal EasingType EasingType { get; set; } = EasingType.Sine;

        /// <summary>
        /// The direction of the easing of the animation.
        /// </summary>
        internal EasingDirection EasingDirection { get; set; } = EasingDirection.Out;

        /// <summary>
        /// The duration of the animation.
        /// </summary>
        internal int Duration { get; set; } = 60;

        /// <summary>
        /// How many frames of the animation have been completed.
        /// </summary>
        internal int Position { get; set; } = 0;

        /// <summary>
        /// If the animation has finished.
        /// </summary>
        internal bool Finished
        {
            get
            {
                return Position >= Duration;
            }
        }

        /// <summary>
        /// The window associated with the animation.
        /// </summary>
        internal Window Window { get; set; }

        /// <summary>
        /// Advance the animation by one frame.
        /// </summary>
        /// <returns>Whether or not the animation is now finished.</returns>
        internal abstract bool Advance();

        private int? timerId { get; set; } = null;

        /// <summary>
        /// Start the animation.
        /// </summary>
        internal void Start()
        {
            if (timerId == null)
            {
                timerId = Cosmos.HAL.Global.PIT.RegisterTimer(new Cosmos.HAL.PIT.PITTimer(() =>
                {
                    Advance();
                    if (Finished)
                    {
                        Stop();
                    }
                }, (ulong)((1000d /* ms */ / 60d) * 1e+6d /* ms -> ns */ ), true));
            }
        }

        /// <summary>
        /// Stop the animation.
        /// </summary>
        internal void Stop()
        {
            if (timerId != null)
            {
                Cosmos.HAL.Global.PIT.UnregisterTimer((int)timerId);
                timerId = null;
            }
        }
    }
}
