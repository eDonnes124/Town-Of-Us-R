using System;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using AmongUs.GameOptions;

namespace TownOfUs.CrewmateRoles.SeerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Seer);
            if (!flag) return true;
            var role = Role.GetRole<Seer>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            var flag2 = role.SeerTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;

            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact.AbilityUsed)
            {
                role.Investigated.Add(role.ClosestPlayer.PlayerId);
            }
            if (interact.FullCooldownReset)
            {
                role.LastInvestigated = DateTime.UtcNow;
                return false;
            }
            else if (interact.GaReset)
            {
                role.LastInvestigated = DateTime.UtcNow;
                role.LastInvestigated = role.LastInvestigated.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.SeerCd);
                return false;
            }
            else if (interact.ZeroSecReset) return false;
            return false;
        }
    }
}
