using System.Collections.Generic;

namespace SphereOS.Core
{
    internal static class Extensions
    {
        // https://stackoverflow.com/questions/450233/generic-list-moving-an-item-within-the-list
        internal static void Move<T>(this List<T> list, int oldIndex, int newIndex)
        {
            var item = list[oldIndex];

            list.RemoveAt(oldIndex);

            if (newIndex > oldIndex) newIndex--;

            list.Insert(newIndex, item);
        }

        // https://stackoverflow.com/questions/14353485/how-do-i-map-numbers-in-c-sharp-like-with-map-in-arduino
        public static float Map(this float value, float fromSource, float toSource, float fromTarget, float toTarget)
        {
            return (value - fromSource)
                   / (toSource - fromSource)
                   * (toTarget - fromTarget)
                   + fromTarget;
        }
    }
}
