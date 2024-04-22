using HarmonyLib;

namespace TownOfUs.Patches;

[HarmonyPatch(typeof(LongBoiPlayerBody))]
public static class LongBoiPatches
{
    [HarmonyPatch(nameof(LongBoiPlayerBody.SetHeightFromColor))]
    [HarmonyPrefix]
    public static void LongBoy_ColorHeightPatch(LongBoiPlayerBody __instance, ref int colorIndex)
    {
        while (colorIndex >= __instance.heightsPerColor.Count) colorIndex -= __instance.heightsPerColor.Count;
    }
}