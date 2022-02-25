using System;
using HarmonyLib;
using UnityEngine;

namespace TownOfUs.RainbowMod
{
    [HarmonyPatch(typeof(PlayerTab))]
    public static class PlayerTabPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(PlayerTab.OnEnable))]
        public static void OnEnablePostfix(PlayerTab __instance)
        {
            for (int i = 0; i < __instance.ColorChips.Count; i++)
            {
                var colorChip = __instance.ColorChips[i];
                colorChip.transform.localScale *= 0.8f;
                var x = __instance.XRange.Lerp((i % 4) / 4f) + 0.25f;
                var y = __instance.YStart - (i / 4) * 0.55f;
                colorChip.transform.localPosition = new Vector3(x, y, -1f);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(PlayerTab.Update))]
        public static void UpdatePostfix(PlayerTab __instance)
        {
            for (int i = 0; i < __instance.ColorChips.Count; i++)
            {
                if (RainbowUtils.IsRainbow(i))
                {
                    __instance.ColorChips[i].Inner.color = RainbowUtils.Rainbow;
                    break;
                }
            }

        }
    }
}
