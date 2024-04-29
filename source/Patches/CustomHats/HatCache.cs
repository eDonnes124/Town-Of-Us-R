using System.Collections.Generic;
using UnityEngine;

namespace TownOfUs.Patches.CustomHats
{
    public static class HatCache
    {
        public static SortedList<string, List<HatData>> SortedHats = null;
        
        public static GameObject ChipHolder = null;

        public static Dictionary<string, Transform> Children = [];

        public static int hatIdx = 0;
    }
}
