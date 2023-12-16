using System.Collections.Generic;
using UnityEngine;

namespace TownOfUs
{
    public static class ListExtensions
    {
        public static void Shuffle<T>(this List<T> list)
        {
            for (var i = list.Count - 1; i > 0; --i)
            {
                var j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        public static T TakeFirst<T>(this List<T> list)
        {
            try
            {
                var item = list[0];
                list.RemoveAt(0);
                return item;
            }
            catch
            {
                return default;
            }
        }

        public static T Ability<T>(this List<T> list)
        {
            var item = list[0];
            return item;
        }
    }
}