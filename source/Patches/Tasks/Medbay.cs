using System;
using HarmonyLib;

namespace TownOfUs.Tasks
{
	internal class BigBoiMedScan
	{

		[HarmonyPatch(typeof(MedScanMinigame))]
		private static class MedScanMinigamePatch
		{
			[HarmonyPatch(nameof(MedScanMinigame.Begin))]
			[HarmonyPostfix]
			private static void BeginPostfix(MedScanMinigame __instance)
			{
                // Update medical details for Giant modifier
                if (PlayerControl.LocalPlayer.Is(ModifierEnum.BigBoi))
                {
					__instance.completeString = __instance.completeString.Replace("3' 6\"", "5' 0\"").Replace("92lb", "188lb");
				}
			}
		}
	}
}