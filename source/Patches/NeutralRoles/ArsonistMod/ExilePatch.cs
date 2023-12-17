using HarmonyLib;
using TownOfUs.Roles;
using System.Linq;
using TownOfUs.Extensions;
using UnityEngine;
using System;
using TownOfUs.Patches;

namespace TownOfUs.NeutralRoles.ArsonistMod
{
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    public static class AirshipExileController_WrapUpAndSpawn
    {
        public static void Postfix(AirshipExileController __instance) => SetLastKillerBool.ExileControllerPostfix(__instance);
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public class SetLastKillerBool
    {

        public static void ExileControllerPostfix(ExileController __instance)
        {
            var exiled = __instance.exiled?.Object;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist)) return;
            var alives = PlayerControl.AllPlayerControls.ToArray()
                    .Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList();
            foreach (var player in alives)
            {
                if (player.Data.IsImpostor() || player.Is(Faction.Impostors))
                {
                    return;
                }
            }
            var role = Role.GetRole<Arsonist>(PlayerControl.LocalPlayer);
            role.LastKiller = true;
            return;
        }

        public static void Postfix(ExileController __instance) => ExileControllerPostfix(__instance);

        [HarmonyPatch(typeof(UnityEngine.Object), nameof(UnityEngine.Object.Destroy), new Type[] { typeof(GameObject) })]
        public static void Prefix(GameObject obj)
        {
            if (!SubmergedCompatibility.Loaded || GameOptionsManager.Instance?.currentNormalGameOptions?.MapId != 6) return;
            if (obj.name?.Contains("ExileCutscene") == true) ExileControllerPostfix(ExileControllerPatch.lastExiled);
        }
    }
}