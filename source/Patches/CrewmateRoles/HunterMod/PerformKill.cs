using AmongUs.GameOptions;
using HarmonyLib;
using System;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.HunterMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class Stalk
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Hunter)) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Hunter>(PlayerControl.LocalPlayer);
            if (__instance == role.StalkButton)
            {
                if (role.ClosestStalkPlayer == null) return false;
                if (!role.StalkUsable) return false;
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (role.StalkTimer() != 0) return false;
                var stalkInteract = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestStalkPlayer, false);
                if (stalkInteract[4] == true)
                {
                    role.StalkDuration = CustomGameOptions.HunterStalkDuration;
                    role.UsesLeft--;
                    role.StalkedPlayer = role.ClosestStalkPlayer;
                    role.Stalk();
                    Utils.Rpc(CustomRPC.HunterStalk, PlayerControl.LocalPlayer.PlayerId, role.ClosestStalkPlayer.PlayerId);
                }
                if (stalkInteract[0] == true)
                {
                    role.LastStalked = DateTime.UtcNow;
                }
                else if (stalkInteract[1] == true)
                {
                    role.LastStalked = DateTime.UtcNow;
                    role.LastStalked = role.LastKilled.AddSeconds(-CustomGameOptions.HunterKillCd + CustomGameOptions.ProtectKCReset);
                }
                return false;
            }

            if (role.ClosestPlayer == null) return false;
            if (!role.CaughtPlayers.Contains(role.ClosestPlayer)) return false;
            if (role.HunterKillTimer() != 0) return false;
            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers <
                        GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (!flag3) return false;
            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, true);
            if (interact[0] == true)
            {
                role.LastKilled = DateTime.UtcNow;
            }
            else if (interact[1] == true)
            {
                role.LastKilled = DateTime.UtcNow;
                role.LastKilled = role.LastKilled.AddSeconds(-CustomGameOptions.HunterKillCd + CustomGameOptions.ProtectKCReset);
            }
            else if (interact[2] == true)
            {
                role.LastKilled = DateTime.UtcNow;
                role.LastKilled = role.LastKilled.AddSeconds(-CustomGameOptions.HunterKillCd + CustomGameOptions.VestKCReset);
            }
            return false;
        }
    }
}