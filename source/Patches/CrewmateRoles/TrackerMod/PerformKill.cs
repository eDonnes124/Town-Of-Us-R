using System;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using TownOfUs.Extensions;
using AmongUs.GameOptions;
using Unity.Services.Core.Scheduler.Internal;

namespace TownOfUs.CrewmateRoles.TrackerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill {
        
        public static bool Prefix(KillButton __instance) {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Tracker)) return true;
            var role = Role.GetRole<Tracker>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;

            var flag2 = role.TrackerTimer() == 0f;
            if (!flag2) return false;

            if (!__instance.enabled) return false;
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;
            PlayerControl target = role.ClosestPlayer;

            role.LastStartedTracking = DateTime.UtcNow;
            role.isTrackingPlayer = true;
            
            role.TrackPlayer(target);

            return false;
        }
    }
}