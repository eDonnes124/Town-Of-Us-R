using System;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using AmongUs.GameOptions;

namespace TownOfUs.CrewmateRoles.VampireHunterMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.VampireHunter)) return true;
            var role = Role.GetRole<VampireHunter>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            var flag2 = role.StakeTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;
            if (!role.ButtonUsable) return false;

            if (!role.ClosestPlayer.Is(RoleEnum.Vampire))
            {
                var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
                if (interact[0] == true)
                {
                    role.LastStaked = DateTime.UtcNow;
                    role.UsesLeft--;
                    if (role.UsesLeft == 0 && role.CorrectKills == 0 && CustomGameOptions.SelfKillAfterFinalStake)
                        Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);
                    return false;
                }
                else if (interact[1] == true)
                {
                    role.LastStaked = DateTime.UtcNow;
                    role.LastStaked = role.LastStaked.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.StakeCd);
                    return false;
                }
                else if (interact[3] == true) return false;
                return false;
            }
            else
            {
                var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, true);
                if (interact[4] == true) return false;
                else if (interact[0] == true)
                {
                    role.LastStaked = DateTime.UtcNow;
                    return false;
                }
                else if (interact[1] == true)
                {
                    role.LastStaked = DateTime.UtcNow;
                    role.LastStaked = role.LastStaked.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.StakeCd);
                    return false;
                }
                else if (interact[3] == true) return false;
                return false;
            }
        }
    }
}
