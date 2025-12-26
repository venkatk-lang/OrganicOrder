using System.Collections.Generic;
using UnityEngine;
namespace IACGGames
{
    /// <summary>
    /// A static class for general helpful methods
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Destroy all child objects of this transform (Unintentionally evil sounding).
        /// Use it like so:
        /// <code>
        /// transform.DestroyChildren();
        /// </code>
        /// </summary>
        public static void DestroyChildren(this Transform t)
        {
            int count = t.childCount;
            for (int i = count - 1; i >= 0; i--)
                Object.Destroy(t.GetChild(i).gameObject);
        }

        public static void ShuffleList<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int rand = Random.Range(0, i + 1);
                (list[i], list[rand]) = (list[rand], list[i]);
            }
        }
    }
}