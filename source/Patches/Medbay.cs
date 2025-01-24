using HarmonyLib;

namespace TownOfUs
{
    internal class MedScan
    {
        [HarmonyPatch(typeof(MedScanMinigame))]
        private static class MedScanMinigamePatch
        {
            [HarmonyPatch(nameof(MedScanMinigame.Begin))]
            [HarmonyPostfix]
            private static void BeginPostfix(MedScanMinigame __instance)
            {
                if (PlayerControl.LocalPlayer.Is(ModifierEnum.Giant))
                {
                    __instance.completeString = __instance.completeString.Replace("3' 6\"", "5' 3\"").Replace("92lb", "184lb");
                }
                else if (PlayerControl.LocalPlayer.Is(ModifierEnum.Mini))
                {
                    __instance.completeString = __instance.completeString.Replace("3' 6\"", "1' 9\"").Replace("92lb", "46lb");
                }
            }
        }
    }
}