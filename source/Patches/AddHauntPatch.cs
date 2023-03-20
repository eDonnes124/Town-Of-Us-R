using HarmonyLib;
using System.Collections.Generic;

namespace TownOfUs.Patches
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
    [HarmonyPriority(Priority.First)]
    class AddHauntPatch
    {
        public static List<PlayerControl> AssassinatedPlayers = new List<PlayerControl>();
        public static void Prefix(ExileController __instance)
        {
            foreach (var player in AssassinatedPlayers)
            {
                player.Exiled();
            }
            AssassinatedPlayers.Clear();
        }
    }
}