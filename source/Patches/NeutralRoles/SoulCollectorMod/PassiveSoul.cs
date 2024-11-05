using System;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUs.Patches;
using TownOfUs.Roles;
using TownOfUs.CrewmateRoles.AltruistMod;

namespace TownOfUs.NeutraleRoles.SoulCollectorMod
{
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    public static class AirshipExileController_WrapUpAndSpawn
    {
        public static void Postfix(AirshipExileController __instance) => PassiveSoul.ExileControllerPostfix(__instance);
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public class PassiveSoul
    {
        public static void ExileControllerPostfix(ExileController __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.SoulCollector))
            {
                var sc = (SoulCollector)role;
                if (CustomGameOptions.PassiveSoulCollection) sc.SoulsCollected += 1;
                if (sc.SoulsCollected >= CustomGameOptions.SoulsToWin)
                {
                    sc.CollectedSouls = true;

                    if (!CustomGameOptions.SoulCollectorEndsGame)
                    {
                        KillButtonTarget.DontRevive = sc.Player.PlayerId;
                        sc.Player.Exiled();
                    }
                }
            }
        }

        public static void Postfix(ExileController __instance) => ExileControllerPostfix(__instance);

        [HarmonyPatch(typeof(Object), nameof(Object.Destroy), new Type[] { typeof(GameObject) })]
        public static void Prefix(GameObject obj)
        {
            if (!SubmergedCompatibility.Loaded || GameOptionsManager.Instance?.currentNormalGameOptions?.MapId != 6) return;
            if (obj.name?.Contains("ExileCutscene") == true) ExileControllerPostfix(ExileControllerPatch.lastExiled);
        }
    }
}