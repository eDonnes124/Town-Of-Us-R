using AmongUs.GameOptions;
using HarmonyLib;
using System;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.WerewolfMod
{
    [HarmonyPatch]
    public class PerformKill
    {
        [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
        public static bool Prefix(AbilityButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Werewolf);
            if (!flag) return true;
            if (RoleManager.IsGhostRole(PlayerControl.LocalPlayer.Data.RoleType)) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            var role = Role.GetRole<Werewolf>(PlayerControl.LocalPlayer);
            if (role.Player.inVent) return false;

            if (role.RampageTimer() != 0) return false;
            if (!__instance.isActiveAndEnabled || __instance.isCoolingDown) return false;

            role.TimeRemaining = CustomGameOptions.RampageDuration;
            role.Rampage();
            return false;

        }

        [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Werewolf);
            if (!flag) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            var role = Role.GetRole<Werewolf>(PlayerControl.LocalPlayer);
            if (role.Player.inVent) return false;

            if (role.KillTimer() != 0) return false;
            if (!role.Rampaged) return false;
            if (__instance != HudManager.Instance.KillButton) return true;
            if (!__instance.isActiveAndEnabled || __instance.isCoolingDown) return false;
            if (role.ClosestPlayer == null) return false;
            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers <
                        GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (!flag3) return false;

            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, true);
            if (interact[4] == true) return false;
            else if (interact[0] == true)
            {
                role.LastKilled = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastKilled = DateTime.UtcNow;
                role.LastKilled = role.LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.RampageKillCd);
                return false;
            }
            else if (interact[2] == true)
            {
                role.LastKilled = DateTime.UtcNow;
                role.LastKilled = role.LastKilled.AddSeconds(CustomGameOptions.VestKCReset - CustomGameOptions.RampageKillCd);
                return false;
            }
            else if (interact[3] == true) return false;
            return false;
        }
    }
}
