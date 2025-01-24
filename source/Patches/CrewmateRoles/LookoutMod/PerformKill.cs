using System;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using AmongUs.GameOptions;
using System.Collections.Generic;

namespace TownOfUs.CrewmateRoles.LookoutMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static Sprite Sprite => TownOfUs.Arrow;
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Lookout)) return true;
            var role = Role.GetRole<Lookout>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            var flag2 = role.WatchTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;
            var target = role.ClosestPlayer;
            if (!role.ButtonUsable) return false;

            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[4] == true)
            {
                role.Watching.Add(role.ClosestPlayer.PlayerId, new List<RoleEnum>());
                role.UsesLeft--;
            }
            if (interact[0] == true)
            {
                role.LastWatched = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastWatched = DateTime.UtcNow;
                role.LastWatched = role.LastWatched.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.WatchCooldown);
                return false;
            }
            else if (interact[3] == true) return false;
            return false;
        }
    }
}
